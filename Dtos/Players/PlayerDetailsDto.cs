using cs2_esports.Dtos.Teams;

namespace cs2_esports.Dtos.Players;

public class PlayerDetailsDto : PlayerSummaryDto
{
    public TeamListItemDto? CurrentTeam { get; set; }
}