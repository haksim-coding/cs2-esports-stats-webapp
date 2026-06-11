namespace cs2_esports.Models;

public class PlayerDetailsViewModel
{
    public required Player Player { get; init; }
    public IReadOnlyList<Match> UpcomingMatches { get; init; } = Array.Empty<Match>();
}
