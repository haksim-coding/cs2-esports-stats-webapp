using cs2_esports.Models;

namespace cs2_esports.Dtos.Events;

public class EventSummaryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Organizer { get; set; } = string.Empty;
    public EventTier Tier { get; set; }
    public decimal PrizePoolUsd { get; set; }
    public DateTime StartDateUtc { get; set; }
    public DateTime EndDateUtc { get; set; }
    public bool IsLan { get; set; }
    public string? BannerImagePath { get; set; }
    public int EventVenueId { get; set; }
    public string VenueName { get; set; } = string.Empty;
    public string VenueCity { get; set; } = string.Empty;
    public string VenueCountryCode { get; set; } = string.Empty;
    public int TeamCount { get; set; }
    public int MatchCount { get; set; }
}
