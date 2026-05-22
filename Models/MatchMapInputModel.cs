using System.ComponentModel.DataAnnotations;

namespace cs2_esports.Models;

public class MatchMapInputModel
{
    [Display(Name = "Map")]
    public MapPool? Map { get; set; }

    public int MapSequence { get; set; }

    [Range(0, 99)]
    [Display(Name = "Team A Score")]
    public int? TeamAScore { get; set; }

    [Range(0, 99)]
    [Display(Name = "Team B Score")]
    public int? TeamBScore { get; set; }

    [Display(Name = "Overtime")]
    public bool WentToOvertime { get; set; }
}