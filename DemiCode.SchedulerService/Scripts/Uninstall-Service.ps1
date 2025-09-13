# Uninstall-Service.ps1
# PowerShell script to uninstall DemiCode Scheduler Service from Windows
# Run this script as Administrator

param(
    [Parameter(Mandatory=$false)]
    [string]$ServiceName = "DemiCodeScheduler"
)

# Ensure running as Administrator
if (-NOT ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole] "Administrator"))
{
    Write-Error "This script must be run as Administrator. Exiting..."
    exit 1
}

Write-Host "Uninstalling DemiCode Scheduler Service..." -ForegroundColor Green

# Check if service exists
$service = Get-Service -Name $ServiceName -ErrorAction SilentlyContinue

if (-not $service) {
    Write-Warning "Service '$ServiceName' not found. Nothing to uninstall."
    exit 0
}

# Stop the service if it's running
if ($service.Status -eq 'Running') {
    Write-Host "Stopping service '$ServiceName'..." -ForegroundColor Yellow
    try {
        Stop-Service -Name $ServiceName -Force -ErrorAction Stop
        Write-Host "Service stopped successfully." -ForegroundColor Green
        
        # Wait for service to fully stop
        $timeout = 30
        $timer = 0
        while ((Get-Service -Name $ServiceName).Status -ne 'Stopped' -and $timer -lt $timeout) {
            Start-Sleep -Seconds 1
            $timer++
        }
        
        if ($timer -ge $timeout) {
            Write-Warning "Service did not stop within $timeout seconds."
        }
    } catch {
        Write-Warning "Failed to stop service: $_"
        Write-Warning "Proceeding with removal anyway..."
    }
}

# Remove the service
Write-Host "Removing service '$ServiceName'..." -ForegroundColor Yellow
try {
    sc.exe delete $ServiceName | Out-Null
    
    # Verify removal
    Start-Sleep -Seconds 2
    $checkService = Get-Service -Name $ServiceName -ErrorAction SilentlyContinue
    
    if (-not $checkService) {
        Write-Host "Service '$ServiceName' uninstalled successfully!" -ForegroundColor Green
    } else {
        Write-Warning "Service appears to still exist. You may need to restart the computer to complete the removal."
    }
} catch {
    Write-Error "Failed to uninstall service: $_"
    exit 1
}

Write-Host "`nUninstallation completed!" -ForegroundColor Green