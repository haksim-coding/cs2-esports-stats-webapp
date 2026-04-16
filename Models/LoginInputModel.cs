using System.ComponentModel.DataAnnotations;

namespace cs2_esports.Models;

public class LoginInputModel
{
    [Required]
    [Display(Name = "Username or Email")]
    public string Username { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;
}
