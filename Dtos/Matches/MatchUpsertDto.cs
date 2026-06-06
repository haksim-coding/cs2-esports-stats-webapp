using System.ComponentModel.DataAnnotations;
using cs2_esports.Models;

namespace cs2_esports.Dtos.Matches;

public class MatchUpsertDto
{
    [Required]
    public DateTime ScheduledAtUtc { get; set; }

    public bool IsFinished { get; set; }

    [Required]
    [EnumDataType(typeof(MatchFormat))]
    public MatchFormat Format { get; set; }

    public DateTime? FinishedAtUtc { get; set; }

    [Range(1, int.MaxValue)]
    public int EventId { get; set; }

    [Range(1, int.MaxValue)]
    public int TeamAId { get; set; }

    [Range(1, int.MaxValue)]
    public int TeamBId { get; set; }

    public List<MatchMapDto> Maps { get; set; } = [];
}