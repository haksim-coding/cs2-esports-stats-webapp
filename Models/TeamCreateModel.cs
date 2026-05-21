using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace cs2_esports.Models;

public class TeamCreateModel
{
    [Required]
    [StringLength(80, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(10, MinimumLength = 2)]
    public string Tag { get; set; } = string.Empty;

    [Required]
    [StringLength(2, MinimumLength = 2)]
    [Display(Name = "Country Code")]
    public string CountryCode { get; set; } = string.Empty;

    [Range(1990, 2100)]
    [Display(Name = "Year Founded")]
    public int FoundedYear { get; set; }

    [MaxLength(5, ErrorMessage = "You can select up to 5 players.")]
    public List<int> SelectedPlayerIds { get; set; } = [];

    [ValidateNever]
    public List<PlayerAutocompleteItemModel> SelectedPlayers { get; set; } = [];
}