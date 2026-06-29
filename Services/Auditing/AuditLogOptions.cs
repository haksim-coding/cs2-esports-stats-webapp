namespace cs2_esports.Services.Auditing;

public sealed class AuditLogOptions
{
    public const string SectionName = "AuditLogging";

    public string Directory { get; set; } = "Logs";
}
