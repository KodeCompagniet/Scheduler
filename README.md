# DemiCode Scheduler

Lightweight, battle-tested scheduling for .NET. DemiCode Scheduler triggers alarms based on flexible schedules (interval, one-shot, weekly, counted) and lets you attach tasks that run on each alarm — either .NET code (methods on your classes) or external processes. Configure everything via .NET configuration files or build it programmatically.

Targets: .NET Standard 2.0


## Features

- Flexible schedules: interval, counted interval, weekly, and one-shot
- Configuration-first or code-first setup
- Attach tasks globally or per named schedule
- Built-in tasks: run .NET methods (`CodeTask`) or external applications (`ApplicationTask`)
- Pluggable logging with console and log4net support
- Small surface area, strong test coverage, production-proven


## Quick Start (Code‑First)

Run code on a 5-second interval, attach a .NET method and an external process, and listen to alarms.

```csharp
using System;
using DemiCode.Scheduling;
using DemiCode.Scheduling.Schedules;
using DemiCode.Scheduling.Tasks;

class Program
{
    static void Main()
    {
        using var scheduler = SchedulerFactory.Create();

        // 1) Add schedules
        scheduler.Add(new IntervalSchedule("Every5Sec", DateTime.Now, TimeSpan.FromSeconds(5)));

        // 2) Handle alarms
        scheduler.ScheduledAlarm += (s, e) =>
            Console.WriteLine($"Alarm at {e.SignalTime:O}, actual {e.ActualSignalTime:O}");

        // 3) Attach tasks
        // Run a method on your class when any alarm fires
        scheduler.Tasks.Add(new CodeTask(typeof(MyJob), nameof(MyJob.Run), "hello"));

        // Run a process only for a specific schedule
        scheduler.Tasks.Add(new ApplicationTask("/usr/bin/echo", "scheduled run"), "Every5Sec");

        // 4) Start
        scheduler.Start();

        Console.WriteLine("Press Enter to exit...");
        Console.ReadLine();
        scheduler.Stop();
    }
}

public class MyJob
{
    // Non-static methods require a parameterless ctor
    public void Run(string arg) => Console.WriteLine($"Running MyJob with '{arg}'");
}
```

Things to note:

- Add schedules before starting; adding while running throws `InvalidOperationException`.
- Attach tasks globally (fires on any alarm) or to a named schedule only.
- `CodeTask` validates your method signature up front and disposes its instance if `IDisposable`.


## Quick Start (Configuration‑First)

Add this to your app config and bootstrap a scheduler from configuration.

```xml
<!-- app.config or appsettings.{Environment}.config -->
<configuration>
  <configSections>
    <section name="scheduling"
             type="DemiCode.Scheduling.Configuration.SchedulerSettings, DemiCode.Scheduling, Culture=neutral, PublicKeyToken=069d10bcdee4c3e4" />
  </configSections>

  <scheduling defaultScheduler="prod">
    <schedulers>
      <scheduler name="prod">
        <!-- Built-in schedules -->
        <IntervalSchedule name="Every5Sec">
          <start>2025-01-01 00:00:00</start>
          <interval>00:00:05</interval>
        </IntervalSchedule>
        <WeeklySchedule name="FridayAt22">
          <fromDate>2025-01-01</fromDate>
          <toDate>2025-12-31</toDate>
          <timeOfDay>22:00:00</timeOfDay>
          <days>
            <day>friday</day>
          </days>
        </WeeklySchedule>

        <!-- Tasks (global or schedule-specific via <scheduleref/>) -->
        <tasks>
          <CodeTask class="MyApp.Jobs.CleanupJob, MyApp" method="Run">
            <param0>foo</param0>
          </CodeTask>

          <ApplicationTask path="C:\tools\backup.exe">
            <arg0>-q</arg0>
          </ApplicationTask>

          <!-- Attach a task only to a named schedule -->
          <task type="MyApp.Jobs.NotifyJob, MyApp">
            <scheduleref name="Every5Sec" />
          </task>
        </tasks>

        <!-- Logging (Console or log4net; or a concrete factory via <logger type=.../>) -->
        <loggers>
          <ConsoleLogger />
          <!-- or <Log4netLogger /> -->
        </loggers>
      </scheduler>
    </schedulers>
  </scheduling>
</configuration>
```

Bootstrap and start:

```csharp
using DemiCode.Scheduling;

using var scheduler = SchedulerFactory.CreateFromConfig("scheduling");
// Optionally: SchedulerFactory.CreateFromConfig("scheduling", reloadConfigFromDisk: true);
// Or from an explicit .config file: SchedulerFactory.CreateFromConfigFile("MyApp.exe.config", "scheduling");

// Schedule-specific event handlers
scheduler["Every5Sec"].Alarm += (s, e) => Console.WriteLine("Tick!");

scheduler.Start();
```


## Common Recipes

- Add multiple schedules at once:
  ```csharp
  scheduler.AddRange(new[]
  {
      new OneShotSchedule("Now", DateTime.Now),
      new IntervalCountedSchedule("Burst", DateTime.Now.AddSeconds(1), 10, TimeSpan.FromSeconds(1))
  });
  ```

- Attach a sequence of tasks to run in order:
  ```xml
  <TaskSequence>
    <tasks>
      <CodeTask class="MyApp.Jobs.First, MyApp" method="Run" />
      <CodeTask class="MyApp.Jobs.Second, MyApp" method="Run" />
      <ApplicationTask path="C:\tools\done.exe" />
    </tasks>
  </TaskSequence>
  ```


## Project Structure

- `DemiCode.Scheduling`: core scheduler, schedules, tasks, configuration
- `DemiCode.Logging` and `DemiCode.Logging.log4net`: logging abstractions and log4net adapter
- `DemiCode.SchedulerService`: example Windows service host
- `Samples/`: runnable C# and VB.NET examples
- `DemiCode.Scheduling.Test`: NUnit test suite showing real usage patterns


## Build and Test

- Build the solution:
  - `dotnet build DemiCode.Scheduling.sln`

- Run tests:
  - `dotnet test DemiCode.Scheduling.sln`

The core library builds for `netstandard2.0`.


## Notes and Tips

- Removing/replacing tasks at runtime is intentionally not supported; define the set up front.
- For non-static `CodeTask` methods, your class must have a parameterless constructor.
- Use `scheduler.Loggers.Add(...)` to add a logger factory programmatically when not using config.


## License

MIT — see `LICENSE`.

