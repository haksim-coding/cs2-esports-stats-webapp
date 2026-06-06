using System.ComponentModel.DataAnnotations;
using cs2_esports.Models;

namespace cs2_esports.Dtos.Events;

public class EventUpsertDto
{
    [Required]
    [StringLength(120, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(120, MinimumLength = 2)]
    public string Organizer { get; set; } = string.Empty;

    [Required]
    [EnumDataType(typeof(EventTier))]
    public EventTier Tier { get; set; }

    [Range(typeof(decimal), "0", "1000000000")]
    public decimal PrizePoolUsd { get; set; }

    [Required]
    public DateTime StartDateUtc { get; set; }

    [Required]
    public DateTime EndDateUtc { get; set; }

    public bool IsLan { get; set; }

    [StringLength(260)]
    public string? BannerImagePath { get; set; }

    [Range(1, int.MaxValue)]
    public int EventVenueId { get; set; }

    public List<int> SelectedTeamIds { get; set; } = [];
}
