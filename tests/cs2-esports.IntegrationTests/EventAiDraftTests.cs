using System.Net;
using System.Net.Http.Json;
using System.Text.RegularExpressions;
using cs2_esports.Dtos.Events;
using cs2_esports.IntegrationTests.Infrastructure;
using cs2_esports.Models;

namespace cs2_esports.IntegrationTests;

public partial class EventAiDraftTests : IClassFixture<Cs2ScopeWebApplicationFactory>
{
    private readonly Cs2ScopeWebApplicationFactory _factory;

    public EventAiDraftTests(Cs2ScopeWebApplicationFactory factory)
    {
        _factory = factory;
        _factory.ResetDatabase();
    }

    [Fact]
    public async Task CreateDraft_ReturnsStructuredDraft_WithoutSavingEvent()
    {
        using var client = _factory.CreateEslAdminClient();
        var token = await GetAntiforgeryToken(client);

        var response = await PostPrompt(
            client,
            token,
            "Create an ESL event called IEM Zagreb, starting 15 July 2026, with a $250,000 prize pool.");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<AiDraftResponse>();
        Assert.Equal("IEM Zagreb", payload!.Draft.Name);
        Assert.Equal("ESL", payload.Draft.Organizer);
        Assert.Equal(250000m, payload.Draft.PrizePoolUsd);
        Assert.Equal(new DateTime(2026, 7, 15, 0, 0, 0, DateTimeKind.Utc), payload.Draft.StartDateUtc);
        Assert.Equal(0, _factory.WithDbContext(db => db.Tournaments.Count()));
    }

    [Fact]
    public async Task CreateDraft_RejectsInvalidProviderResult()
    {
        using var client = _factory.CreateSuperAdminClient();
        var token = await GetAntiforgeryToken(client);

        var response = await PostPrompt(client, token, "Create an event with invalid dates");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(0, _factory.WithDbContext(db => db.Tournaments.Count()));
    }

    [Fact]
    public async Task CreateDraft_EnforcesOrganizerPermissions()
    {
        using var client = _factory.CreateEslAdminClient();
        var token = await GetAntiforgeryToken(client);

        var response = await PostPrompt(client, token, "Create a BLAST event in Zagreb");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Contains("not allowed", await response.Content.ReadAsStringAsync(), StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task CreateDraft_SelectsTopEightRankedTeams_WithoutSavingEvent()
    {
        SeedRankedTeams(10);
        using var client = _factory.CreateSuperAdminClient();
        var token = await GetAntiforgeryToken(client);

        var response = await PostPrompt(
            client,
            token,
            "Create PGL Masters Zagreb with the top 8 teams attending");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var payload = await response.Content.ReadFromJsonAsync<AiDraftResponse>();
        Assert.Equal(8, payload!.Draft.SelectedTeamIds!.Count);
        Assert.Equal(8, payload.SelectedTeams.Count);
        Assert.Equal(
            Enumerable.Range(1, 8),
            payload.SelectedTeams.Select(team => team.WorldRanking));
        Assert.Equal(0, _factory.WithDbContext(db => db.Tournaments.Count()));
    }

    private void SeedRankedTeams(int count)
    {
        _factory.WithDbContext(db =>
        {
            db.Teams.AddRange(Enumerable.Range(1, count).Select(ranking => new Team
            {
                Name = $"Ranked Team {ranking}",
                Tag = $"T{ranking}",
                CountryCode = "HR",
                WorldRanking = ranking,
                FoundedYear = 2020
            }));
            db.SaveChanges();
            return true;
        });
    }

    private static async Task<string> GetAntiforgeryToken(HttpClient client)
    {
        var html = await client.GetStringAsync("/Events/Create");
        var match = AntiforgeryTokenRegex().Match(html);
        Assert.True(match.Success, "The create-event page did not contain an antiforgery token.");
        return WebUtility.HtmlDecode(match.Groups[1].Value);
    }

    private static Task<HttpResponseMessage> PostPrompt(HttpClient client, string token, string prompt) =>
        client.PostAsync("/events/ai-draft", new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["Prompt"] = prompt,
            ["__RequestVerificationToken"] = token
        }));

    [GeneratedRegex("name=\"__RequestVerificationToken\"[^>]*value=\"([^\"]+)\"", RegexOptions.IgnoreCase)]
    private static partial Regex AntiforgeryTokenRegex();

    private sealed class AiDraftResponse
    {
        public AiEventDraft Draft { get; set; } = new();
        public List<SelectedTeamResponse> SelectedTeams { get; set; } = [];
    }

    private sealed class SelectedTeamResponse
    {
        public int Id { get; set; }
        public string Text { get; set; } = string.Empty;
        public int WorldRanking { get; set; }
    }
}
