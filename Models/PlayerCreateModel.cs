using System.ComponentModel.DataAnnotations;

namespace cs2_esports.Models;

public class PlayerCreateModel
{
    [Required]
    [StringLength(40, MinimumLength = 2)]
    [Display(Name = "Nickname")]
    public string Nickname { get; set; } = string.Empty;

    [Required]
    [StringLength(80, MinimumLength = 2)]
    [Display(Name = "Full Name")]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [StringLength(2, MinimumLength = 2)]
    [Display(Name = "Country Code")]
    public string CountryCode { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Date of Birth")]
    public DateTime DateOfBirth { get; set; }

    [Required]
    [Display(Name = "Role")]
    public PlayerRole Role { get; set; }

    [Range(typeof(decimal), "0", "5")]
    [Display(Name = "Rating 2.0")]
    public decimal Rating2 { get; set; }

    [Range(0, int.MaxValue)]
    [Display(Name = "Total Maps Played")]
    public int TotalMapsPlayed { get; set; }

    [Display(Name = "Player image")]
    public IFormFile? PlayerImage { get; set; }

    [Display(Name = "Current Team")]
    public int? TeamId { get; set; }
}
