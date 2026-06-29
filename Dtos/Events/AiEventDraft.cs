using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using cs2_esports.Models;

namespace cs2_esports.Dtos.Events;

public sealed class AiEventDraft
{
    [StringLength(120, MinimumLength = 2)]
    public string? Name { get; set; }

    [StringLength(120, MinimumLength = 2)]
    public string? Organizer { get; set; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public EventTier? Tier { get; set; }

    [Range(typeof(decimal), "0", "1000000000")]
    public decimal? PrizePoolUsd { get; set; }

    public DateTime? StartDateUtc { get; set; }

    public DateTime? EndDateUtc { get; set; }

    public bool? IsLan { get; set; }

    [Range(1, int.MaxValue)]
    public int? EventVenueId { get; set; }

    [MaxLength(16)]
    public List<int>? SelectedTeamIds { get; set; }
}
