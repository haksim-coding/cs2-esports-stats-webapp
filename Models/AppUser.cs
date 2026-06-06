using Microsoft.AspNetCore.Identity;

namespace cs2_esports.Models;

public class AppUser : IdentityUser
{
    public string DisplayName { get; set; } = string.Empty;
    public string Bio { get; set; } = string.Empty;
    public string CountryCode { get; set; } = string.Empty;
    public DateTime RegisteredAtUtc { get; set; } = DateTime.UtcNow;
    public bool IsSuspended { get; set; }
    public DateTime? HiredAtUtc { get; set; }
    public DateTime? LastModerationActionAtUtc { get; set; }
    public int? LegacyAdminUserId { get; set; }
}