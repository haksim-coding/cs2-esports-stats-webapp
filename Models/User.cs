namespace cs2_esports.Models;

public abstract class User
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string CountryCode { get; set; } = string.Empty;
    public DateTime RegisteredAtUtc { get; set; } = DateTime.UtcNow;
    public bool IsSuspended { get; set; }
}
