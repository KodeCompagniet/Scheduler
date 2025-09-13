# DemiCode Scheduler Service - Deployment Guide

## Overview

The DemiCode Scheduler Service has been modernized for .NET 8, replacing the legacy .NET Framework installer components with a modern hosting model using `Microsoft.Extensions.Hosting`. The service can now run in multiple modes:

- **Windows Service** - For production deployment on Windows servers
- **Console Application** - For development and debugging
- **Docker Container** - For containerized deployments (future enhancement)

## Prerequisites

- .NET 8.0 Runtime or SDK installed
- Windows 10/11 or Windows Server 2016+ (for Windows Service mode)
- Administrator privileges (for service installation)

## Building the Service

```bash
# Build for Release
dotnet build -c Release

# Or build and publish as self-contained executable
dotnet publish -c Release -r win-x64 --self-contained
```

## Deployment Options

### Option 1: Windows Service Deployment (Recommended for Production)

#### Installation

1. Build the project in Release mode
2. Navigate to the output directory: `bin\Release\net8.0-windows\`
3. Run the installation script as Administrator:

```powershell
# Basic installation with default settings
.\Scripts\Install-Service.ps1

# Custom installation with specific parameters
.\Scripts\Install-Service.ps1 -ServiceName "MyScheduler" `
                                -DisplayName "My Company Scheduler" `
                                -Description "Custom task scheduler service" `
                                -StartupType "Automatic"

# Install with specific service account
.\Scripts\Install-Service.ps1 -ServiceAccount "domain\serviceuser" `
                                -Password "SecurePassword123"
```

#### Managing the Service

Use the provided management script:

```powershell
# Check service status
.\Scripts\Manage-Service.ps1 status

# Start the service
.\Scripts\Manage-Service.ps1 start

# Stop the service
.\Scripts\Manage-Service.ps1 stop

# Restart the service
.\Scripts\Manage-Service.ps1 restart

# View recent logs
.\Scripts\Manage-Service.ps1 logs
```

Or use Windows Services Manager:
1. Open Services (services.msc)
2. Find "DemiCode Scheduler Service"
3. Right-click to Start, Stop, or configure

#### Uninstallation

Run the uninstall script as Administrator:

```powershell
.\Scripts\Uninstall-Service.ps1
```

### Option 2: Console Application (Development/Testing)

Run directly from the command line:

```bash
# From project directory
dotnet run

# From published directory
.\DemiCode.SchedulerService.exe
```

The service will detect it's running in console mode and:
- Display logs in the console
- Allow graceful shutdown with Ctrl+C
- Enable easier debugging

### Option 3: Manual Service Installation

If you prefer not to use the PowerShell scripts, you can install manually:

```powershell
# Using PowerShell New-Service cmdlet
New-Service -Name "DemiCodeScheduler" `
            -BinaryPathName "C:\Path\To\DemiCode.SchedulerService.exe" `
            -DisplayName "DemiCode Scheduler Service" `
            -Description "Runs scheduled tasks based on configuration" `
            -StartupType Manual

# Using sc.exe
sc create DemiCodeScheduler binPath= "C:\Path\To\DemiCode.SchedulerService.exe" `
           DisplayName= "DemiCode Scheduler Service" `
           start= demand
```

## Configuration

### Scheduler Configuration

The scheduler configuration remains in `app.config` for backward compatibility. The service watches this file for changes and automatically reloads when modified.

Example configuration:
```xml
<scheduling defaultScheduler="prod">
  <schedulers>
    <scheduler name="prod">
      <tasks>
        <task type="DemiCode.SchedulerService.MyTask, DemiCode.SchedulerService" 
              message="Hello from scheduled task">
          <scheduleref name="everyMinute" />
        </task>
      </tasks>
      <IntervalSchedule name="everyMinute">
        <interval>00:01:00</interval>
      </IntervalSchedule>
    </scheduler>
  </schedulers>
</scheduling>
```

### Logging Configuration

Logging is configured in `log4net.config`:
- **File Logs**: Written to `logs\schedulerservice.log` with daily rotation
- **Event Log**: Windows Event Log for warnings and errors
- **Console**: When running in console mode

Adjust log levels in `log4net.config`:
```xml
<logger name="DemiCode.SchedulerService">
  <level value="DEBUG" /> <!-- Change to INFO, WARN, ERROR as needed -->
</logger>
```

## Troubleshooting

### Service Won't Start

1. Check Windows Event Viewer for error messages
2. Verify .NET 8 runtime is installed: `dotnet --list-runtimes`
3. Ensure app.config is present and valid
4. Check file permissions on the installation directory
5. Review logs in the `logs` subdirectory

### Configuration Changes Not Applied

1. The service monitors app.config for changes
2. Check logs for configuration errors
3. Ensure the service account has read permissions on config files

### Permission Issues

- Service installation requires Administrator privileges
- The service account needs:
  - Read access to the installation directory
  - Write access to the logs directory
  - Network access if tasks require it

### Debug Mode

To run in debug mode:
1. Stop the Windows Service if running
2. Run from Visual Studio or command line
3. Attach debugger as needed

## Migration from Legacy Service

If migrating from the old .NET Framework service:

1. **Stop and uninstall** the old service using InstallUtil.exe
2. **Copy configuration** from the old app.config
3. **Install the new service** using the provided scripts
4. **Verify tasks** are running as expected
5. **Monitor logs** for any issues

## Security Considerations

- By default, the service runs as LocalSystem
- Consider using a dedicated service account with minimal privileges
- Protect configuration files containing sensitive information
- Regularly review and rotate log files
- Enable Windows Firewall if network access is not required

## Support Scripts

The `Scripts` directory contains PowerShell scripts for service management:

- `Install-Service.ps1` - Install the service with customizable parameters
- `Uninstall-Service.ps1` - Remove the service from the system
- `Manage-Service.ps1` - Start, stop, restart, and check status

All scripts include help documentation. Run with `-?` for parameter details:
```powershell
Get-Help .\Scripts\Install-Service.ps1 -Detailed
```

## Future Enhancements

Planned improvements for future versions:
- Migration to appsettings.json configuration
- Docker container support
- Health check endpoints
- REST API for management
- Integration with modern logging frameworks (Serilog)
- Cross-platform support (Linux/macOS)

## Version History

- **2.0.0** (2025-01-01) - Migrated to .NET 8 with IHostedService
- **1.1.x** - Legacy .NET Framework version with System.Configuration.Install

## Contact

For issues or questions about deployment, please refer to the main project documentation or create an issue in the project repository.