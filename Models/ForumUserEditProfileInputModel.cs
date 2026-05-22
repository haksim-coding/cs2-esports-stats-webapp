using System.ComponentModel.DataAnnotations;

namespace cs2_esports.Models;

public class ForumUserEditProfileInputModel
{
    [Required]
    [StringLength(40, MinimumLength = 3)]
    [Display(Name = "Username")]
    public string Username { get; set; } = string.Empty;

    [StringLength(500)]
    [Display(Name = "Bio")]
    public string Bio { get; set; } = string.Empty;
}