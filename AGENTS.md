# DemiCode Scheduler - LLM Agent Reference

**Last Updated**: 2025-10-04
**Project Type**: .NET Scheduling Library
**Target Frameworks**: .NET Standard 2.0, .NET Framework 4.8, .NET 8.0 (tests)

## Quick Facts

- **Main Language**: C# 7.3
- **Build System**: .NET SDK / MSBuild
- **Test Framework**: NUnit 3.14
- **Solution File**: `DemiCode.Scheduling.sln`
- **Main Code**: Zero compiler warnings (enforced via `TreatWarningsAsErrors`)
- **Code Style**: Configured via `.editorconfig` in root

## Project Structure

```
DemiCode.Scheduling.sln
├── DemiCode.Shared/              # Shared utilities (config, factories, logging abstractions)
├── DemiCode.Scheduling/          # Core scheduler library
├── DemiCode.Logging/             # Logging abstractions
├── DemiCode.Logging.log4net/     # log4net adapter
├── DemiCode.SchedulerService/    # Windows service host (net8.0)
├── DemiCode.Shared.Test/         # Unit tests for shared components
├── DemiCode.Logging.Test/        # Unit tests for logging
├── DemiCode.Scheduling.Test/     # Core scheduler tests (487 tests)
├── DemiCode.Scheduling.Test.Dummy/ # Test helper project
├── Samples/                      # Example applications (C#, VB.NET)
└── Testprojects/                 # Development test projects
```

## Build & Test Commands

```bash
# Full build
dotnet build DemiCode.Scheduling.sln

# Clean build
dotnet build DemiCode.Scheduling.sln --no-incremental

# Run all tests
dotnet test DemiCode.Scheduling.sln

# Run tests with detailed output
dotnet test DemiCode.Scheduling.sln --verbosity normal

# Format code
dotnet format DemiCode.Scheduling.sln
```

## Key Configuration Files

- **Directory.Build.props** - Solution-wide MSBuild properties
  - Sets `LangVersion`, signing, documentation generation
  - Enforces `TreatWarningsAsErrors` for main projects (not tests)
  - Suppresses CS1591 (missing XML docs warnings)

- **.editorconfig** - Code style settings (indent size, naming conventions, etc.)

- **AssemblyInfo.Common.cs** - Shared assembly attributes

- **AssemblyInfo.Version.cs** - Version information (currently 1.1.6.0)

- **DemiCodeStrongName.snk** - Strong name key for assembly signing

## Important Code Patterns

### Multi-Targeting
Most libraries target both `netstandard2.0` and `net48`:
```xml
<TargetFrameworks>netstandard2.0;net48</TargetFrameworks>
```

### Configuration-Dependent References
Projects conditionally reference System.Configuration based on target framework:
```xml
<ItemGroup Condition="'$(TargetFramework)' == 'net48'">
  <Reference Include="System.Configuration" />
</ItemGroup>
<ItemGroup Condition="'$(TargetFramework)' == 'netstandard2.0'">
  <PackageReference Include="System.Configuration.ConfigurationManager" Version="8.0.0" />
</ItemGroup>
```

### GetHashCode Implementation
When overriding `Equals()`, always implement `GetHashCode()` using this pattern for .NET Standard 2.0 compatibility:
```csharp
public override int GetHashCode()
{
    unchecked
    {
        int hash = base.GetHashCode();
        hash = (hash * 397) ^ (field?.GetHashCode() ?? 0);
        return hash;
    }
}
```
Do NOT use `HashCode.Combine()` (not available in netstandard2.0).

## Known Issues & Special Cases

### 1. MonthlySchedule Bug (Documented in MONTHLY_SCHEDULE_BUG.md)
- **Location**: `DemiCode.Scheduling/Schedules/MonthlySchedule.cs:91-96`
- **Status**: Bug is **intentionally preserved** for backward compatibility
- **Issue**: Implementation doesn't match design documentation
- **Action**: Do NOT fix without explicit approval. Tests are written to match buggy behavior.

### 2. Obsolete APIs in Tests
Test projects intentionally use obsolete APIs to verify backward compatibility:
- `Scheduler.CreateFromConfig()` - Use `SchedulerFactory.CreateFromConfig()` in new code
- `TaskCollection.Add(ITask, IDictionary)` - IDictionary parameter is obsolete
- `TaskHandler(ITask, IDictionary, ILoggerFactory)` - IDictionary parameter is obsolete
- XML-based schedule constructors - Use configuration system instead

These obsolete usages in **test code** are intentional and should not be removed.

