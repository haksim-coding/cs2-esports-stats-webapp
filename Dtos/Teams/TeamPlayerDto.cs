namespace cs2_esports.Dtos.Teams;

public class TeamPlayerDto
{
    public int Id { get; set; }
    public string Nickname { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string CountryCode { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public decimal Rating2 { get; set; }
    public int TotalMapsPlayed { get; set; }
    public DateTime JoinedTeamAtUtc { get; set; }
    public int? TeamId { get; set; }
}