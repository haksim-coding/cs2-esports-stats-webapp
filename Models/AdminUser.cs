using System.Collections.Generic;

namespace cs2_esports.Models;

public class AdminUser : User
{
    public DateTime HiredAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? LastModerationActionAtUtc { get; set; }
    public string PermissionGroup { get; set; } = "TournamentAdmin";

    public virtual ICollection<Event> ManagedTournaments { get; set; } = new List<Event>();
}

