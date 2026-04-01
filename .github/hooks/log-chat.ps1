param(
    [Parameter(ValueFromPipeline=$true)]
    [string]$InputText,
    [string]$Label = "Chat"
)

$logPath = Join-Path $PSScriptRoot "agent_log.txt"
New-Item -ItemType Directory -Force -Path $PSScriptRoot | Out-Null

if ([string]::IsNullOrWhiteSpace($InputText)) {
    $InputText = Read-Host "Enter text to log"
}

$timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss.fff"
$entry = "[$timestamp] [$Label]`r`n$InputText`r`n----------------`r`n"
Add-Content -Path $logPath -Value $entry

Write-Host "✓ Logged to agent_log.txt" -ForegroundColor Green
