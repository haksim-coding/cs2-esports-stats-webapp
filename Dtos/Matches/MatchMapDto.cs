using cs2_esports.Models;

namespace cs2_esports.Dtos.Matches;

public class MatchMapDto
{
    public int Id { get; set; }
    public int MapSequence { get; set; }
    public MapPool Map { get; set; }
    public int TeamAScore { get; set; }
    public int TeamBScore { get; set; }
    public bool WentToOvertime { get; set; }
}