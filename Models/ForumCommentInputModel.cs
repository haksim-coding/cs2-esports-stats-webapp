using System.ComponentModel.DataAnnotations;

namespace cs2_esports.Models;

public class ForumCommentInputModel
{
    [Required]
    [Range(1, int.MaxValue)]
    public int ForumId { get; set; }

    public int AuthorId { get; set; }

    [Required]
    [StringLength(2000, MinimumLength = 2)]
    public string Content { get; set; } = string.Empty;
}
