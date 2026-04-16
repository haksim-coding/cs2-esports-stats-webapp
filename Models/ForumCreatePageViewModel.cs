namespace cs2_esports.Models;

public class ForumCreatePageViewModel
{
    public ForumCreateInputModel Input { get; set; } = new();
    public IReadOnlyList<Event> Events { get; set; } = Array.Empty<Event>();
    public ForumUser? CurrentUser { get; set; }
}
