using System.ComponentModel.DataAnnotations;

namespace cs2_esports.Models;

public class ForumRegisterInputModel
{
    [Required]
    [StringLength(40, MinimumLength = 3)]
    public string Username { get; set; } = string.Empty;

    [Required]
    [StringLength(60, MinimumLength = 3)]
    public string DisplayName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(2, MinimumLength = 2)]
    public string CountryCode { get; set; } = string.Empty;

    [Required]
    [StringLength(64, MinimumLength = 6)]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}