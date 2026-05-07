namespace cs2_esports.Models;

public class ForumUserProfileViewModel
{
    public ForumUser User { get; set; } = null!;
    public IReadOnlyList<Team> FavoriteTeams { get; set; } = Array.Empty<Team>();
    public IReadOnlyList<Player> FavoritePlayers { get; set; } = Array.Empty<Player>();
}