### 3. Test Project Warnings
Test projects have 21 expected warnings (obsolete APIs, security attributes). These are normal and intentional.

## Testing Notes

- **Total Tests**: 487 (71 shared, 47 logging, 369 scheduling)
- **Skipped Tests**: 18 (platform-specific or require manual setup)
- **Test Status**: All passing as of 2025-10-04
- **Test Helpers**: Mock classes in `DemiCode.Scheduling.Test/Mocks/`
- **Test Data**: XML configuration files in `DemiCode.Scheduling.Test/TestData/`

### Running Specific Tests
```bash
# Run only scheduling tests
dotnet test DemiCode.Scheduling.Test/DemiCode.Scheduling.Test.csproj

# Run specific test
dotnet test --filter "FullyQualifiedName~MonthlyScheduleTest"

# Run with code coverage
dotnet test /p:CollectCoverage=true
```

## Code Quality Standards

1. **No warnings in main code** - `TreatWarningsAsErrors` is enabled for non-test projects
2. **All public APIs must have XML documentation** - Or suppress with `<NoWarn>CS1591</NoWarn>`
3. **Equals() must have GetHashCode()** - Use unchecked arithmetic pattern for netstandard2.0
4. **Configuration classes must be testable** - All config elements override `Equals()` and `GetHashCode()`
5. **Strong naming required** - All assemblies are signed with `DemiCodeStrongName.snk`

## Architecture Notes

### Scheduler Lifecycle
1. Create scheduler via `SchedulerFactory.Create()` or `CreateFromConfig()`
2. Add schedules (must be done before starting)
3. Optionally add tasks and event handlers
4. Call `Start()` to begin scheduling
5. Call `Stop()` or `Dispose()` to clean up

### Schedule Types
- `IntervalSchedule` - Repeating interval (e.g., every 5 seconds)
- `IntervalCountedSchedule` - Limited repetitions (e.g., 10 times every 5 seconds)
- `WeeklySchedule` - Specific days/times (e.g., Monday at 9:00 AM)
- `OneShotSchedule` - Single occurrence
- `MonthlySchedule` - Monthly occurrence (HAS KNOWN BUG - see above)

### Task Types
- `CodeTask` - Invokes a .NET method
- `ApplicationTask` - Launches external process
- `TaskSequence` - Executes multiple tasks in order

### Logging
- Abstraction: `ILogger`, `ILoggerFactory` in DemiCode.Logging
- Implementations: ConsoleLogger (built-in), Log4netLogger (adapter)
- Configuration: Via XML config or `scheduler.Loggers.Add()`

## Migration History

This project was migrated from .NET Framework to multi-target (.NET Standard 2.0 + .NET Framework 4.8) in 2025. See migration documentation:
- `MIGRATION_STATUS.md` - Overall migration status
- `MONTHLY_SCHEDULE_BUG.md` - Known issue documentation
- `migration.md` - Detailed migration notes

## Common Agent Tasks

### Adding a GetHashCode() Method
```csharp
// For simple value-based classes
public override int GetHashCode()
{
    return base.GetHashCode();
}

// For classes with fields
public override int GetHashCode()
{
    unchecked
    {
        int hash = base.GetHashCode();
        hash = (hash * 397) ^ (field1?.GetHashCode() ?? 0);
        hash = (hash * 397) ^ (field2?.GetHashCode() ?? 0);
        return hash;
    }
}
```

### Adding XML Documentation
```csharp
/// <summary>
/// Brief description of what this does.
/// </summary>
/// <param name="paramName">Parameter description</param>
/// <returns>Return value description</returns>
/// <exception cref="ExceptionType">When this exception is thrown</exception>
public void MethodName(Type paramName)
{
    // ...
}
```

### Fixing cref Warnings
Use fully qualified names or ensure types are in scope:
```csharp
// ❌ Ambiguous
/// <see cref="GetMethod"/>

// ✅ Specific
/// <see cref="GetMethod(string, string)"/>

// ✅ Fully qualified
/// <see cref="DemiCode.Shared.Factories.FactoryException"/>
```

## Contact & Resources

- **GitHub Issues**: https://github.com/anthropics/claude-code/issues (for reporting agent problems)
- **Documentation**: See README.md for user-facing docs
- **Test Examples**: DemiCode.Scheduling.Test contains comprehensive usage examples

## Version Information

- **Current Version**: 1.1.6.0 (preparing for release)
- **Previous Version**: 1.1.5.0
- **Compatibility**: Targets .NET Standard 2.0 for broad compatibility
