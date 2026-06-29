namespace cs2_esports.Models;

public class AdminFilesIndexViewModel
{
    public List<AdminFileItemViewModel> EventFiles { get; set; } = [];
    public List<AdminFileItemViewModel> PlayerFiles { get; set; } = [];
    public List<AdminLogFileItemViewModel> LogFiles { get; set; } = [];
}
