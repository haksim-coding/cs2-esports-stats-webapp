namespace cs2_esports.Models;

public class AdminLogFileItemViewModel
{
    public string FileName { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public DateTime LastModifiedAtUtc { get; set; }
}
