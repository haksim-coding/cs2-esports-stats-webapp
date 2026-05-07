using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cs2_esports.Models;

public class Player
{
    [Key]
    public int Id { get; set; }
    public string Nickname { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string CountryCode { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public PlayerRole Role { get; set; }
    public decimal Rating2 { get; set; }
    public int TotalMapsPlayed { get; set; }
    public DateTime JoinedTeamAtUtc { get; set; }
    [NotMapped]
    public bool IsFavorite { get; set; }

    [ForeignKey(nameof(Team))]
    public int TeamId { get; set; }
    public virtual Team? Team { get; set; }
}
