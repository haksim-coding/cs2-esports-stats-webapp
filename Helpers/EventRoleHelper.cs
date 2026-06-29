using System.Security.Claims;

namespace cs2_esports.Helpers;

public static class EventRoleHelper
{
    public const string SuperAdminRole = "SuperAdmin";
    public const string BlastAdminRole = "BlastAdmin";
    public const string EslAdminRole = "EslAdmin";
    public const string TournamentAdminRole = "TournamentAdmin";

    // AuthorizeAttribute requires a compile-time constant containing comma-separated roles.
    public const string EventAdminRoles = $"{SuperAdminRole},{BlastAdminRole},{EslAdminRole},{TournamentAdminRole}";
    public const string SuperAdminOnlyRoles = SuperAdminRole;

    private static readonly string[] EventAdminRoleNames =
    [
        SuperAdminRole,
        BlastAdminRole,
        EslAdminRole,
        TournamentAdminRole
    ];

    public static bool CanManageRosterContent(ClaimsPrincipal user) =>
        IsAuthenticatedInRole(user, SuperAdminRole);

    public static bool IsEventAdmin(ClaimsPrincipal user) =>
        user.Identity?.IsAuthenticated == true && EventAdminRoleNames.Any(user.IsInRole);

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

    private static string GetRequiredRole(string organizer)
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

    private static bool IsAuthenticatedInRole(ClaimsPrincipal user, string role) =>
        user.Identity?.IsAuthenticated == true && user.IsInRole(role);
}
