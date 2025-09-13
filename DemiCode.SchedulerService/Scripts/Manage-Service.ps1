# Manage-Service.ps1
# PowerShell script to manage DemiCode Scheduler Service
# Run this script as Administrator for start/stop/restart operations

param(
    [Parameter(Mandatory=$true, Position=0)]
    [ValidateSet("start", "stop", "restart", "status", "logs")]
    [string]$Action,
    
    [Parameter(Mandatory=$false)]
    [string]$ServiceName = "DemiCodeScheduler"
)

# Function to check if running as Administrator
function Test-Administrator {
    $currentUser = [Security.Principal.WindowsIdentity]::GetCurrent()
    $principal = New-Object Security.Principal.WindowsPrincipal($currentUser)
    return $principal.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)
}

# Function to get service status
function Get-ServiceStatus {
    $service = Get-Service -Name $ServiceName -ErrorAction SilentlyContinue
    
    if (-not $service) {
        Write-Host "Service '$ServiceName' not found." -ForegroundColor Red
        Write-Host "Run Install-Service.ps1 to install the service first." -ForegroundColor Yellow
        return $null
    }
    
    Write-Host "`nService Information:" -ForegroundColor Cyan
    Write-Host "  Name: $($service.Name)"
    Write-Host "  Display Name: $($service.DisplayName)"
    Write-Host "  Status: $($service.Status)" -ForegroundColor $(if ($service.Status -eq 'Running') { 'Green' } else { 'Yellow' })
    Write-Host "  Startup Type: $($service.StartType)"
    
    return $service
}

# Function to view logs
function Show-Logs {
    Write-Host "`nChecking for recent DemiCode Scheduler events..." -ForegroundColor Cyan
    
    try {
        $events = Get-EventLog -LogName Application -Source "DemiCode Scheduler Service" -Newest 20 -ErrorAction SilentlyContinue
        
        if ($events) {
            Write-Host "Recent log entries:" -ForegroundColor Green
            $events | Format-Table TimeGenerated, EntryType, Message -AutoSize -Wrap
        } else {
            Write-Host "No recent events found in Application log." -ForegroundColor Yellow
            Write-Host "The service may be logging to a file. Check the application directory for log files." -ForegroundColor Yellow
        }
    } catch {
        Write-Warning "Could not retrieve event logs. The service may not have logged any events yet."
    }
}

# Main script logic
switch ($Action) {
    "status" {
        Get-ServiceStatus | Out-Null
    }
    
    "start" {
        if (-not (Test-Administrator)) {
            Write-Error "Starting the service requires Administrator privileges. Please run as Administrator."
            exit 1
        }
        
        $service = Get-ServiceStatus
        if ($service) {
            if ($service.Status -eq 'Running') {
                Write-Host "`nService is already running." -ForegroundColor Yellow
            } else {
                Write-Host "`nStarting service..." -ForegroundColor Green
                try {
                    Start-Service -Name $ServiceName -ErrorAction Stop
                    Start-Sleep -Seconds 2
                    $service = Get-Service -Name $ServiceName
                    if ($service.Status -eq 'Running') {
                        Write-Host "Service started successfully!" -ForegroundColor Green
                    } else {
                        Write-Warning "Service may not have started properly. Current status: $($service.Status)"
                    }
                } catch {
                    Write-Error "Failed to start service: $_"
                }
            }
        }
    }
    
    "stop" {
        if (-not (Test-Administrator)) {
            Write-Error "Stopping the service requires Administrator privileges. Please run as Administrator."
            exit 1
        }
        
        $service = Get-ServiceStatus
        if ($service) {
            if ($service.Status -eq 'Stopped') {
                Write-Host "`nService is already stopped." -ForegroundColor Yellow
            } else {
                Write-Host "`nStopping service..." -ForegroundColor Green
                try {
                    Stop-Service -Name $ServiceName -Force -ErrorAction Stop
                    Start-Sleep -Seconds 2
                    $service = Get-Service -Name $ServiceName
                    if ($service.Status -eq 'Stopped') {
                        Write-Host "Service stopped successfully!" -ForegroundColor Green
                    } else {
                        Write-Warning "Service may not have stopped properly. Current status: $($service.Status)"
                    }
                } catch {
                    Write-Error "Failed to stop service: $_"
                }
            }
        }
    }
    
    "restart" {
        if (-not (Test-Administrator)) {
            Write-Error "Restarting the service requires Administrator privileges. Please run as Administrator."
            exit 1
        }
        
        $service = Get-ServiceStatus
        if ($service) {
            Write-Host "`nRestarting service..." -ForegroundColor Green
            try {
                Restart-Service -Name $ServiceName -Force -ErrorAction Stop
                Start-Sleep -Seconds 3
                $service = Get-Service -Name $ServiceName
                if ($service.Status -eq 'Running') {
                    Write-Host "Service restarted successfully!" -ForegroundColor Green
                } else {
                    Write-Warning "Service may not have restarted properly. Current status: $($service.Status)"
                }
            } catch {
                Write-Error "Failed to restart service: $_"
            }
        }
    }
    
    "logs" {
        Show-Logs
    }
}