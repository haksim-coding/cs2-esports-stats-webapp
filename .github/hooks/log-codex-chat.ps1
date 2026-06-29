param(
    [string]$LogPath = (Join-Path $PSScriptRoot "agent_log.txt")
)

$payloadText = [Console]::In.ReadToEnd()

try {
    $payload = $payloadText | ConvertFrom-Json -ErrorAction Stop
} catch {
    # A logging failure must never interrupt the Codex turn.
    [Console]::Out.WriteLine("{}")
    exit 0
}

$label = $null
$message = $null

switch ($payload.hook_event_name) {
    "UserPromptSubmit" {
        $label = "CodexUser"
        $message = [string]$payload.prompt
    }
    "Stop" {
        $label = "CodexAssistant"
        $message = [string]$payload.last_assistant_message
    }
}

if ($label -and -not [string]::IsNullOrWhiteSpace($message)) {
    $logDirectory = Split-Path -Parent $LogPath
    if (-not [string]::IsNullOrWhiteSpace($logDirectory)) {
        New-Item -ItemType Directory -Force -Path $logDirectory | Out-Null
    }

    $timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss.fff"
    $sessionId = [string]$payload.session_id
    $turnId = [string]$payload.turn_id
    $entry = "[$timestamp] [$label] [SessionId: $sessionId] [TurnId: $turnId]`r`n$message`r`n----------------`r`n"
    [System.IO.File]::AppendAllText(
        $LogPath,
        $entry,
        [System.Text.UTF8Encoding]::new($false)
    )
}

# Stop hooks require valid JSON on stdout. An empty object means "continue".
[Console]::Out.WriteLine("{}")
