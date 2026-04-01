param(
    [string]$LogFile = ".\\.github\hooks\agent_log.txt",
    [int]$PollSeconds = 3
)

$logPath = Resolve-Path $LogFile -ErrorAction SilentlyContinue
if (-not $logPath) { $logPath = $LogFile }

$statePath = Join-Path $PSScriptRoot "monitor-state.json"
$processedRequestIds = New-Object 'System.Collections.Generic.HashSet[string]'

if (Test-Path $statePath) {
    try {
        $stateJson = Get-Content -Path $statePath -Raw -ErrorAction SilentlyContinue
        if (-not [string]::IsNullOrWhiteSpace($stateJson)) {
            $state = $stateJson | ConvertFrom-Json -ErrorAction Stop
            if ($state.requestIds) {
                foreach ($id in $state.requestIds) {
                    [void]$processedRequestIds.Add([string]$id)
                }
            }
        }
    } catch {
    }
}

Write-Host "Monitoring Copilot Chat prompts from chatSessions..." -ForegroundColor Cyan

while ($true) {
    try {
        $storageDir = "$env:APPDATA\Code\User\workspaceStorage"
        $sessionFiles = @(Get-ChildItem -Path $storageDir -Recurse -Filter "*.jsonl" -ErrorAction SilentlyContinue | Where-Object { $_.FullName -match "chatSessions\\.*\.jsonl$" })

        foreach ($sessionFile in $sessionFiles) {
            $lines = @(Get-Content -Path $sessionFile.FullName -ErrorAction SilentlyContinue)
            foreach ($line in $lines) {
                if ([string]::IsNullOrWhiteSpace($line)) {
                    continue
                }

                $obj = $null
                try {
                    $obj = $line | ConvertFrom-Json -ErrorAction Stop
                } catch {
                    continue
                }

                if (($obj.kind -ne 2) -or (-not $obj.k) -or ($obj.k[0] -ne "requests") -or (-not $obj.v)) {
                    continue
                }

                foreach ($request in $obj.v) {
                    if (-not $request.requestId -or -not $request.message -or [string]::IsNullOrWhiteSpace($request.message.text)) {
                        continue
                    }

                    $reqId = [string]$request.requestId
                    if ($processedRequestIds.Contains($reqId)) {
                        continue
                    }

                    [void]$processedRequestIds.Add($reqId)

                    $when = Get-Date
                    if ($request.timestamp) {
                        try {
                            $when = [DateTimeOffset]::FromUnixTimeMilliseconds([int64]$request.timestamp).LocalDateTime
                        } catch {
                        }
                    }

                    $timestamp = $when.ToString("yyyy-MM-dd HH:mm:ss.fff")
                    $entry = "[$timestamp] [CopilotPrompt]`r`n$($request.message.text)`r`n----------------`r`n"
                    Add-Content -Path $logPath -Value $entry -Force
                    Write-Host "Logged prompt: $reqId" -ForegroundColor Green
                }
            }
        }

        $ids = @($processedRequestIds.ToArray())
        @{ requestIds = $ids } | ConvertTo-Json | Set-Content -Path $statePath -Encoding UTF8
        Start-Sleep -Seconds $PollSeconds
    }
    catch {
        Start-Sleep -Seconds $PollSeconds
    }
}
