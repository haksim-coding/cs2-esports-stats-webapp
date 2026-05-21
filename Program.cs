using cs2_esports.Data;
using cs2_esports.Repositories.Interfaces;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using cs2_esports.Repositories.Ef;

CultureInfo.DefaultThreadCurrentCulture = CultureInfo.GetCultureInfo("en-US");
CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.GetCultureInfo("en-US");

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();
builder.Services.AddDbContext<Cs2ScopeDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Cs2ScopeDbContext")));

builder.Services.AddScoped<ITeamRepository, EfTeamRepository>();
builder.Services.AddScoped<IEventRepository, EfEventRepository>();
builder.Services.AddScoped<IMatchRepository, EfMatchRepository>();
builder.Services.AddScoped<IPlayerRepository, EfPlayerRepository>();
builder.Services.AddScoped<IForumRepository, EfForumRepository>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<Cs2ScopeDbContext>();
    app.Logger.LogInformation(
        "Database seeded with {TeamCount} teams, {EventCount} events, {MatchCount} matches, {ForumCount} forums and {PlayerCount} players.",
        await dbContext.Teams.CountAsync(),
        await dbContext.Tournaments.CountAsync(),
        await dbContext.Matches.CountAsync(),
        await dbContext.Forums.CountAsync(),
        await dbContext.Players.CountAsync());
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
    name: "login",
    pattern: "login",
    defaults: new { controller = "Auth", action = "Login" });

app.MapControllerRoute(
    name: "register",
    pattern: "register",
    defaults: new { controller = "Auth", action = "Register" });

app.MapControllerRoute(
    name: "logout",
    pattern: "logout",
    defaults: new { controller = "Auth", action = "Logout" });

app.MapControllerRoute(
    name: "profile",
    pattern: "my-profile",
    defaults: new { controller = "Auth", action = "Profile" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
