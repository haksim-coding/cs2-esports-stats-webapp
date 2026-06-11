using cs2_esports.Helpers;
using cs2_esports.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace cs2_esports.Data;

public static class IdentitySynchronizer
{
    public static async Task SynchronizeAsync(IServiceProvider serviceProvider)
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
                    throw new InvalidOperationException($"Failed to create Identity user '{adminUser.Username}': {string.Join(", ", createResult.Errors.Select(error => error.Description))}");
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

        var forumUsers = await dbContext.ForumUsers.ToListAsync();
        foreach (var forumUser in forumUsers)
        {
            var identityUser = await userManager.FindByNameAsync(forumUser.Username)
                ?? await userManager.FindByEmailAsync(forumUser.Email);

            if (identityUser is null)
            {
                identityUser = new AppUser
                {
                    UserName = forumUser.Username,
                    Email = forumUser.Email,
                    DisplayName = forumUser.DisplayName,
                    Bio = forumUser.Bio,
                    CountryCode = forumUser.CountryCode,
                    RegisteredAtUtc = forumUser.RegisteredAtUtc,
                    IsSuspended = forumUser.IsSuspended,
                    LegacyForumUserId = forumUser.Id,
                    EmailConfirmed = true
                };

                var createResult = await userManager.CreateAsync(identityUser, forumUser.Password);
                if (!createResult.Succeeded)
                {
                    throw new InvalidOperationException($"Failed to migrate forum user '{forumUser.Username}' to Identity: {string.Join(", ", createResult.Errors.Select(error => error.Description))}");
                }
            }
            else
            {
                if (identityUser.LegacyAdminUserId.HasValue)
                {
                    throw new InvalidOperationException($"Cannot migrate forum user '{forumUser.Username}' because the matching Identity account belongs to an administrator.");
                }

                if (identityUser.LegacyForumUserId.HasValue && identityUser.LegacyForumUserId != forumUser.Id)
                {
                    throw new InvalidOperationException($"Cannot migrate forum user '{forumUser.Username}' because the matching Identity account is linked to another forum profile.");
                }

                identityUser.DisplayName = forumUser.DisplayName;
                identityUser.Bio = forumUser.Bio;
                identityUser.CountryCode = forumUser.CountryCode;
                identityUser.RegisteredAtUtc = forumUser.RegisteredAtUtc;
                identityUser.IsSuspended = forumUser.IsSuspended;
                identityUser.LegacyForumUserId = forumUser.Id;
                var updateResult = await userManager.UpdateAsync(identityUser);
                if (!updateResult.Succeeded)
                {
                    throw new InvalidOperationException($"Failed to link forum user '{forumUser.Username}' to Identity: {string.Join(", ", updateResult.Errors.Select(error => error.Description))}");
                }
            }

            forumUser.Password = "[MIGRATED_TO_IDENTITY]";
        }

        await dbContext.SaveChangesAsync();
    }
}
