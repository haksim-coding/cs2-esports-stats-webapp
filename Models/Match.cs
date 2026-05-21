using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cs2_esports.Models;

public class Match
{
    [Key]
    public int Id { get; set; }

    public DateTime ScheduledAtUtc { get; set; }
    public bool IsFinished { get; set; }
    public MatchFormat Format { get; set; }
    public int TeamAScore { get; set; }
    public int TeamBScore { get; set; }
    public DateTime? FinishedAtUtc { get; set; }

    [ForeignKey(nameof(Event))]
    public int EventId { get; set; }
    public virtual Event? Event { get; set; }

    [ForeignKey(nameof(TeamA))]
    public int TeamAId { get; set; }
    public virtual Team? TeamA { get; set; }

    [ForeignKey(nameof(TeamB))]
    public int TeamBId { get; set; }
    public virtual Team? TeamB { get; set; }

    public virtual ICollection<MatchMap> Maps { get; set; } = new List<MatchMap>();
}