var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var seededData = cs2_esports.Models.SampleDataSeeder.Create();
builder.Services.AddSingleton(seededData);

var app = builder.Build();

app.Logger.LogInformation(
    "Seeded {TournamentCount} tournaments, {TeamCount} teams and {PlayerCount} players.",
    seededData.Tournaments.Count,
    seededData.Teams.Count,
    seededData.Players.Count);

var linqReport = cs2_esports.Models.SampleLinqQueries.BuildReport(seededData);
foreach (var line in linqReport)
{
    app.Logger.LogInformation(line);
}
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
