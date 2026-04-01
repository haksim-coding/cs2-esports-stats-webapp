namespace cs2_esports.Models;

public class ForumUser : User
{
    public DateTime LastActiveAtUtc { get; set; } = DateTime.UtcNow;
    public bool IsPremiumMember { get; set; }

    public List<Forum> Threads { get; set; } = new();
}
