using System.Collections.Generic;

namespace cs2_esports.Models
{
    public class HomeViewModel
    {
        public List<Event> UpcomingEvents { get; set; } = new();
        public List<Player> TopPlayers { get; set; } = new();
        public List<Team> TopTeams { get; set; } = new();
        public ForumUser? LoggedInUser { get; set; }
    }
}
