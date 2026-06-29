using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using cs2_esports.Dtos.Teams;
using cs2_esports.IntegrationTests.Infrastructure;
using cs2_esports.Services.Auditing;

namespace cs2_esports.IntegrationTests;

public class AuditLoggingTests : IClassFixture<Cs2ScopeWebApplicationFactory>
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);
    private readonly Cs2ScopeWebApplicationFactory _factory;

    public AuditLoggingTests(Cs2ScopeWebApplicationFactory factory)
    {
        _factory = factory;
        _factory.ResetDatabase();
    }

    [Fact]
    public async Task SuccessfulEntityMutation_WritesStructuredAuditEntry()
    {
        using var client = _factory.CreateSuperAdminClient();

        var response = await client.PostAsJsonAsync("/api/team", new TeamUpsertDto
        {
            Name = "Audit Team",
            Tag = "AUD",
            CountryCode = "HR",
            FoundedYear = 2026,
            PrizeMoneyUsd = 1000
        });

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var entry = Assert.Single(ReadEntries());
        Assert.Equal("Succeeded", entry.Outcome);
        Assert.Equal("POST", entry.HttpMethod);
        Assert.Equal("Teams", entry.Entity);
        Assert.Equal("Create", entry.Action);
        Assert.Equal("integration-test-user", entry.ActorName);
        Assert.Equal("SuperAdmin", Assert.Single(entry.ActorRoles));
        Assert.Equal(201, entry.StatusCode);
        Assert.Equal("/api/team", entry.Path);
        Assert.False(string.IsNullOrWhiteSpace(entry.EntityId));
        Assert.False(string.IsNullOrWhiteSpace(entry.TraceId));
    }

    [Theory]
    [InlineData("/api/team", "Teams")]
    [InlineData("/api/players", "Players")]
    [InlineData("/api/events", "Events")]
    [InlineData("/api/matches", "Matches")]
    public async Task RejectedMutations_AreLoggedForEveryApiEntity(string path, string entity)
    {
        using var client = _factory.CreateSuperAdminClient();

        var response = await client.PostAsJsonAsync(path, new { });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var entry = Assert.Single(ReadEntries());
        Assert.Equal("Rejected", entry.Outcome);
        Assert.Equal(entity, entry.Entity);
        Assert.Equal("Create", entry.Action);
        Assert.Equal(400, entry.StatusCode);
    }

    [Fact]
    public async Task AdminFiles_ListsAndSafelyOpensAuditLog()
    {
        using var client = _factory.CreateSuperAdminClient();
        await client.PostAsJsonAsync("/api/team", new { });
        var logFile = Assert.Single(Directory.GetFiles(_factory.AuditLogDirectory, "audit-*.jsonl"));
        var fileName = Path.GetFileName(logFile);

        var indexResponse = await client.GetAsync("/admin/files");
        var indexHtml = await indexResponse.Content.ReadAsStringAsync();
        var logResponse = await client.GetAsync($"/admin/files/logs/{fileName}");
        var traversalResponse = await client.GetAsync("/admin/files/logs/audit-..%2Fappsettings.json.jsonl");

        Assert.Equal(HttpStatusCode.OK, indexResponse.StatusCode);
        Assert.Contains("Audit logs", indexHtml);
        Assert.Contains(fileName, indexHtml);
        Assert.Equal(HttpStatusCode.OK, logResponse.StatusCode);
        Assert.Equal("application/x-ndjson", logResponse.Content.Headers.ContentType?.MediaType);
        Assert.Equal(HttpStatusCode.NotFound, traversalResponse.StatusCode);
    }

    private List<AuditLogEntry> ReadEntries()
    {
        return Directory.GetFiles(_factory.AuditLogDirectory, "audit-*.jsonl")
            .SelectMany(File.ReadLines)
            .Select(line => JsonSerializer.Deserialize<AuditLogEntry>(line, SerializerOptions))
            .Where(entry => entry is not null)
            .Cast<AuditLogEntry>()
            .ToList();
    }
}
