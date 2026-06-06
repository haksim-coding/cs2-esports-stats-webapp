using System.ComponentModel.DataAnnotations;

namespace cs2_esports.Dtos.Teams;

public class TeamUpsertDto
{
    [Required]
    [StringLength(80, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(10, MinimumLength = 2)]
    public string Tag { get; set; } = string.Empty;

    [Required]
    [StringLength(2, MinimumLength = 2)]
    public string CountryCode { get; set; } = string.Empty;

    [Range(1990, 2100)]
    public int FoundedYear { get; set; }

    public decimal PrizeMoneyUsd { get; set; }

    [MaxLength(5, ErrorMessage = "You can select up to 5 players.")]
    public List<int> SelectedPlayerIds { get; set; } = [];
}