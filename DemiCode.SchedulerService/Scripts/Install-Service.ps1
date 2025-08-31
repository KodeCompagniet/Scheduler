# Install-Service.ps1
# PowerShell script to install DemiCode Scheduler Service on Windows
# Run this script as Administrator

param(
    [Parameter(Mandatory=$false)]
    [string]$ServiceName = "DemiCodeScheduler",
    
    [Parameter(Mandatory=$false)]
    [string]$DisplayName = "DemiCode Scheduler Service",
    
    [Parameter(Mandatory=$false)]
    [string]$Description = "DemiCode Scheduler Service - Runs scheduled tasks based on configuration",
    
    [Parameter(Mandatory=$false)]
    [string]$StartupType = "Manual",  # Manual, Automatic, Disabled
    
    [Parameter(Mandatory=$false)]
    [string]$ServiceAccount = "LocalSystem",  # LocalSystem, LocalService, NetworkService, or domain\username
    
    [Parameter(Mandatory=$false)]
    [string]$Password = ""
)

# Ensure running as Administrator
if (-NOT ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole] "Administrator"))
{
    Write-Error "This script must be run as Administrator. Exiting..."
    exit 1
}

# Get the directory where the script is located
$scriptPath = Split-Path -Parent $MyInvocation.MyCommand.Path
$servicePath = Join-Path (Split-Path -Parent $scriptPath) "DemiCode.SchedulerService.exe"

# Check if the executable exists
if (-not (Test-Path $servicePath)) {
    Write-Error "Service executable not found at: $servicePath"
    Write-Error "Please build the project first using: dotnet build -c Release"
    exit 1
}

Write-Host "Installing DemiCode Scheduler Service..." -ForegroundColor Green
Write-Host "Service executable: $servicePath"

# Check if service already exists
$existingService = Get-Service -Name $ServiceName -ErrorAction SilentlyContinue

if ($existingService) {
    Write-Warning "Service '$ServiceName' already exists."
    $response = Read-Host "Do you want to reinstall it? (Y/N)"
    
    if ($response -eq 'Y' -or $response -eq 'y') {
        Write-Host "Stopping existing service..." -ForegroundColor Yellow
        Stop-Service -Name $ServiceName -Force -ErrorAction SilentlyContinue
        
        Write-Host "Removing existing service..." -ForegroundColor Yellow
        sc.exe delete $ServiceName | Out-Null
        Start-Sleep -Seconds 2
    } else {
        Write-Host "Installation cancelled." -ForegroundColor Yellow
        exit 0
    }
}

# Build the service creation parameters
$params = @{
    Name = $ServiceName
    BinaryPathName = $servicePath
    DisplayName = $DisplayName
    Description = $Description
    StartupType = $StartupType
}

# Handle service account
if ($ServiceAccount -eq "LocalSystem") {
    # LocalSystem is the default, no credentials needed
} elseif ($ServiceAccount -eq "LocalService") {
    $params.Credential = New-Object System.Management.Automation.PSCredential("NT AUTHORITY\LOCAL SERVICE", (New-Object System.Security.SecureString))
} elseif ($ServiceAccount -eq "NetworkService") {
    $params.Credential = New-Object System.Management.Automation.PSCredential("NT AUTHORITY\NETWORK SERVICE", (New-Object System.Security.SecureString))
} elseif ($ServiceAccount -and $Password) {
    $securePassword = ConvertTo-SecureString $Password -AsPlainText -Force
    $params.Credential = New-Object System.Management.Automation.PSCredential($ServiceAccount, $securePassword)
}

# Create the service
try {
    New-Service @params | Out-Null
    Write-Host "Service '$ServiceName' installed successfully!" -ForegroundColor Green
    
    # Display service information
    $service = Get-Service -Name $ServiceName
    Write-Host "`nService Information:" -ForegroundColor Cyan
    Write-Host "  Name: $($service.Name)"
    Write-Host "  Display Name: $($service.DisplayName)"
    Write-Host "  Status: $($service.Status)"
    Write-Host "  Startup Type: $StartupType"
    
    Write-Host "`nTo start the service, run:" -ForegroundColor Yellow
    Write-Host "  Start-Service -Name $ServiceName"
    Write-Host "Or use Windows Services Manager (services.msc)"
    
} catch {
    Write-Error "Failed to install service: $_"
    exit 1
}

Write-Host "`nInstallation completed successfully!" -ForegroundColor Green