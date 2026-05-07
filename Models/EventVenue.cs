using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace cs2_esports.Models;

public class EventVenue
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string CountryCode { get; set; } = string.Empty;
    public int Capacity { get; set; }
    public bool IsIndoor { get; set; }
    public string SurfaceType { get; set; } = string.Empty;

    public virtual ICollection<Event> Tournaments { get; set; } = new List<Event>();
}

