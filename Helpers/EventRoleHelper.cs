using System.Security.Claims;

namespace cs2_esports.Helpers;




/*
TODO

Make sure that the role managemnt is scalable, not using string literals everywhere, maybe an enum or a more structured approach to defining roles and permissions. This will make it easier to manage and extend in the future as new organizers or roles are added.

*/



public static class EventRoleHelper
{
    public const string SuperAdminRole = "SuperAdmin";
    public const string BlastAdminRole = "BlastAdmin";
    public const string EslAdminRole = "EslAdmin";
    public const string TournamentAdminRole = "TournamentAdmin";
    public const string EventAdminRoles = $"{SuperAdminRole},{BlastAdminRole},{EslAdminRole},{TournamentAdminRole}";
    public const string SuperAdminOnlyRoles = SuperAdminRole;

    public static bool CanManageRosterContent(ClaimsPrincipal user)
    {
        return user.Identity?.IsAuthenticated == true && user.IsInRole(SuperAdminRole);
    }

    public static bool IsSuperAdmin(ClaimsPrincipal user)
    {
        return user.Identity?.IsAuthenticated == true && user.IsInRole(SuperAdminRole);
    }

    public static bool IsEventAdmin(ClaimsPrincipal user)
    {
        return user.Identity?.IsAuthenticated == true &&
            (user.IsInRole(SuperAdminRole) ||
             user.IsInRole(BlastAdminRole) ||
             user.IsInRole(EslAdminRole) ||
             user.IsInRole(TournamentAdminRole));
    }

    public static string GetDefaultOrganizerForAdmin(ClaimsPrincipal user)
    {
        if (user.IsInRole(EslAdminRole))
        {
            return "ESL";
        }

        if (user.IsInRole(BlastAdminRole))
        {
            return "BLAST";
        }

        return string.Empty;
    }

    public static string GetRequiredRole(string organizer)
    {
        var normalizedOrganizer = organizer.Trim().ToLowerInvariant();

        if (normalizedOrganizer.Contains("blast"))
        {
            return BlastAdminRole;
        }

        if (normalizedOrganizer.Contains("esl"))
        {
            return EslAdminRole;
        }

        return TournamentAdminRole;
    }

    public static bool CanManageOrganizer(ClaimsPrincipal user, string organizer)
    {
        if (user.Identity?.IsAuthenticated != true)
        {
            return false;
        }

        return user.IsInRole(SuperAdminRole) || user.IsInRole(GetRequiredRole(organizer));
    }

    public static string GetRoleLabel(string role)
    {
        return role switch
        {
            SuperAdminRole => "Super Admin",
            BlastAdminRole => "BLAST Admin",
            EslAdminRole => "ESL Admin",
            TournamentAdminRole => "Tournament Admin",
            _ => role
        };
    }

    public static string? GetPrimaryAdminRole(IEnumerable<string> roles)
    {
        if (roles.Contains(SuperAdminRole))
        {
            return SuperAdminRole;
        }

        if (roles.Contains(BlastAdminRole))
        {
            return BlastAdminRole;
        }

        if (roles.Contains(EslAdminRole))
        {
            return EslAdminRole;
        }

        if (roles.Contains(TournamentAdminRole))
        {
            return TournamentAdminRole;
        }

        return null;
    }

    public static IReadOnlyList<string> GetRolesForAdminUser(string username)
    {
        return [GetPrimaryRoleForAdminUser(username, null)];
    }

    public static IReadOnlyList<string> GetRolesForAdminUser(string username, string? permissionGroup)
    {
        return [GetPrimaryRoleForAdminUser(username, permissionGroup)];
    }

    public static string GetPrimaryRoleForAdminUser(string username, string? permissionGroup)
    {
        var normalizedUsername = username.Trim().ToLowerInvariant();

        var roleFromKnownAccount = normalizedUsername switch
        {
            "admin_maksim" => SuperAdminRole,
            "blast_admin" => BlastAdminRole,
            "esl_admin" => EslAdminRole,
            _ => null
        };

        if (!string.IsNullOrWhiteSpace(roleFromKnownAccount))
        {
            return roleFromKnownAccount;
        }

        return NormalizeAdminRole(permissionGroup);
    }

    private static string NormalizeAdminRole(string? permissionGroup)
    {
        return permissionGroup?.Trim() switch
        {
            SuperAdminRole => SuperAdminRole,
            BlastAdminRole => BlastAdminRole,
            EslAdminRole => EslAdminRole,
            TournamentAdminRole => TournamentAdminRole,
            _ => TournamentAdminRole
        };
    }
}
