using System.ComponentModel.DataAnnotations;

namespace cs2_esports.Dtos.Events;

public sealed class AiEventDraftRequest
{
    [Required]
    [StringLength(2000, MinimumLength = 5)]
    public string Prompt { get; set; } = string.Empty;
}
