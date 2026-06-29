namespace cs2_esports.Services.Auditing;

public sealed record AuditLogEntry
{
    public DateTimeOffset TimestampUtc { get; init; }
    public string Outcome { get; init; } = string.Empty;
    public string HttpMethod { get; init; } = string.Empty;
    public string Entity { get; init; } = string.Empty;
    public string Action { get; init; } = string.Empty;
    public string? EntityId { get; init; }
    public string? ActorName { get; init; }
    public string? ActorId { get; init; }
    public IReadOnlyList<string> ActorRoles { get; init; } = [];
    public string Path { get; init; } = string.Empty;
    public int StatusCode { get; init; }
    public string TraceId { get; init; } = string.Empty;
    public string? RemoteIpAddress { get; init; }
    public string? ErrorType { get; init; }
}
