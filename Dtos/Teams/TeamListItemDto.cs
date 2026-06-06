namespace cs2_esports.Dtos.Teams;

public class TeamListItemDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Tag { get; set; } = string.Empty;
    public string CountryCode { get; set; } = string.Empty;
    public int WorldRanking { get; set; }
    public int FoundedYear { get; set; }
    public decimal PrizeMoneyUsd { get; set; }
    public DateTime LastRosterUpdateUtc { get; set; }
    public int PlayerCount { get; set; }
    public List<TeamPlayerDto> Players { get; set; } = [];
}
