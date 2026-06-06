using cs2_esports.Dtos.Events;
using cs2_esports.Dtos.Teams;

namespace cs2_esports.Dtos.Matches;

public class MatchDetailsDto : MatchSummaryDto
{
    public EventSummaryDto? Event { get; set; }
    public TeamListItemDto? TeamA { get; set; }
    public TeamListItemDto? TeamB { get; set; }
    public List<MatchMapDto> Maps { get; set; } = [];
}