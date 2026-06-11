using System.Net;
using System.Net.Http.Json;
using cs2_esports.Dtos.Matches;
using cs2_esports.IntegrationTests.Infrastructure;
using cs2_esports.Models;

namespace cs2_esports.IntegrationTests;

public class MatchApiTests : IClassFixture<Cs2ScopeWebApplicationFactory>
{
    private readonly Cs2ScopeWebApplicationFactory _factory;

    public MatchApiTests(Cs2ScopeWebApplicationFactory factory)
    {
        _factory = factory;
        _factory.ResetDatabase();
    }

    [Fact]
    public async Task GetAll_ReturnsOkAndMatchList()
    {
        // Arrange
        SeedMatch();
        using var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/matches");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var matches = await response.Content.ReadFromJsonAsync<List<MatchSummaryDto>>();
        Assert.Single(matches!);
    }

    [Fact]
    public async Task GetById_ReturnsOk_WhenMatchExists()
    {
        // Arrange
        var matchId = SeedMatch();
        using var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync($"/api/matches/{matchId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var match = await response.Content.ReadFromJsonAsync<MatchDetailsDto>();
        Assert.Equal(matchId, match!.Id);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenMatchDoesNotExist()
    {
        // Arrange
        using var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/matches/999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Create_CreatesMatchAndReturnsCreated()
    {
        // Arrange
        var ids = SeedMatchDependencies();
        using var client = _factory.CreateSuperAdminClient();
        var model = ValidMatch(ids.EventId, ids.TeamAId, ids.TeamBId);

        // Act
        var response = await client.PostAsJsonAsync("/api/matches", model);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var createdMatch = await response.Content.ReadFromJsonAsync<MatchDetailsDto>();
        Assert.Equal(model.EventId, createdMatch!.EventId);
        Assert.Equal(1, createdMatch.MapCount);
        Assert.Equal(1, _factory.WithDbContext(db => db.Matches.Count()));
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_ForInvalidModel()
    {
        // Arrange
        var ids = SeedMatchDependencies();
        using var client = _factory.CreateSuperAdminClient();
        var invalidModel = ValidMatch(ids.EventId, ids.TeamAId, ids.TeamAId);

        // Act
        var response = await client.PostAsJsonAsync("/api/matches", invalidModel);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(0, _factory.WithDbContext(db => db.Matches.Count()));
    }

    [Fact]
    public async Task Update_UpdatesExistingMatch()
    {
        // Arrange
        var matchId = SeedMatch();
        var ids = _factory.WithDbContext(db => new MatchIds(
            db.Tournaments.Single().Id,
            db.Teams.OrderBy(team => team.Id).First().Id,
            db.Teams.OrderBy(team => team.Id).Last().Id));
        using var client = _factory.CreateSuperAdminClient();
        var model = ValidMatch(ids.EventId, ids.TeamAId, ids.TeamBId);
        model.ScheduledAtUtc = new DateTime(2026, 9, 2);

        // Act
        var response = await client.PutAsJsonAsync($"/api/matches/{matchId}", model);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var updatedMatch = await response.Content.ReadFromJsonAsync<MatchDetailsDto>();
        Assert.Equal(model.ScheduledAtUtc, updatedMatch!.ScheduledAtUtc);
        Assert.Equal(model.ScheduledAtUtc, _factory.WithDbContext(db => db.Matches.Single().ScheduledAtUtc));
    }

    [Fact]
    public async Task Update_ReturnsNotFound_WhenMatchDoesNotExist()
    {
        // Arrange
        var ids = SeedMatchDependencies();
        using var client = _factory.CreateSuperAdminClient();

        // Act
        var response = await client.PutAsJsonAsync("/api/matches/999", ValidMatch(ids.EventId, ids.TeamAId, ids.TeamBId));

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_DeletesExistingMatch()
    {
        // Arrange
        var matchId = SeedMatch();
        using var client = _factory.CreateSuperAdminClient();

        // Act
        var response = await client.DeleteAsync($"/api/matches/{matchId}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.False(_factory.WithDbContext(db => db.Matches.Any()));
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenMatchDoesNotExist()
    {
        // Arrange
        using var client = _factory.CreateSuperAdminClient();

        // Act
        var response = await client.DeleteAsync("/api/matches/999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private int SeedMatch()
    {
        return _factory.WithDbContext(db =>
        {
            var dependencies = NewMatchDependencies();
            var match = new Match
            {
                ScheduledAtUtc = new DateTime(2026, 9, 1),
                Format = MatchFormat.BestOf1,
                Event = dependencies.Event,
                TeamA = dependencies.TeamA,
                TeamB = dependencies.TeamB
            };

            db.Matches.Add(match);
            db.SaveChanges();
            return match.Id;
        });
    }

    private MatchIds SeedMatchDependencies()
    {
        return _factory.WithDbContext(db =>
        {
            var dependencies = NewMatchDependencies();
            db.Tournaments.Add(dependencies.Event);
            db.Teams.AddRange(dependencies.TeamA, dependencies.TeamB);
            db.SaveChanges();
            return new MatchIds(dependencies.Event.Id, dependencies.TeamA.Id, dependencies.TeamB.Id);
        });
    }

    private static MatchDependencies NewMatchDependencies()
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

        return new MatchDependencies(
            eventItem,
            NewTeam("Team A", "TMA", 1),
            NewTeam("Team B", "TMB", 2));
    }

    private static Team NewTeam(string name, string tag, int ranking)
    {
        return new Team
        {
            Name = name,
            Tag = tag,
            CountryCode = "HR",
            WorldRanking = ranking,
            FoundedYear = 2020,
            PrizeMoneyUsd = 1000,
            LastRosterUpdateUtc = DateTime.UtcNow
        };
    }

    private static MatchUpsertDto ValidMatch(int eventId, int teamAId, int teamBId)
    {
        return new MatchUpsertDto
        {
            ScheduledAtUtc = new DateTime(2026, 9, 1),
            Format = MatchFormat.BestOf1,
            EventId = eventId,
            TeamAId = teamAId,
            TeamBId = teamBId,
            Maps =
            [
                new MatchMapDto
                {
                    MapSequence = 1,
                    Map = MapPool.Mirage,
                    TeamAScore = 0,
                    TeamBScore = 0
                }
            ]
        };
    }

    private sealed record MatchIds(int EventId, int TeamAId, int TeamBId);
    private sealed record MatchDependencies(Event Event, Team TeamA, Team TeamB);
}
