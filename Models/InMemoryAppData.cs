namespace cs2_esports.Models;

public class InMemoryAppData
{
    public List<AdminUser> AdminUsers { get; set; } = new();
    public List<EventVenue> EventVenues { get; set; } = new();
    public List<Team> Teams { get; set; } = new();
    public List<Player> Players { get; set; } = new();
    public List<Event> Tournaments { get; set; } = new();
    public List<ForumUser> ForumUsers { get; set; } = new();
    public List<Forum> Forums { get; set; } = new();
    public List<ForumComment> ForumComments { get; set; } = new();
}

