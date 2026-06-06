using System.ComponentModel.DataAnnotations;
using cs2_esports.Models;

namespace cs2_esports.Dtos.Players;

public class PlayerUpsertDto
{
    [Required]
    [StringLength(40, MinimumLength = 2)]
    public string Nickname { get; set; } = string.Empty;

    [Required]
    [StringLength(80, MinimumLength = 2)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [StringLength(2, MinimumLength = 2)]
    public string CountryCode { get; set; } = string.Empty;

    [Required]
    public DateTime DateOfBirth { get; set; }

    [Required]
    [EnumDataType(typeof(PlayerRole))]
    public PlayerRole Role { get; set; }

    [Range(typeof(decimal), "0", "5")]
    public decimal Rating2 { get; set; }

    [Range(0, int.MaxValue)]
    public int TotalMapsPlayed { get; set; }

    [Range(1, int.MaxValue)]
    public int? TeamId { get; set; }
}