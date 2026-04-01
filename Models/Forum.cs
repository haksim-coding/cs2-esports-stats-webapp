namespace cs2_esports.Models;

public class Forum
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public ForumCategory Category { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime LastUpdatedAtUtc { get; set; } = DateTime.UtcNow;
    public int ViewCount { get; set; }
    public bool IsPinned { get; set; }
    public bool IsLocked { get; set; }

    public int AuthorId { get; set; }
    public ForumUser? Author { get; set; }

    public int? TournamentId { get; set; }
    public Tournament? Tournament { get; set; }
}
