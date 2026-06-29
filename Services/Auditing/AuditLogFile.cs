namespace cs2_esports.Services.Auditing;

public sealed record AuditLogFile(
    string FileName,
    long FileSize,
    DateTime LastModifiedAtUtc);
