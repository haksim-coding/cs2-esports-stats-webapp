using System.ComponentModel.DataAnnotations;

namespace cs2_esports.Models;

public class MatchCreateModel
{
    [Required]
    [Display(Name = "Scheduled At")]
    public DateTime ScheduledAtUtc { get; set; }

    [Display(Name = "Finished Match")]
    public bool IsFinished { get; set; }

    [Required]
    [Display(Name = "Format")]
    public MatchFormat Format { get; set; }

    [Range(0, 99)]
    [Display(Name = "Team A Score")]
    public int TeamAScore { get; set; }

    [Range(0, 99)]
    [Display(Name = "Team B Score")]
    public int TeamBScore { get; set; }

    [Display(Name = "Finished At")]
    public DateTime? FinishedAtUtc { get; set; }

    [Range(1, int.MaxValue)]
    [Display(Name = "Event")]
    public int EventId { get; set; }

    [Range(1, int.MaxValue)]
    [Display(Name = "Team A")]
    public int TeamAId { get; set; }

    [Range(1, int.MaxValue)]
    [Display(Name = "Team B")]
    public int TeamBId { get; set; }

    public List<MatchMapInputModel> Maps { get; set; } = [];
}