using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cs2_esports.Models;

public class ForumComment
{
    [Key]
    public int Id { get; set; }

    [ForeignKey(nameof(Forum))]
    public int ForumId { get; set; }
    public virtual Forum? Forum { get; set; }

    [ForeignKey(nameof(Author))]
    public int AuthorId { get; set; }
    public virtual ForumUser? Author { get; set; }

    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public bool IsEdited { get; set; }
}
