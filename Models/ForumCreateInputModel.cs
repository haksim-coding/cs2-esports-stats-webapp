using System.ComponentModel.DataAnnotations;

namespace cs2_esports.Models;

public class ForumCreateInputModel
{
    [Required]
    [StringLength(120, MinimumLength = 5)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(4000, MinimumLength = 10)]
    public string Content { get; set; } = string.Empty;

    [Required]
    public ForumCategory Category { get; set; }

    public int AuthorId { get; set; }

    public int? TournamentId { get; set; }
}
