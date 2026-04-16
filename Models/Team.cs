namespace cs2_esports.Models;

public class Team
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Tag { get; set; } = string.Empty;
    public string CountryCode { get; set; } = string.Empty;
    public int WorldRanking { get; set; }
    public int FoundedYear { get; set; }
    public decimal PrizeMoneyUsd { get; set; }
    public DateTime LastRosterUpdateUtc { get; set; } = DateTime.UtcNow;

    public List<Player> Players { get; set; } = new();
    public List<Event> Tournaments { get; set; } = new();
}

