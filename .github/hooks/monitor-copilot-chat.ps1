param(
    [string]$LogFile = "agent_log.txt",
    [int]$PollSeconds = 3
)

$logPath = if ([System.IO.Path]::IsPathRooted($LogFile)) {
    $LogFile
} else {
    Join-Path $PSScriptRoot $LogFile
}

$logDirectory = Split-Path -Parent $logPath
if (-not [string]::IsNullOrWhiteSpace($logDirectory) -and -not (Test-Path $logDirectory)) {
    New-Item -ItemType Directory -Path $logDirectory -Force | Out-Null
}

if (-not (Test-Path $logPath)) {
    New-Item -ItemType File -Path $logPath -Force | Out-Null
}

$statePath = Join-Path $PSScriptRoot "monitor-state.json"
$processedRequestIds = New-Object 'System.Collections.Generic.HashSet[string]'
$stateLoaded = $false
$mutex = New-Object System.Threading.Mutex($false, 'Global\Cs2EsportsCopilotChatLogger')
$hasLock = $false

try {
    $hasLock = $mutex.WaitOne(0)
    if (-not $hasLock) {
        Write-Host "Copilot Chat logger is already running." -ForegroundColor Yellow
        exit 0
    }
} catch {
    Write-Host "Failed to acquire logger lock." -ForegroundColor Yellow
    exit 0
}

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
            $stateLoaded = $true
        }
    } catch {
    }
}

# On first run, seed request IDs from existing sessions without logging old history.
if (-not $stateLoaded) {
    $storageDir = "$env:APPDATA\Code\User\workspaceStorage"
    $sessionFiles = @(Get-ChildItem -Path $storageDir -Recurse -Filter "*.jsonl" -ErrorAction SilentlyContinue | Where-Object { $_.FullName -match "chatSessions\\.*\.jsonl$" })
    foreach ($sessionFile in $sessionFiles) {
        $lines = @(Get-Content -Path $sessionFile.FullName -ErrorAction SilentlyContinue)
        foreach ($line in $lines) {
            if ([string]::IsNullOrWhiteSpace($line)) {
                continue
            }

            try {
                $obj = $line | ConvertFrom-Json -ErrorAction Stop
            } catch {
                continue
            }

            if ($obj.kind -eq 2 -and $obj.k -and $obj.k[0] -eq "requests" -and $obj.v) {
                foreach ($request in @($obj.v)) {
                    if ($request.requestId) {
                        [void]$processedRequestIds.Add([string]$request.requestId)
                    }
                }
            }
        }
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

                $requestItems = @()
                if ($obj.kind -eq 2 -and $obj.k -and $obj.k[0] -eq "requests" -and $obj.v) {
                    $requestItems = @($obj.v)
                } elseif ($obj.message -and $obj.message.text) {
                    $requestItems = @($obj)
                }

                foreach ($request in $requestItems) {
                    $requestText = $null
                    if ($request.message -and $request.message.text) {
                        $requestText = [string]$request.message.text
                    } elseif ($request.text) {
                        $requestText = [string]$request.text
                    }

                    if ([string]::IsNullOrWhiteSpace($requestText)) {
                        continue
                    }

                    $reqId = if ($request.requestId) { [string]$request.requestId } else { "$($sessionFile.FullName):$($requestText.GetHashCode())" }
                    if ($processedRequestIds.Contains($reqId)) {
                        continue
                    }

                    $when = Get-Date
                    if ($request.timestamp) {
                        try {
                            $when = [DateTimeOffset]::FromUnixTimeMilliseconds([int64]$request.timestamp).LocalDateTime
                        } catch {
                        }
                    }

                    $timestamp = $when.ToString("yyyy-MM-dd HH:mm:ss.fff")
                    $entry = "[$timestamp] [CopilotPrompt] [RequestId: $reqId]`r`n$requestText`r`n----------------`r`n"
                    Add-Content -Path $logPath -Value $entry -Force
                    [void]$processedRequestIds.Add($reqId)
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

if ($hasLock) {
    $mutex.ReleaseMutex() | Out-Null
}
