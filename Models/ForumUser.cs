using System.Collections.Generic;

namespace cs2_esports.Models;

public class ForumUser : User
{
    public DateTime LastActiveAtUtc { get; set; } = DateTime.UtcNow;
    public bool IsPremiumMember { get; set; }

    public virtual ICollection<Forum> Threads { get; set; } = new List<Forum>();
    public virtual ICollection<ForumComment> Comments { get; set; } = new List<ForumComment>();
    public virtual ICollection<Team> FavoriteTeams { get; set; } = new List<Team>();
    public virtual ICollection<Player> FavoritePlayers { get; set; } = new List<Player>();
}
