using System.Net;
using System.Text.Json;
using cs2_esports.IntegrationTests.Infrastructure;
using cs2_esports.Models;

namespace cs2_esports.IntegrationTests;

public class HomeSearchTests : IClassFixture<Cs2ScopeWebApplicationFactory>
{
    private readonly Cs2ScopeWebApplicationFactory _factory;

    public HomeSearchTests(Cs2ScopeWebApplicationFactory factory)
    {
        _factory = factory;
        _factory.ResetDatabase();
    }

    [Fact]
    public async Task Search_ReturnsCategorizedPlayersTeamsAndEvents()
    {
        SeedSearchData();
        using var client = _factory.CreateClient();

        var response = await client.GetAsync("/search?query=Falcon");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        using var document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var root = document.RootElement;

        Assert.Equal(3, root.GetProperty("total").GetInt32());
        Assert.Equal("FalconEye", root.GetProperty("players")[0].GetProperty("title").GetString());
        Assert.Equal("Playing for Falcons", root.GetProperty("players")[0].GetProperty("meta").GetString());
        Assert.Equal("/player/FalconEye", root.GetProperty("players")[0].GetProperty("url").GetString());
        Assert.Equal("Falcons", root.GetProperty("teams")[0].GetProperty("title").GetString());
        Assert.Equal("/team/Falcons", root.GetProperty("teams")[0].GetProperty("url").GetString());
        Assert.Equal("Falcon Cup", root.GetProperty("events")[0].GetProperty("title").GetString());
        Assert.Equal("/event/Falcon-Cup", root.GetProperty("events")[0].GetProperty("url").GetString());
    }

    [Fact]
    public async Task Search_MatchesPlayerFullNameTeamTagAndEventOrganizer()
    {
        SeedSearchData();
        using var client = _factory.CreateClient();

        using var playerResult = JsonDocument.Parse(await client.GetStringAsync("/search?query=Alex"));
        using var teamResult = JsonDocument.Parse(await client.GetStringAsync("/search?query=FLC"));
        using var eventResult = JsonDocument.Parse(await client.GetStringAsync("/search?query=ScopeWorks"));

        Assert.Equal("FalconEye", playerResult.RootElement.GetProperty("players")[0].GetProperty("title").GetString());
        Assert.Equal("Falcons", teamResult.RootElement.GetProperty("teams")[0].GetProperty("title").GetString());
        Assert.Equal("Falcon Cup", eventResult.RootElement.GetProperty("events")[0].GetProperty("title").GetString());
    }

    [Fact]
    public async Task Search_ReturnsEmptyGroups_WhenQueryIsTooShort()
    {
        using var client = _factory.CreateClient();

        using var result = JsonDocument.Parse(await client.GetStringAsync("/search?query=F"));

        Assert.Equal(0, result.RootElement.GetProperty("total").GetInt32());
        Assert.Empty(result.RootElement.GetProperty("pages").EnumerateArray());
        Assert.Empty(result.RootElement.GetProperty("players").EnumerateArray());
        Assert.Empty(result.RootElement.GetProperty("teams").EnumerateArray());
        Assert.Empty(result.RootElement.GetProperty("events").EnumerateArray());
    }

    [Fact]
    public async Task Search_ReturnsNavigationAndPublicPages()
    {
        using var client = _factory.CreateClient();

        using var dashboardResult = JsonDocument.Parse(await client.GetStringAsync("/search?query=dashboard"));
        using var communityResult = JsonDocument.Parse(await client.GetStringAsync("/search?query=community"));

        var homePage = dashboardResult.RootElement.GetProperty("pages")[0];
        Assert.Equal("Home", homePage.GetProperty("title").GetString());
        Assert.Equal("/", homePage.GetProperty("url").GetString());

        var forumsPage = communityResult.RootElement.GetProperty("pages")[0];
        Assert.Equal("Forums", forumsPage.GetProperty("title").GetString());
        Assert.Equal("/Forums", forumsPage.GetProperty("url").GetString());
    }

    [Fact]
    public async Task Search_HidesProfileForGuestsAndDoesNotIncludePrivacyPage()
    {
        using var client = _factory.CreateClient();

        using var profileResult = JsonDocument.Parse(await client.GetStringAsync("/search?query=profile"));
        using var privacyResult = JsonDocument.Parse(await client.GetStringAsync("/search?query=privacy"));

        Assert.DoesNotContain(
            profileResult.RootElement.GetProperty("pages").EnumerateArray(),
            page => page.GetProperty("title").GetString() == "My Profile");
        Assert.Empty(privacyResult.RootElement.GetProperty("pages").EnumerateArray());
    }

    private void SeedSearchData()
    {
        _factory.WithDbContext(db =>
        {
            var team = new Team
            {
                Name = "Falcons",
                Tag = "FLC",
                CountryCode = "SA",
                WorldRanking = 8,
                FoundedYear = 2017
            };
            team.Players.Add(new Player
            {
                Nickname = "FalconEye",
                FullName = "Alex Search",
                CountryCode = "HR",
                DateOfBirth = new DateTime(2000, 1, 1),
                JoinedTeamAtUtc = new DateTime(2025, 1, 1)
            });

            db.Teams.Add(team);
            db.Tournaments.Add(new Event
            {
                Name = "Falcon Cup",
                Organizer = "ScopeWorks",
                Tier = EventTier.A,
                StartDateUtc = new DateTime(2026, 8, 1),
                EndDateUtc = new DateTime(2026, 8, 5),
                EventVenue = new EventVenue
                {
                    Name = "Search Arena",
                    City = "Zagreb",
                    CountryCode = "HR",
                    Capacity = 1000,
                    IsIndoor = true,
                    SurfaceType = "Stage"
                }
            });
            db.SaveChanges();
            return true;
        });
    }
}
