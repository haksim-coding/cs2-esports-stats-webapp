using cs2_esports.Data;
using cs2_esports.Helpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace cs2_esports.IntegrationTests.Infrastructure;

public class Cs2ScopeWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _databaseName = $"cs2-scope-tests-{Guid.NewGuid()}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

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
    }

    public T WithDbContext<T>(Func<Cs2ScopeDbContext, T> action)
    {
        using var scope = Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<Cs2ScopeDbContext>();
        return action(dbContext);
    }
}
