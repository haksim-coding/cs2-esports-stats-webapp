using cs2_esports.Dtos.Matches;
using cs2_esports.Dtos.Teams;
using cs2_esports.Models;

namespace cs2_esports.Dtos.Events;

public class EventDetailsDto : EventSummaryDto
{
    public EventVenueDto? Venue { get; set; }
    public List<TeamListItemDto> Teams { get; set; } = [];
    public List<MatchSummaryDto> Matches { get; set; } = [];
}