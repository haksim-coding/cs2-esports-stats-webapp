using cs2_esports.Models;
using cs2_esports.Repositories.Interfaces;
using cs2_esports.Repositories.Mock;
using Microsoft.AspNetCore.Localization;
using System.Globalization;

CultureInfo.DefaultThreadCurrentCulture = CultureInfo.GetCultureInfo("en-US");
CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo("en-US");

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();

builder.Services.AddSingleton<InMemoryAppData>(_ => SampleDataSeeder.Create());
builder.Services.AddSingleton<ITeamRepository, TeamMockRepository>();
builder.Services.AddSingleton<IEventRepository, EventMockRepository>();
builder.Services.AddSingleton<IPlayerRepository, PlayerMockRepository>();
builder.Services.AddSingleton<IForumRepository, ForumMockRepository>();

var app = builder.Build();

var seededData = app.Services.GetRequiredService<InMemoryAppData>();

app.Logger.LogInformation(
    "Seeded {TournamentCount} tournaments, {TeamCount} teams and {PlayerCount} players.",
    seededData.Tournaments.Count,
    seededData.Teams.Count,
    seededData.Players.Count);

var linqReport = SampleLinqQueries.BuildReport(seededData);
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
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("en-US"),
    SupportedCultures = new[] { CultureInfo.GetCultureInfo("en-US") },
    SupportedUICultures = new[] { CultureInfo.GetCultureInfo("en-US") }
});
app.UseSession();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
