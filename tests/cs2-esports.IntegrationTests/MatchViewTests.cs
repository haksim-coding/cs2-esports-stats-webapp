using System.Net;
using System.Net.Http.Json;
using System.Text.RegularExpressions;
using cs2_esports.IntegrationTests.Infrastructure;
using cs2_esports.Models;

namespace cs2_esports.IntegrationTests;

public partial class MatchViewTests : IClassFixture<Cs2ScopeWebApplicationFactory>
{
    private readonly Cs2ScopeWebApplicationFactory _factory;

    public MatchViewTests(Cs2ScopeWebApplicationFactory factory)
    {
        _factory = factory;
        _factory.ResetDatabase();
    }

    [Fact]
    public async Task Details_ShowsTheEventsVenue()
    {
        var ids = SeedMatchDependencies(createMatch: true);
        using var client = _factory.CreateClient();

        var html = await client.GetStringAsync($"/matches/details/{ids.MatchId}");

        Assert.Contains("Test Arena", html);
        Assert.Contains("Zagreb, HR", html);
    }

    [Fact]
    public async Task Create_ShowsErrorWhenFinishedTimeIsBeforeStartTime()
    {
        var ids = SeedMatchDependencies();
        using var client = _factory.CreateSuperAdminClient();
        var token = await GetAntiforgeryToken(client);
        var form = ValidMatchForm(ids, token);
        form["ScheduledAtUtc"] = "2026-09-01T18:00:00Z";
        form["FinishedAtUtc"] = "2026-09-01T17:00:00Z";

        var response = await client.PostAsync("/matches/create", new FormUrlEncodedContent(form));
        var html = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("Finished date and time must be on or after the match start date and time.", html);
        Assert.Equal(0, _factory.WithDbContext(db => db.Matches.Count()));
    }

    [Fact]
    public async Task Create_ShowsErrorForInvalidMapScore()
    {
        var ids = SeedMatchDependencies();
        using var client = _factory.CreateSuperAdminClient();
        var token = await GetAntiforgeryToken(client);
        var form = ValidMatchForm(ids, token);
        form["Maps[0].TeamAScore"] = "12";
        form["Maps[0].TeamBScore"] = "10";

        var response = await client.PostAsync("/matches/create", new FormUrlEncodedContent(form));
        var html = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("Map 1 has an invalid regulation score", html);
        Assert.Equal(0, _factory.WithDbContext(db => db.Matches.Count()));
    }

    [Fact]
    public async Task Create_AllowsUnfinishedMatchWithoutMaps()
    {
        var ids = SeedMatchDependencies();
        using var client = _factory.CreateSuperAdminClient();
        var token = await GetAntiforgeryToken(client);
        var form = ValidMatchForm(ids, token);
        RemoveMapResults(form);

        var response = await client.PostAsync("/matches/create", new FormUrlEncodedContent(form));

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var match = _factory.WithDbContext(db => db.Matches.Single());
        Assert.False(match.IsFinished);
        Assert.Null(match.FinishedAtUtc);
        Assert.Empty(match.Maps);
    }

    [Fact]
    public async Task Create_RequiresMapsWhenFinishedTimeIsAssigned()
    {
        var ids = SeedMatchDependencies();
        using var client = _factory.CreateSuperAdminClient();
        var token = await GetAntiforgeryToken(client);
        var form = ValidMatchForm(ids, token);
        RemoveMapResults(form);
        form["FinishedAtUtc"] = "2026-09-01T20:00:00Z";

        var response = await client.PostAsync("/matches/create", new FormUrlEncodedContent(form));
        var html = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("At least 1 map result(s) are required for a finished Best Of 1 match.", html);
        Assert.Equal(0, _factory.WithDbContext(db => db.Matches.Count()));
    }

    [Fact]
    public async Task EventTeams_ReturnsOnlyTeamsFromTheSelectedEvent()
    {
        var ids = SeedMatchDependencies();
        using var client = _factory.CreateSuperAdminClient();

        var teams = await client.GetFromJsonAsync<List<TeamOption>>($"/matches/eventteams?eventId={ids.EventId}");

        Assert.NotNull(teams);
        Assert.Equal([ids.TeamAId, ids.TeamBId], teams.Select(team => team.Id).Order().ToArray());
        Assert.DoesNotContain(teams, team => team.Id == ids.OtherTeamId);
    }

