using System.Collections.Generic;

namespace cs2_esports.Models
{
    public class HomeViewModel
    {
        public List<Event> UpcomingEvents { get; set; } = new();
        public List<Player> TopPlayers { get; set; } = new();
        public List<Team> TopTeams { get; set; } = new();
        public User? LoggedInUser { get; set; }
        public string? LoggedInUserRoleLabel { get; set; }
        public string? LoggedInUserRoleBadgeClass { get; set; }
        public bool CanCreateForumPost { get; set; }
    }
}
