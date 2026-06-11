using System.Net;
using System.Net.Http.Json;
using cs2_esports.Dtos.Teams;
using cs2_esports.IntegrationTests.Infrastructure;
using cs2_esports.Models;

namespace cs2_esports.IntegrationTests;

public class TeamApiTests : IClassFixture<Cs2ScopeWebApplicationFactory>
{
    private readonly Cs2ScopeWebApplicationFactory _factory;

    public TeamApiTests(Cs2ScopeWebApplicationFactory factory)
    {
        _factory = factory;
        _factory.ResetDatabase();
    }

    [Fact]
    public async Task GetAll_ReturnsOkAndTeamList()
    {
        // Arrange
        SeedTeam();
        using var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/team");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var teams = await response.Content.ReadFromJsonAsync<List<TeamListItemDto>>();
        Assert.Single(teams!);
    }

    [Fact]
    public async Task GetById_ReturnsOk_WhenTeamExists()
    {
        // Arrange
        var teamId = SeedTeam();
        using var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync($"/api/team/{teamId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var team = await response.Content.ReadFromJsonAsync<TeamDetailsDto>();
        Assert.Equal(teamId, team!.Id);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenTeamDoesNotExist()
    {
        // Arrange
        using var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/team/999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Create_CreatesTeamAndReturnsCreated()
    {
        // Arrange
        using var client = _factory.CreateSuperAdminClient();
        var model = ValidTeam();

        // Act
        var response = await client.PostAsJsonAsync("/api/team", model);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var createdTeam = await response.Content.ReadFromJsonAsync<TeamDetailsDto>();
        Assert.Equal(model.Name, createdTeam!.Name);
        Assert.Equal(1, _factory.WithDbContext(db => db.Teams.Count()));
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_ForInvalidModel()
    {
        // Arrange
        using var client = _factory.CreateSuperAdminClient();
        var invalidModel = ValidTeam();
        invalidModel.Name = "A";

        // Act
        var response = await client.PostAsJsonAsync("/api/team", invalidModel);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(0, _factory.WithDbContext(db => db.Teams.Count()));
    }

    [Fact]
    public async Task Update_UpdatesExistingTeam()
    {
        // Arrange
        var teamId = SeedTeam();
        using var client = _factory.CreateSuperAdminClient();
        var model = ValidTeam();
        model.Name = "Updated Team";
        model.Tag = "UPD";

        // Act
        var response = await client.PutAsJsonAsync($"/api/team/{teamId}", model);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Updated Team", _factory.WithDbContext(db => db.Teams.Single().Name));
    }

    [Fact]
    public async Task Update_ReturnsNotFound_WhenTeamDoesNotExist()
    {
        // Arrange
        using var client = _factory.CreateSuperAdminClient();

        // Act
        var response = await client.PutAsJsonAsync("/api/team/999", ValidTeam());

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_DeletesExistingTeam()
    {
        // Arrange
        var teamId = SeedTeam();
        using var client = _factory.CreateSuperAdminClient();

        // Act
        var response = await client.DeleteAsync($"/api/team/{teamId}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.False(_factory.WithDbContext(db => db.Teams.Any()));
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenTeamDoesNotExist()
    {
        // Arrange
        using var client = _factory.CreateSuperAdminClient();

        // Act
        var response = await client.DeleteAsync("/api/team/999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private int SeedTeam()
    {
        return _factory.WithDbContext(db =>
        {
            var team = new Team
            {
                Name = "Test Team",
                Tag = "TEST",
                CountryCode = "HR",
                WorldRanking = 1,
                FoundedYear = 2020,
                PrizeMoneyUsd = 1000,
                LastRosterUpdateUtc = DateTime.UtcNow
            };

            db.Teams.Add(team);
            db.SaveChanges();
            return team.Id;
        });
    }

    private static TeamUpsertDto ValidTeam()
    {
        return new TeamUpsertDto
        {
            Name = "New Team",
            Tag = "NEW",
            CountryCode = "HR",
            FoundedYear = 2024,
            PrizeMoneyUsd = 5000
        };
    }
}
