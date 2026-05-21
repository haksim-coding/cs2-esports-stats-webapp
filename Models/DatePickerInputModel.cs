namespace cs2_esports.Models;

public class DatePickerInputModel
{
    public string Name { get; set; } = string.Empty;

    public DateTime? Value { get; set; }

    public string Mode { get; set; } = "datetime";
}