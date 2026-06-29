namespace cs2_esports.Services.Auditing;

public interface IAuditLogService
{
    Task WriteAsync(AuditLogEntry entry, CancellationToken cancellationToken = default);
    IReadOnlyList<AuditLogFile> GetLogFiles();
    string? ResolveLogFile(string? fileName);
}
