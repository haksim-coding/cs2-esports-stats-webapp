using cs2_esports.Helpers;
using cs2_esports.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace cs2_esports.Data;

public static class IdentitySeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();
        var dbContext = serviceProvider.GetRequiredService<Cs2ScopeDbContext>();

        var roles = new[]
        {
            EventRoleHelper.SuperAdminRole,
            EventRoleHelper.BlastAdminRole,
            EventRoleHelper.EslAdminRole,
            EventRoleHelper.TournamentAdminRole
        };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        var adminUsers = await dbContext.AdminUsers.ToListAsync();

        foreach (var adminUser in adminUsers)
        {
            var expectedPermissionGroup = EventRoleHelper.GetPrimaryRoleForAdminUser(adminUser.Username, adminUser.PermissionGroup);
            if (!string.Equals(adminUser.PermissionGroup, expectedPermissionGroup, StringComparison.Ordinal))
            {
                adminUser.PermissionGroup = expectedPermissionGroup;
            }
        }

        await dbContext.SaveChangesAsync();

        foreach (var adminUser in adminUsers)
        {
            var identityUser = await userManager.FindByNameAsync(adminUser.Username)
                ?? await userManager.FindByEmailAsync(adminUser.Email);

            if (identityUser is null)
            {
                identityUser = new AppUser
                {
                    UserName = adminUser.Username,
                    Email = adminUser.Email,
                    DisplayName = adminUser.DisplayName,
                    Bio = adminUser.Bio,
                    CountryCode = adminUser.CountryCode,
                    RegisteredAtUtc = adminUser.RegisteredAtUtc,
                    IsSuspended = adminUser.IsSuspended,
                    HiredAtUtc = adminUser.HiredAtUtc,
                    LastModerationActionAtUtc = adminUser.LastModerationActionAtUtc,
                    LegacyAdminUserId = adminUser.Id,
                    EmailConfirmed = true
                };

                var createResult = await userManager.CreateAsync(identityUser, adminUser.Password);
                if (!createResult.Succeeded)
                {
                    throw new InvalidOperationException($"Failed to seed Identity user '{adminUser.Username}': {string.Join(", ", createResult.Errors.Select(error => error.Description))}");
                }
            }
            else
            {
                identityUser.DisplayName = adminUser.DisplayName;
                identityUser.Bio = adminUser.Bio;
                identityUser.CountryCode = adminUser.CountryCode;
                identityUser.RegisteredAtUtc = adminUser.RegisteredAtUtc;
                identityUser.IsSuspended = adminUser.IsSuspended;
                identityUser.HiredAtUtc = adminUser.HiredAtUtc;
                identityUser.LastModerationActionAtUtc = adminUser.LastModerationActionAtUtc;
                identityUser.LegacyAdminUserId = adminUser.Id;

                await userManager.UpdateAsync(identityUser);
            }

            var rolesForUser = EventRoleHelper.GetRolesForAdminUser(adminUser.Username, adminUser.PermissionGroup);

            var currentRoles = await userManager.GetRolesAsync(identityUser);
            var rolesToRemove = currentRoles.Where(role => !rolesForUser.Contains(role)).ToArray();
            if (rolesToRemove.Length > 0)
            {
                await userManager.RemoveFromRolesAsync(identityUser, rolesToRemove);
            }

            foreach (var role in rolesForUser)
            {
                if (!await userManager.IsInRoleAsync(identityUser, role))
                {
                    await userManager.AddToRoleAsync(identityUser, role);
                }
            }
        }
    }
}
