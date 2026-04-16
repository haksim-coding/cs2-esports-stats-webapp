namespace cs2_esports.Models;

public class Event
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Organizer { get; set; } = string.Empty;
    public EventTier Tier { get; set; }
    public decimal PrizePoolUsd { get; set; }
    public DateTime StartDateUtc { get; set; }
    public DateTime EndDateUtc { get; set; }
    public bool IsLan { get; set; }

    public int EventVenueId { get; set; }
    public EventVenue? EventVenue { get; set; }

    public int? AdminUserId { get; set; }
    public AdminUser? AdminUser { get; set; }

    public List<Team> Teams { get; set; } = new();
    public List<Forum> ForumThreads { get; set; } = new();
}

