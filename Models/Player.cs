namespace cs2_esports.Models;

public class Player
{
    public int Id { get; set; }
    public string Nickname { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string CountryCode { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public PlayerRole Role { get; set; }
    public decimal Rating2 { get; set; }
    public int TotalMapsPlayed { get; set; }
    public DateTime JoinedTeamAtUtc { get; set; }

    public int TeamId { get; set; }
    public Team? Team { get; set; }
}
