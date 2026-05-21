using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cs2_esports.Models;

public class Event
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Organizer { get; set; } = string.Empty;
    public EventTier Tier { get; set; }
    public decimal PrizePoolUsd { get; set; }
    public DateTime StartDateUtc { get; set; }
    public DateTime EndDateUtc { get; set; }
    public bool IsLan { get; set; }

    [ForeignKey(nameof(EventVenue))]
    public int EventVenueId { get; set; }
    public virtual EventVenue? EventVenue { get; set; }

    [ForeignKey(nameof(AdminUser))]
    public int? AdminUserId { get; set; }
    public virtual AdminUser? AdminUser { get; set; }

    public virtual ICollection<Team> Teams { get; set; } = new List<Team>();
    public virtual ICollection<Match> Matches { get; set; } = new List<Match>();
    public virtual ICollection<Forum> ForumThreads { get; set; } = new List<Forum>();
}

