param(
    [string]$Event = "Unknown"
)

$logPath = Join-Path $PSScriptRoot "agent_log.txt"

# Ensure the hooks directory exists, then append event + payload to the log file.
New-Item -ItemType Directory -Force -Path $PSScriptRoot | Out-Null

$payload = [Console]::In.ReadToEnd()
if ([string]::IsNullOrWhiteSpace($payload)) {
    $payload = ($input | Out-String)
}
if ([string]::IsNullOrWhiteSpace($payload)) {
    $payload = "<no payload>"
}

$timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss.fff"
$entry = "[$timestamp] [$Event]`r`n$payload`r`n----------------`r`n"
Add-Content -Path $logPath -Value $entry
