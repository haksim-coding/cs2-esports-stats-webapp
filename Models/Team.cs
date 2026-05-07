using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cs2_esports.Models;

public class Team
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Tag { get; set; } = string.Empty;
    public string CountryCode { get; set; } = string.Empty;
    public int WorldRanking { get; set; }
    public int FoundedYear { get; set; }
    public decimal PrizeMoneyUsd { get; set; }
    public DateTime LastRosterUpdateUtc { get; set; } = DateTime.UtcNow;
    [NotMapped]
    public bool IsFavorite { get; set; }

    public virtual ICollection<Player> Players { get; set; } = new List<Player>();
    public virtual ICollection<Event> Tournaments { get; set; } = new List<Event>();
}

