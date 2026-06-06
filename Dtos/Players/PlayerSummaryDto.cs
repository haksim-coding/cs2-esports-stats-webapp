using cs2_esports.Models;

namespace cs2_esports.Dtos.Players;

public class PlayerSummaryDto
{
    public int Id { get; set; }
    public string Nickname { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string CountryCode { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public PlayerRole Role { get; set; }
    public decimal Rating2 { get; set; }
    public int TotalMapsPlayed { get; set; }
    public string? ImagePath { get; set; }
    public DateTime JoinedTeamAtUtc { get; set; }
    public int? TeamId { get; set; }
    public string? TeamName { get; set; }
    public string? TeamTag { get; set; }
}
