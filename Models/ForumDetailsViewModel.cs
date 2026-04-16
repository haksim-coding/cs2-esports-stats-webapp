namespace cs2_esports.Models;

public class ForumDetailsViewModel
{
    public Forum Forum { get; set; } = null!;
    public IReadOnlyList<ForumComment> Comments { get; set; } = Array.Empty<ForumComment>();
    public ForumCommentInputModel NewComment { get; set; } = new();
    public ForumUser? CurrentUser { get; set; }
}
