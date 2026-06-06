using cs2_esports.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

var config = new ConfigurationBuilder()
    .SetBasePath(Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..")))
    .AddJsonFile("appsettings.json")
    .Build();
var options = new DbContextOptionsBuilder<Cs2ScopeDbContext>()
    .UseSqlServer(config.GetConnectionString("Cs2ScopeDbContext"))
    .Options;
using var db = new Cs2ScopeDbContext(options);
foreach (var team in db.Teams.AsNoTracking().OrderBy(t => t.WorldRanking).ThenBy(t => t.Name))
{
    Console.WriteLine($"{team.Id}\t#{team.WorldRanking}\t{team.Name}\t{team.Tag}\t{team.CountryCode}");
}
