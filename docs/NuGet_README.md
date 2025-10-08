# DemiCode Scheduler

A robust, lightweight job scheduling framework for .NET applications. Schedule and run tasks at specific intervals or times with full cross-platform support.

## Features

- **Multiple Schedule Types**: Daily, weekly, monthly, interval-based, and custom schedules
- **Task Types**: Application tasks (executables), code tasks (methods), and custom implementations
- **Cross-Platform**: Works on Windows, Linux, and macOS (.NET Standard 2.0+ and .NET Framework 4.8)
- **Thread-Safe**: Built-in thread safety with proper locking and state management
- **Flexible Configuration**: XML or programmatic configuration
- **Extensible**: Easy to create custom schedules and task types
- **Strong-Named**: All assemblies are strong-named for GAC deployment

## Quick Start

### Installation

```bash
dotnet add package DemiCode.Scheduling
```

For log4net integration:
```bash
dotnet add package DemiCode.Logging.log4net
```

### Basic Usage

```csharp
using DemiCode.Scheduling;
using DemiCode.Scheduling.Schedules;
using DemiCode.Scheduling.Tasks;

// Create a scheduler
var scheduler = new Scheduler();

// Create a task
var task = new ApplicationTask("notepad.exe");

// Create a schedule (run every 5 minutes)
var schedule = new IntervalSchedule(TimeSpan.FromMinutes(5));

// Create and add a job
var job = new Job("MyJob", task, schedule);
scheduler.Jobs.Add(job);

// Start the scheduler
scheduler.Start();

// Stop when done
scheduler.Stop();
```

### Configuration Example

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <configSections>
    <section name="scheduler" type="DemiCode.Scheduling.SchedulerSection, DemiCode.Scheduling"/>
  </configSections>

  <scheduler>
    <job name="Backup" type="DemiCode.Scheduling.Job">
      <task type="DemiCode.Scheduling.Tasks.ApplicationTask">
        <constructor>
          <parameter type="System.String">backup.exe</parameter>
        </constructor>
      </task>
      <schedule type="DemiCode.Scheduling.Schedules.DailySchedule">
        <constructor>
          <parameter type="System.TimeSpan">02:00:00</parameter>
        </constructor>
      </schedule>
    </job>
  </scheduler>
</configuration>
```

## Package Structure

- **DemiCode.Shared**: Core utilities and base types
- **DemiCode.Scheduling**: Main scheduling engine, schedules, and tasks
- **DemiCode.Logging.log4net**: log4net integration for scheduler logging

## Version 2.0 Breaking Changes

### Logger Namespace Consolidation
- Consolidated all logging types into `DemiCode.Logging` namespace
- Update: `using DemiCode.Logging.Loggers;` → `using DemiCode.Logging;`

### ApplicationTask Shell Execution
- Changed to direct process execution (no shell)
- For shell features, explicitly invoke the shell:
  - Windows: `new ApplicationTask("cmd.exe", "/c", "your command")`
  - Linux/macOS: `new ApplicationTask("/bin/sh", "-c", "your command")`

## Documentation

For complete documentation, examples, and migration guides, visit:
[https://github.com/KodeCompagniet/demicode-scheduling](https://github.com/KodeCompagniet/demicode-scheduling)

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Support

- Report issues: [GitHub Issues](https://github.com/KodeCompagniet/demicode-scheduling/issues)
- Discussions: [GitHub Discussions](https://github.com/KodeCompagniet/demicode-scheduling/discussions)