namespace cs2_esports.Models;

public class AdminFileItemViewModel
{
    public string Kind { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string WebPath { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public List<string> AssignedEntities { get; set; } = [];
}
