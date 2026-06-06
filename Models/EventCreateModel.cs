using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace cs2_esports.Models;

public class EventCreateModel
{
    [Required]
    [StringLength(120, MinimumLength = 2)]
    [Display(Name = "Event Name")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(120, MinimumLength = 2)]
    [Display(Name = "Organizer")]
    public string Organizer { get; set; } = string.Empty;

    [Required]
    [Display(Name = "Tier")]
    public EventTier Tier { get; set; }

    [Range(typeof(decimal), "0", "1000000000")]
    [Display(Name = "Prize Pool")]
    public decimal PrizePoolUsd { get; set; }

    [Required]
    [Display(Name = "Start Date")]
    public DateTime StartDateUtc { get; set; }

    [Required]
    [Display(Name = "End Date")]
    public DateTime EndDateUtc { get; set; }

    [Display(Name = "LAN Event")]
    public bool IsLan { get; set; }

    [Display(Name = "Event Banner")]
    [ValidateNever]
    public IFormFile? BannerImage { get; set; }

    [Range(1, int.MaxValue)]
    [Display(Name = "Venue")]
    public int EventVenueId { get; set; }

    [ValidateNever]
    public List<int> SelectedTeamIds { get; set; } = [];

    [ValidateNever]
    public List<TeamAutocompleteItemModel> SelectedTeams { get; set; } = [];
}
