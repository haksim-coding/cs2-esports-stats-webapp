using System.Net;
using System.Net.Http.Json;
using cs2_esports.Dtos.Players;
using cs2_esports.IntegrationTests.Infrastructure;
using cs2_esports.Models;

namespace cs2_esports.IntegrationTests;

public class PlayerApiTests : IClassFixture<Cs2ScopeWebApplicationFactory>
{
    private readonly Cs2ScopeWebApplicationFactory _factory;

    public PlayerApiTests(Cs2ScopeWebApplicationFactory factory)
    {
        _factory = factory;
        _factory.ResetDatabase();
    }

    [Fact]
    public async Task GetAll_ReturnsOkAndPlayerList()
    {
        // Arrange
        SeedPlayer();
        using var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/players");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var players = await response.Content.ReadFromJsonAsync<List<PlayerSummaryDto>>();
        Assert.Single(players!);
    }

    [Fact]
    public async Task GetById_ReturnsOk_WhenPlayerExists()
    {
        // Arrange
        var playerId = SeedPlayer();
        using var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync($"/api/players/{playerId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var player = await response.Content.ReadFromJsonAsync<PlayerDetailsDto>();
        Assert.Equal(playerId, player!.Id);
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenPlayerDoesNotExist()
    {
        // Arrange
        using var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/players/999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Create_CreatesPlayerAndReturnsCreated()
    {
        // Arrange
        using var client = _factory.CreateSuperAdminClient();
        var model = ValidPlayer();

        // Act
        var response = await client.PostAsJsonAsync("/api/players", model);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var createdPlayer = await response.Content.ReadFromJsonAsync<PlayerDetailsDto>();
        Assert.Equal(model.Nickname, createdPlayer!.Nickname);
        Assert.Equal(1, _factory.WithDbContext(db => db.Players.Count()));
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_ForInvalidModel()
    {
        // Arrange
        using var client = _factory.CreateSuperAdminClient();
        var invalidModel = ValidPlayer();
        invalidModel.Nickname = "A";

        // Act
        var response = await client.PostAsJsonAsync("/api/players", invalidModel);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal(0, _factory.WithDbContext(db => db.Players.Count()));
    }

    [Fact]
    public async Task Update_UpdatesExistingPlayer()
    {
        // Arrange
        var playerId = SeedPlayer();
        using var client = _factory.CreateSuperAdminClient();
        var model = ValidPlayer();
        model.Nickname = "updated-player";

        // Act
        var response = await client.PutAsJsonAsync($"/api/players/{playerId}", model);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var updatedPlayer = await response.Content.ReadFromJsonAsync<PlayerDetailsDto>();
        Assert.Equal("updated-player", updatedPlayer!.Nickname);
        Assert.Equal("updated-player", _factory.WithDbContext(db => db.Players.Single().Nickname));
    }

    [Fact]
    public async Task Update_ReturnsNotFound_WhenPlayerDoesNotExist()
    {
        // Arrange
        using var client = _factory.CreateSuperAdminClient();

        // Act
        var response = await client.PutAsJsonAsync("/api/players/999", ValidPlayer());

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_DeletesExistingPlayer()
    {
        // Arrange
        var playerId = SeedPlayer();
        using var client = _factory.CreateSuperAdminClient();

        // Act
        var response = await client.DeleteAsync($"/api/players/{playerId}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        Assert.False(_factory.WithDbContext(db => db.Players.Any()));
    }

    [Fact]
    public async Task Delete_ReturnsNotFound_WhenPlayerDoesNotExist()
    {
        // Arrange
        using var client = _factory.CreateSuperAdminClient();

        // Act
        var response = await client.DeleteAsync("/api/players/999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    private int SeedPlayer()
    {
        return _factory.WithDbContext(db =>
        {
            var player = new Player
            {
                Nickname = "test-player",
                FullName = "Test Player",
                CountryCode = "HR",
                DateOfBirth = new DateTime(2000, 1, 1),
                Role = PlayerRole.Rifler,
                Rating2 = 1.10m,
                TotalMapsPlayed = 100,
                JoinedTeamAtUtc = DateTime.UtcNow
            };

            db.Players.Add(player);
            db.SaveChanges();
            return player.Id;
        });
    }

    private static PlayerUpsertDto ValidPlayer()
    {
        return new PlayerUpsertDto
        {
            Nickname = "new-player",
            FullName = "New Player",
            CountryCode = "HR",
            DateOfBirth = new DateTime(2001, 1, 1),
            Role = PlayerRole.Awper,
            Rating2 = 1.20m,
            TotalMapsPlayed = 50
        };
    }
}
