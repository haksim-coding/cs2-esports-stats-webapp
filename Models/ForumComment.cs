namespace cs2_esports.Models;

public class ForumComment
{
    public int Id { get; set; }
    public int ForumId { get; set; }
    public Forum? Forum { get; set; }

    public int AuthorId { get; set; }
    public ForumUser? Author { get; set; }

    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public bool IsEdited { get; set; }
}
