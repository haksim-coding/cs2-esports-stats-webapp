using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cs2_esports.Models;

public class MatchMap
{
    [Key]
    public int Id { get; set; }

    public int MapSequence { get; set; }
    public MapPool Map { get; set; }
    public int TeamAScore { get; set; }
    public int TeamBScore { get; set; }
    public bool WentToOvertime { get; set; }

    [ForeignKey(nameof(Match))]
    public int MatchId { get; set; }
    public virtual Match? Match { get; set; }
}