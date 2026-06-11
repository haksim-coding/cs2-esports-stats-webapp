using System.Net;
using System.Net.Http.Json;
using cs2_esports.Dtos.Events;
using cs2_esports.IntegrationTests.Infrastructure;
using cs2_esports.Models;
using Microsoft.EntityFrameworkCore;

namespace cs2_esports.IntegrationTests;

public class EventApiTests : IClassFixture<Cs2ScopeWebApplicationFactory>
{
    private readonly Cs2ScopeWebApplicationFactory _factory;

    public EventApiTests(Cs2ScopeWebApplicationFactory factory)
    {
        _factory = factory;
        _factory.ResetDatabase();
    }

    [Fact]
    public async Task GetAll_ReturnsOkAndEventList()
    {
        // Arrange
        SeedEvent();
        using var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/events");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var events = await response.Content.ReadFromJsonAsync<List<EventSummaryDto>>();
        Assert.Single(events!);
    }

    [Fact]
    public async Task GetById_ReturnsOk_WhenEventExists()
    {
        // Arrange
        var eventId = SeedEvent();
        using var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync($"/api/events/{eventId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var eventItem = await response.Content.ReadFromJsonAsync<EventDetailsDto>();
        Assert.Equal(eventId, eventItem!.Id);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenEventDoesNotExist()
    {
        // Arrange
        using var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/events/999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Create_CreatesEventAndReturnsCreated()
    {
        // Arrange
        var venueId = SeedVenue();
        using var client = _factory.CreateSuperAdminClient();
        var model = ValidEvent(venueId);

        // Act
        var response = await client.PostAsJsonAsync("/api/events", model);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var createdEvent = await response.Content.ReadFromJsonAsync<EventDetailsDto>();
        Assert.Equal(model.Name, createdEvent!.Name);
        Assert.Equal(1, _factory.WithDbContext(db => db.Tournaments.Count()));
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_ForInvalidModel()
    {
        // Arrange
        var venueId = SeedVenue();
        using var client = _factory.CreateSuperAdminClient();
        var invalidModel = ValidEvent(venueId);
        invalidModel.Name = "A";

        // Act
        var response = await client.PostAsJsonAsync("/api/events", invalidModel);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(0, _factory.WithDbContext(db => db.Tournaments.Count()));
    }

    [Fact]
    public async Task Update_UpdatesExistingEvent()
    {
        // Arrange
        var eventId = SeedEvent();
        var venueId = _factory.WithDbContext(db => db.EventVenues.Single().Id);
        using var client = _factory.CreateSuperAdminClient();
        var model = ValidEvent(venueId);
        model.Name = "Updated Event";

        // Act
        var response = await client.PutAsJsonAsync($"/api/events/{eventId}", model);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var updatedEvent = await response.Content.ReadFromJsonAsync<EventDetailsDto>();
        Assert.Equal("Updated Event", updatedEvent!.Name);
        Assert.Equal("Updated Event", _factory.WithDbContext(db => db.Tournaments.Single().Name));
    }

    [Fact]
    public async Task Update_ReturnsNotFound_WhenEventDoesNotExist()
    {
        // Arrange
        var venueId = SeedVenue();
        using var client = _factory.CreateSuperAdminClient();

        // Act
        var response = await client.PutAsJsonAsync("/api/events/999", ValidEvent(venueId));

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_DeletesExistingEvent()
    {
        // Arrange
        var eventId = SeedEvent();
        using var client = _factory.CreateSuperAdminClient();

        // Act
        var response = await client.DeleteAsync($"/api/events/{eventId}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.False(_factory.WithDbContext(db => db.Tournaments.Any()));
    }

    [Fact]
    public async Task Delete_DeletesMatchesAndDisconnectsTeams()
    {
        // Arrange
        var eventId = SeedEventWithTeamsAndMatch();
        using var client = _factory.CreateSuperAdminClient();

        // Act
        var response = await client.DeleteAsync($"/api/events/{eventId}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        _factory.WithDbContext(db =>
        {
            Assert.False(db.Tournaments.Any());
            Assert.False(db.Matches.Any());
            Assert.Equal(2, db.Teams.Count());
            Assert.All(db.Teams.Include(team => team.Tournaments), team => Assert.Empty(team.Tournaments));
            return true;
        });
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenEventDoesNotExist()
    {
        // Arrange
        using var client = _factory.CreateSuperAdminClient();

        // Act
        var response = await client.DeleteAsync("/api/events/999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Create_ReturnsUnauthorized_ForAnonymousUser()
    {
        // Arrange
        var venueId = SeedVenue();
        using var client = _factory.CreateClient();

        // Act
        var response = await client.PostAsJsonAsync("/api/events", ValidEvent(venueId));

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal(0, _factory.WithDbContext(db => db.Tournaments.Count()));
    }

    [Fact]
    public async Task Update_ReturnsUnauthorized_ForAnonymousUser()
    {
        // Arrange
        var eventId = SeedEvent();
        var venueId = _factory.WithDbContext(db => db.EventVenues.Single().Id);
        using var client = _factory.CreateClient();

        // Act
        var response = await client.PutAsJsonAsync($"/api/events/{eventId}", ValidEvent(venueId));

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal("Test Event", _factory.WithDbContext(db => db.Tournaments.Single().Name));
    }

    [Fact]
    public async Task Delete_ReturnsUnauthorized_ForAnonymousUser()
    {
        // Arrange
        var eventId = SeedEvent();
        using var client = _factory.CreateClient();

        // Act
        var response = await client.DeleteAsync($"/api/events/{eventId}");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.True(_factory.WithDbContext(db => db.Tournaments.Any()));
    }

    [Fact]
    public async Task Create_CreatesEslEvent_ForEslAdmin()
    {
        // Arrange
        var venueId = SeedVenue();
        using var client = _factory.CreateEslAdminClient();
        var model = ValidEvent(venueId);
        model.Organizer = "ESL";

        // Act
        var response = await client.PostAsJsonAsync("/api/events", model);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var createdEvent = await response.Content.ReadFromJsonAsync<EventDetailsDto>();
        Assert.Equal("ESL", createdEvent!.Organizer);
        Assert.Equal("ESL", _factory.WithDbContext(db => db.Tournaments.Single().Organizer));
    }

    [Fact]
    public async Task Create_ReturnsForbidden_WhenEslAdminCreatesBlastEvent()
    {
        // Arrange
        var venueId = SeedVenue();
        using var client = _factory.CreateEslAdminClient();
        var model = ValidEvent(venueId);
        model.Organizer = "BLAST";

        // Act
        var response = await client.PostAsJsonAsync("/api/events", model);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal(0, _factory.WithDbContext(db => db.Tournaments.Count()));
    }

    [Fact]
    public async Task Update_UpdatesBlastEvent_ForBlastAdmin()
    {
        // Arrange
        var eventId = SeedEvent("BLAST");
        var venueId = _factory.WithDbContext(db => db.EventVenues.Single().Id);
        using var client = _factory.CreateBlastAdminClient();
        var model = ValidEvent(venueId);
        model.Name = "Updated BLAST Event";
        model.Organizer = "BLAST";

        // Act
        var response = await client.PutAsJsonAsync($"/api/events/{eventId}", model);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Updated BLAST Event", _factory.WithDbContext(db => db.Tournaments.Single().Name));
    }

    [Fact]
    public async Task Update_ReturnsForbidden_WhenBlastAdminUpdatesEslEvent()
    {
        // Arrange
        var eventId = SeedEvent("ESL");
        var venueId = _factory.WithDbContext(db => db.EventVenues.Single().Id);
        using var client = _factory.CreateBlastAdminClient();
        var model = ValidEvent(venueId);
        model.Organizer = "ESL";

        // Act
        var response = await client.PutAsJsonAsync($"/api/events/{eventId}", model);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.Equal("Test Event", _factory.WithDbContext(db => db.Tournaments.Single().Name));
    }

    [Fact]
    public async Task Delete_DeletesEslEvent_ForEslAdmin()
    {
        // Arrange
        var eventId = SeedEvent("ESL");
        using var client = _factory.CreateEslAdminClient();

        // Act
        var response = await client.DeleteAsync($"/api/events/{eventId}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.False(_factory.WithDbContext(db => db.Tournaments.Any()));
    }

    [Fact]
    public async Task Delete_ReturnsForbidden_WhenEslAdminDeletesBlastEvent()
    {
        // Arrange
        var eventId = SeedEvent("BLAST");
        using var client = _factory.CreateEslAdminClient();

        // Act
        var response = await client.DeleteAsync($"/api/events/{eventId}");

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        Assert.True(_factory.WithDbContext(db => db.Tournaments.Any()));
    }

    private int SeedEvent()
    {
        return SeedEvent("Test Organizer");
    }

    private int SeedEvent(string organizer)
    {
        return _factory.WithDbContext(db =>
        {
            var venue = NewVenue();
            var eventItem = new Event
            {
                Name = "Test Event",
                Organizer = organizer,
                Tier = EventTier.A,
                PrizePoolUsd = 100000,
                StartDateUtc = new DateTime(2026, 7, 1),
                EndDateUtc = new DateTime(2026, 7, 5),
                IsLan = true,
                EventVenue = venue
            };

            db.Tournaments.Add(eventItem);
            db.SaveChanges();
            return eventItem.Id;
        });
    }

    private int SeedVenue()
    {
        return _factory.WithDbContext(db =>
        {
            var venue = NewVenue();
            db.EventVenues.Add(venue);
            db.SaveChanges();
            return venue.Id;
        });
    }

    private int SeedEventWithTeamsAndMatch()
    {
        return _factory.WithDbContext(db =>
        {
            var teamA = NewTeam("Alpha", "ALP", 1);
            var teamB = NewTeam("Bravo", "BRV", 2);
            var eventItem = new Event
            {
                Name = "Test Event",
                Organizer = "Test Organizer",
                Tier = EventTier.A,
                PrizePoolUsd = 100000,
                StartDateUtc = new DateTime(2026, 7, 1),
                EndDateUtc = new DateTime(2026, 7, 5),
                IsLan = true,
                EventVenue = NewVenue(),
                Teams = [teamA, teamB]
            };

            eventItem.Matches.Add(new Match
            {
                ScheduledAtUtc = new DateTime(2026, 7, 2),
                Format = MatchFormat.BestOf3,
                TeamA = teamA,
                TeamB = teamB
            });

            db.Tournaments.Add(eventItem);
            db.SaveChanges();
            return eventItem.Id;
        });
    }

    private static Team NewTeam(string name, string tag, int ranking)
    {
        return new Team
        {
            Name = name,
            Tag = tag,
            CountryCode = "HR",
            WorldRanking = ranking,
            FoundedYear = 2020
        };
    }

    private static EventVenue NewVenue()
    {
        return new EventVenue
        {
            Name = "Test Arena",
            City = "Zagreb",
            CountryCode = "HR",
            Capacity = 10000,
            IsIndoor = true,
            SurfaceType = "Stage"
        };
    }

    private static EventUpsertDto ValidEvent(int venueId)
    {
        return new EventUpsertDto
        {
            Name = "New Event",
            Organizer = "Test Organizer",
            Tier = EventTier.S,
            PrizePoolUsd = 250000,
            StartDateUtc = new DateTime(2026, 8, 1),
            EndDateUtc = new DateTime(2026, 8, 5),
            IsLan = true,
            EventVenueId = venueId
        };
    }
}
