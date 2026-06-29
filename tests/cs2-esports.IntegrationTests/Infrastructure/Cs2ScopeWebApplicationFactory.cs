using cs2_esports.Data;
using cs2_esports.Helpers;
using cs2_esports.Dtos.Events;
using cs2_esports.Models;
using cs2_esports.Services.Ai;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Configuration;

namespace cs2_esports.IntegrationTests.Infrastructure;

public class Cs2ScopeWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName = $"cs2-scope-tests-{Guid.NewGuid()}";
    public string AuditLogDirectory { get; } = Path.Combine(Path.GetTempPath(), $"cs2-scope-audit-tests-{Guid.NewGuid()}");

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureAppConfiguration((_, configuration) =>
            configuration.AddInMemoryCollection(new Dictionary<string, string?>
            {
                [$"AuditLogging:Directory"] = AuditLogDirectory
            }));

        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<DbContextOptions<Cs2ScopeDbContext>>();
            services.RemoveAll<IDbContextOptionsConfiguration<Cs2ScopeDbContext>>();
            services.AddDbContext<Cs2ScopeDbContext>(options =>
                options.UseInMemoryDatabase(_databaseName));

            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = TestAuthHandler.SchemeName;
                    options.DefaultChallengeScheme = TestAuthHandler.SchemeName;
                    options.DefaultForbidScheme = TestAuthHandler.SchemeName;
                })
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                    TestAuthHandler.SchemeName,
                    _ => { });

            services.RemoveAll<IAiEventDraftProvider>();
            services.AddSingleton<IAiEventDraftProvider, TestAiEventDraftProvider>();
        });
    }

    public HttpClient CreateSuperAdminClient()
    {
        return CreateAuthenticatedClient(EventRoleHelper.SuperAdminRole);
    }

    public HttpClient CreateEslAdminClient()
    {
        return CreateAuthenticatedClient(EventRoleHelper.EslAdminRole);
    }

    public HttpClient CreateBlastAdminClient()
    {
        return CreateAuthenticatedClient(EventRoleHelper.BlastAdminRole);
    }

    public HttpClient CreateAuthenticatedClient(string role)
    {
        var client = CreateClient();
        client.DefaultRequestHeaders.Add(TestAuthHandler.UserHeader, "integration-test-user");
        client.DefaultRequestHeaders.Add(TestAuthHandler.RoleHeader, role);
        return client;
    }

    public void ResetDatabase()
    {
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<Cs2ScopeDbContext>();
        dbContext.Database.EnsureDeleted();
        dbContext.Database.EnsureCreated();

        if (Directory.Exists(AuditLogDirectory))
        {
            foreach (var file in Directory.EnumerateFiles(AuditLogDirectory, "audit-*.jsonl"))
            {
                File.Delete(file);
            }
        }
    }

    public T WithDbContext<T>(Func<Cs2ScopeDbContext, T> action)
    {
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<Cs2ScopeDbContext>();
        return action(dbContext);
    }
}

internal sealed class TestAiEventDraftProvider : IAiEventDraftProvider
{
    public Task<AiEventDraft> CreateDraftAsync(
        string prompt,
        AiEventDraftContext context,
        CancellationToken cancellationToken = default)
    {
        var organizer = prompt.Contains("BLAST", StringComparison.OrdinalIgnoreCase)
            ? "BLAST"
            : prompt.Contains("ESL", StringComparison.OrdinalIgnoreCase)
                ? "ESL"
                : context.RequiredOrganizer;
        var start = new DateTime(2026, 7, 15, 0, 0, 0, DateTimeKind.Utc);

        return Task.FromResult(new AiEventDraft
        {
            Name = "IEM Zagreb",
            Organizer = organizer,
            PrizePoolUsd = 250000,
            StartDateUtc = start,
            EndDateUtc = prompt.Contains("invalid dates", StringComparison.OrdinalIgnoreCase)
                ? start.AddDays(-1)
                : null,
            SelectedTeamIds = prompt.Contains("top 8", StringComparison.OrdinalIgnoreCase)
                ? context.Teams?
                    .Where(team => team.WorldRanking > 0)
                    .OrderBy(team => team.WorldRanking)
                    .Take(8)
                    .Select(team => team.Id)
                    .ToList()
                : null
        });
    }
}
