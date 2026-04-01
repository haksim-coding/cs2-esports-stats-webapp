namespace cs2_esports.Models;

public class AdminUser : User
{
    public DateTime HiredAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? LastModerationActionAtUtc { get; set; }
    public string PermissionGroup { get; set; } = "TournamentAdmin";

    public List<Tournament> ManagedTournaments { get; set; } = new();
}
