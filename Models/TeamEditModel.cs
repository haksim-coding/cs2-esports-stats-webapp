namespace cs2_esports.Models;

public class TeamEditModel : TeamCreateModel
{
    public int Id { get; set; }

    public string RouteSlug { get; set; } = string.Empty;
}