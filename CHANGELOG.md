# Changelog

All notable changes to DemiCode Scheduler will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [2.0.0] - 2025-10-04

### Breaking Changes

#### Logger Namespace Consolidation
- **Removed** duplicate `DemiCode.Logging.Loggers` namespace
- **Consolidated** all logging types into `DemiCode.Logging` namespace
- **Added** type forwarders in `DemiCode.Logging.dll` to maintain binary compatibility
- **Migration:** Update `using DemiCode.Logging.Loggers;` to `using DemiCode.Logging;`
- **Impact:** Simple find-replace operation, no other code changes needed
- **Binary Compatibility:** Existing binaries that reference `DemiCode.Logging` continue to work via type forwarders

#### ApplicationTask Shell Execution
- **Changed** `ProcessStartInfo.UseShellExecute` from `true` to `false`
- **Removed** support for shell features (pipes, redirection, variable expansion) in `ApplicationTask`
- **Migration:** To use shell features, explicitly invoke the shell:
  - Windows: `new ApplicationTask("cmd.exe", "/c", "your command")`
  - Linux/macOS: `new ApplicationTask("/bin/sh", "-c", "your command")`
- **Benefit:** Reliable cross-platform argument handling, better security, consistent behavior

### Fixed

#### MonthlySchedule Implementation Corrected
- **Fixed** `MonthlySchedule` now correctly implements monthly scheduling on the first day of each month
- **Fixed** `DateTimeHelper.GetFirstOfMonth` properly returns the 1st of the month (not preserving the original day)
- **Fixed** `MonthlySchedule` no longer emits extra occurrences past the configured `toDate`
- **Behavior:** Monthly schedules now run on the same day each month (the day from `_fromDate`)
- **Impact:** Implementation now matches the original design intent and class name
- **Location:** `DemiCode.Scheduling/DateTimeHelper.cs:24-32`, `DemiCode.Scheduling/Schedules/MonthlySchedule.cs:85-137`
- **Tests:** Comprehensive test coverage in `DemiCode.Scheduling.Test/Schedules/MonthlyScheduleTest.cs`

#### CodeTask Static Method Support
- **Fixed** `CodeTask` throwing reflection errors when invoking static methods on types without parameterless constructors
- **Enhancement:** Static methods are now detected via `MethodInfo.IsStatic` and skip object instantiation
- **Impact:** Utility classes with static-only methods can now be scheduled without dummy constructors
- **Location:** `DemiCode.Scheduling/Tasks/CodeTask.cs:85-95`

### Added

#### ApplicationTask Cross-Platform Argument Handling
- **Added** `ProcessStartInfo.ArgumentList` support for .NET Core 2.1+ / .NET 5+
- **Added** `QuoteArguments()` fallback for .NET Standard 2.0 and .NET Framework
- **Added** Proper escaping for arguments containing spaces, quotes, and backslashes
- **Added** `ArgumentNullException` check for `loggerFactory` parameter (preserves 1.x behavior)
- **Added** 14 new comprehensive unit tests for argument handling edge cases
- **Location:** `DemiCode.Scheduling/Tasks/ApplicationTask.cs`
- **Note:** stdout/stderr are not redirected to avoid process blocking when buffers fill

### Changed

#### Test Coverage
- **Added** static method invocation tests in `CodeTaskTest.cs`
- **Added** `MonthlySchedule` boundary condition tests
- **Added** `ApplicationTask` argument quoting and escaping tests
- **Result:** All 508 tests passing (71 shared + 47 logging + 390 scheduling)

### Documentation

- **Updated** `README.md` with ApplicationTask shell behavior notes
- **Created** `MIGRATION_TO_2.0.md` migration guide
- **Updated** `AGENTS.md` with version 2.0.0 information
- **Created** `CHANGELOG.md` (this file)

## [1.1.6.0] - Previous Release

(Earlier versions were not tracked in this changelog)

---

**Migration Guide:** See [MIGRATION_TO_2.0.md](MIGRATION_TO_2.0.md) for detailed migration instructions.
