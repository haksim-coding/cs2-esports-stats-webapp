using cs2_esports.Data;
using cs2_esports.Helpers;
using cs2_esports.Models;
using cs2_esports.Repositories.Interfaces;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
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
builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
})
    .AddEntityFrameworkStores<Cs2ScopeDbContext>()
    .AddDefaultTokenProviders();
builder.Services.AddDbContext<Cs2ScopeDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Cs2ScopeDbContext")));

builder.Services.AddScoped<ITeamRepository, EfTeamRepository>();
builder.Services.AddScoped<IEventRepository, EfEventRepository>();
builder.Services.AddScoped<IMatchRepository, EfMatchRepository>();
builder.Services.AddScoped<IPlayerRepository, EfPlayerRepository>();
builder.Services.AddScoped<IForumRepository, EfForumRepository>();

var app = builder.Build();

if (!app.Environment.IsEnvironment("Testing"))
{
    using var scope = app.Services.CreateScope();
    await IdentitySynchronizer.SynchronizeAsync(scope.ServiceProvider);
}
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("en-US"),
    SupportedCultures = new[] { CultureInfo.GetCultureInfo("en-US") },
    SupportedUICultures = new[] { CultureInfo.GetCultureInfo("en-US") }
});
app.UseSession();

app.UseAuthentication();
if (!app.Environment.IsEnvironment("Testing"))
{
    app.Use(async (context, next) =>
    {
        var dbContext = context.RequestServices.GetRequiredService<Cs2ScopeDbContext>();
        var hasAdminSession = string.Equals(context.Session.GetString(AuthSessionKeys.UserType), AuthSessionKeys.AdminUserType, StringComparison.Ordinal) &&
            context.Session.GetInt32(AuthSessionKeys.AdminUserId).HasValue;

        if (EventRoleHelper.IsEventAdmin(context.User) && !hasAdminSession)
        {
            await context.SignOutAsync(IdentityConstants.ApplicationScheme);
            context.User = new System.Security.Claims.ClaimsPrincipal(new System.Security.Claims.ClaimsIdentity());
        }

        if (string.Equals(context.Session.GetString(AuthSessionKeys.UserType), AuthSessionKeys.AdminUserType, StringComparison.Ordinal) &&
            !EventRoleHelper.IsEventAdmin(context.User))
        {
            context.Session.Remove(AuthSessionKeys.AdminUserId);
            context.Session.Remove(AuthSessionKeys.UserType);
        }

        var identityUserId = context.User.Identity?.IsAuthenticated == true
            ? context.User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
            : null;
        var linkedForumUserId = !string.IsNullOrWhiteSpace(identityUserId)
            ? await dbContext.Users
                .Where(user => user.Id == identityUserId && user.LegacyAdminUserId == null)
                .Select(user => user.LegacyForumUserId)
                .FirstOrDefaultAsync()
            : null;

        if (linkedForumUserId.HasValue)
        {
            context.Session.Remove(AuthSessionKeys.AdminUserId);
            context.Session.SetInt32(AuthSessionKeys.ForumUserId, linkedForumUserId.Value);
            context.Session.SetString(AuthSessionKeys.UserType, AuthSessionKeys.ForumUserType);
        }
        else if (string.Equals(context.Session.GetString(AuthSessionKeys.UserType), AuthSessionKeys.ForumUserType, StringComparison.Ordinal))
        {
            context.Session.Remove(AuthSessionKeys.ForumUserId);
            context.Session.Remove(AuthSessionKeys.UserType);
        }

        await next();
    });
}
app.UseAuthorization();

app.MapStaticAssets();
app.MapControllers();

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

public partial class Program;
