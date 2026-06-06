using cs2_esports.Models;

namespace cs2_esports.Dtos.Matches;

public class MatchSummaryDto
{
    public int Id { get; set; }
    public DateTime ScheduledAtUtc { get; set; }
    public bool IsFinished { get; set; }
    public MatchFormat Format { get; set; }
    public int TeamAScore { get; set; }
    public int TeamBScore { get; set; }
    public DateTime? FinishedAtUtc { get; set; }
    public int EventId { get; set; }
    public string EventName { get; set; } = string.Empty;
    public int TeamAId { get; set; }
    public string TeamAName { get; set; } = string.Empty;
    public string TeamATag { get; set; } = string.Empty;
    public int TeamBId { get; set; }
    public string TeamBName { get; set; } = string.Empty;
    public string TeamBTag { get; set; } = string.Empty;
    public int MapCount { get; set; }
}