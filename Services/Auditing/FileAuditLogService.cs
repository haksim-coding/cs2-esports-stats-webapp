using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;

namespace cs2_esports.Services.Auditing;

public sealed class FileAuditLogService : IAuditLogService, IDisposable
{
    private const string FilePrefix = "audit-";
    private const string FileExtension = ".jsonl";
    private readonly string _logDirectory;
    private readonly ILogger<FileAuditLogService> _logger;
    private readonly SemaphoreSlim _writeLock = new(1, 1);
    private readonly JsonSerializerOptions _serializerOptions = new(JsonSerializerDefaults.Web)
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    public FileAuditLogService(
        IOptions<AuditLogOptions> options,
        IWebHostEnvironment environment,
        ILogger<FileAuditLogService> logger)
    {
        var configuredDirectory = string.IsNullOrWhiteSpace(options.Value.Directory)
            ? "Logs"
            : options.Value.Directory.Trim();
        _logDirectory = Path.GetFullPath(Path.IsPathRooted(configuredDirectory)
            ? configuredDirectory
            : Path.Combine(environment.ContentRootPath, configuredDirectory));
        _logger = logger;
    }

    public async Task WriteAsync(AuditLogEntry entry, CancellationToken cancellationToken = default)
    {
        try
        {
            await _writeLock.WaitAsync(cancellationToken);
            try
            {
                Directory.CreateDirectory(_logDirectory);
                var timestamp = entry.TimestampUtc == default ? DateTimeOffset.UtcNow : entry.TimestampUtc;
                var filePath = Path.Combine(_logDirectory, $"{FilePrefix}{timestamp:yyyy-MM-dd}{FileExtension}");
                var json = JsonSerializer.Serialize(entry with { TimestampUtc = timestamp }, _serializerOptions);

                await using var stream = new FileStream(
                    filePath,
                    FileMode.Append,
                    FileAccess.Write,
                    FileShare.Read,
                    bufferSize: 4096,
                    useAsync: true);
                await using var writer = new StreamWriter(stream);
                await writer.WriteLineAsync(json.AsMemory(), cancellationToken);
            }
            finally
            {
                _writeLock.Release();
            }
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            // A cancelled request should not turn a completed application action into an error.
        }
        catch (Exception exception)
        {
            // Audit storage must never make the business operation fail.
            _logger.LogError(exception, "Could not append an entry to the audit log file.");
        }
    }

    public IReadOnlyList<AuditLogFile> GetLogFiles()
    {
        try
        {
            Directory.CreateDirectory(_logDirectory);
            return Directory.EnumerateFiles(_logDirectory, $"{FilePrefix}*{FileExtension}", SearchOption.TopDirectoryOnly)
                .Select(path => new FileInfo(path))
                .OrderByDescending(file => file.LastWriteTimeUtc)
                .Select(file => new AuditLogFile(file.Name, file.Length, file.LastWriteTimeUtc))
                .ToList();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Could not enumerate audit log files.");
            return [];
        }
    }

    public string? ResolveLogFile(string? fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName) ||
            !fileName.StartsWith(FilePrefix, StringComparison.Ordinal) ||
            !fileName.EndsWith(FileExtension, StringComparison.Ordinal) ||
            !string.Equals(fileName, Path.GetFileName(fileName), StringComparison.Ordinal))
        {
            return null;
        }

        var fullPath = Path.GetFullPath(Path.Combine(_logDirectory, fileName));
        var relativePath = Path.GetRelativePath(_logDirectory, fullPath);
        if (relativePath.StartsWith("..", StringComparison.Ordinal) ||
            Path.IsPathRooted(relativePath) ||
            !File.Exists(fullPath))
        {
            return null;
        }

        return fullPath;
    }

    public void Dispose()
    {
        _writeLock.Dispose();
    }
}
