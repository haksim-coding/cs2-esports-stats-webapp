namespace cs2_esports.Models;

public class EventEditModel : EventCreateModel
{
    public int Id { get; set; }
    public string? CurrentBannerImagePath { get; set; }
}