    [Fact]
    public async Task Create_RejectsTeamThatIsNotInTheSelectedEvent()
    {
        var ids = SeedMatchDependencies();
        using var client = _factory.CreateSuperAdminClient();
        var token = await GetAntiforgeryToken(client);
        var form = ValidMatchForm(ids, token);
        form["TeamBId"] = ids.OtherTeamId.ToString();

        var response = await client.PostAsync("/matches/create", new FormUrlEncodedContent(form));
        var html = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("Team B must be a team participating in the selected event.", html);
        Assert.Equal(0, _factory.WithDbContext(db => db.Matches.Count()));
    }

    private MatchIds SeedMatchDependencies(bool createMatch = false)
    {
        return _factory.WithDbContext(db =>
        {
            var venue = new EventVenue
            {
                Name = "Test Arena",
                City = "Zagreb",
                CountryCode = "HR",
                Capacity = 10000,
                IsIndoor = true,
                SurfaceType = "Stage"
            };
            var eventItem = new Event
            {
                Name = "Test Event",
                Organizer = "Test Organizer",
                Tier = EventTier.A,
                PrizePoolUsd = 100000,
                StartDateUtc = new DateTime(2026, 8, 1),
                EndDateUtc = new DateTime(2026, 9, 5),
                IsLan = true,
                EventVenue = venue
            };
            var teamA = NewTeam("Team A", "TMA", 1);
            var teamB = NewTeam("Team B", "TMB", 2);
            var otherTeam = NewTeam("Other Team", "OTH", 3);
            eventItem.Teams.Add(teamA);
            eventItem.Teams.Add(teamB);

            db.Tournaments.Add(eventItem);
            db.Teams.AddRange(teamA, teamB, otherTeam);

            cs2_esports.Models.Match? match = null;
            if (createMatch)
            {
                match = new cs2_esports.Models.Match
                {
                    ScheduledAtUtc = new DateTime(2026, 9, 1),
                    Format = MatchFormat.BestOf1,
                    Event = eventItem,
                    TeamA = teamA,
                    TeamB = teamB
                };
                db.Matches.Add(match);
            }

            db.SaveChanges();
            return new MatchIds(eventItem.Id, teamA.Id, teamB.Id, otherTeam.Id, match?.Id ?? 0);
        });
    }

    private static Team NewTeam(string name, string tag, int ranking) => new()
    {
        Name = name,
        Tag = tag,
        CountryCode = "HR",
        WorldRanking = ranking,
        FoundedYear = 2020,
        PrizeMoneyUsd = 1000,
        LastRosterUpdateUtc = DateTime.UtcNow
    };

    private static Dictionary<string, string> ValidMatchForm(MatchIds ids, string token) => new()
    {
        ["__RequestVerificationToken"] = token,
        ["ScheduledAtUtc"] = "2026-09-01T18:00:00Z",
        ["Format"] = ((int)MatchFormat.BestOf1).ToString(),
        ["EventId"] = ids.EventId.ToString(),
        ["TeamAId"] = ids.TeamAId.ToString(),
        ["TeamBId"] = ids.TeamBId.ToString(),
        ["Maps[0].MapSequence"] = "1",
        ["Maps[0].Map"] = ((int)MapPool.Mirage).ToString(),
        ["Maps[0].TeamAScore"] = "13",
        ["Maps[0].TeamBScore"] = "8",
        ["Maps[0].WentToOvertime"] = "false"
    };

    private static void RemoveMapResults(Dictionary<string, string> form)
    {
        foreach (var key in form.Keys.Where(key => key.StartsWith("Maps[", StringComparison.Ordinal)).ToList())
        {
            form.Remove(key);
        }
    }

    private static async Task<string> GetAntiforgeryToken(HttpClient client)
    {
        var html = await client.GetStringAsync("/matches/create");
        var match = AntiforgeryTokenRegex().Match(html);
        Assert.True(match.Success, "The create-match page did not contain an antiforgery token.");
        return WebUtility.HtmlDecode(match.Groups[1].Value);
    }

    [GeneratedRegex("name=\"__RequestVerificationToken\"[^>]*value=\"([^\"]+)\"", RegexOptions.IgnoreCase)]
    private static partial Regex AntiforgeryTokenRegex();

    private sealed record MatchIds(int EventId, int TeamAId, int TeamBId, int OtherTeamId, int MatchId);
    private sealed class TeamOption
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
