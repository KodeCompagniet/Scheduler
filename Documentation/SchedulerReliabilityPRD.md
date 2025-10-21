# Scheduler Reliability & Logging Cleanup PRD

## Stakeholders
- **Owner:** Scheduler/Core Libraries team
- **Stakeholders:** Scheduler maintainers, logging maintainers, QA
- **Goal:** Ship fixes that remove known execution/runtime defects in scheduling tasks, enforce schedule boundaries, and rationalise logging abstractions while retaining cross-platform support.

## Version Strategy

**Target Release:** 2.0.0 (current: 1.1.6.0)

**Breaking Changes:**
- Namespace consolidation: `DemiCode.Logging.Loggers` → `DemiCode.Logging`
- ApplicationTask: `UseShellExecute = false` (was `true`)
- MonthlySchedule boundary behavior corrected (extra occurrence eliminated)

**Migration Guide:** See `MIGRATION_TO_2.0.md` (to be created)

**Rationale:** Semantic versioning requires major version bump due to:
- Binary incompatibility (namespace change in logging abstractions)
- Behavior changes (process execution model, schedule boundary enforcement)
- Public API changes affecting downstream consumers

**Release Timeline:**
- 2.0.0-alpha.1: Internal testing after implementation
- 2.0.0-beta.1: External validation with select consumers
- 2.0.0: Final release with migration documentation

## Problem Statement
1. `CodeTask` treats static targets as instance methods (`DemiCode.Scheduling/Tasks/CodeTask.cs`). The factory call throws for types without a parameterless constructor, preventing static-only utility classes from being scheduled.
2. `MonthlySchedule` emits an extra occurrence past its configured `_toDate` (`DemiCode.Scheduling/Schedules/MonthlySchedule.cs`). Schedules that should end on a date still generate one more alarm.
3. Two logging abstraction stacks exist (`DemiCode.Logging` and `DemiCode.Logging.Loggers`). `Log4netLoggerFactory` implements the wrong interface, forcing casts and risking runtime mismatches when configuration-based factories are composed.
4. `ApplicationTask` manually builds quoted argument strings while keeping `UseShellExecute = true` (`DemiCode.Scheduling/Tasks/ApplicationTask.cs`). Quoting fails on non-Windows hosts when arguments contain spaces or quotes.

## Success Metrics
- Static-only scheduled tasks execute without reflection errors; regression tests cover the scenario.
- Monthly schedules never raise an alarm beyond `_toDate`; new tests assert the boundary behaviour.
- All logging factories share a single interface; solution builds without cross-namespace casts or duplicate types.
- `ApplicationTask` launches commands with complex arguments on both Windows and Linux; sample or test demonstrates successful execution.

## Prerequisites

Before implementation begins:
- [ ] Create `MIGRATION_TO_2.0.md` migration guide template
- [ ] Update `AssemblyInfo.Version.cs` to 2.0.0.0
- [ ] Create feature branch: `feature/reliability-v2.0`
- [ ] Notify known downstream consumers of upcoming breaking changes
- [ ] Review and approve breaking changes with stakeholders

## Requirements

### 1. CodeTask Static Invocation
- Detect `methodInstance.IsStatic` and skip object instantiation for static methods.
- Only create and dispose instances for non-static targets; preserve existing validation for instance methods lacking a default constructor.
- Logging should still wrap invocation in the current try/catch block.
- Add NUnit coverage in `DemiCode.Scheduling.Test/Tasks/CodeTaskTest.cs` for a static method on a type with no parameterless ctor.

### 2. MonthlySchedule Boundary Guard
- `MoveToFirstFrom` must return `false` and set `CurrentOccurrence = DateTime.MaxValue` when the next candidate exceeds `_toDate`.
- When `time.Date == _toDate`, emit `_toDate` once and subsequently report completion.
- Extend `MonthlyScheduleTest` with cases covering: inside range, equal to `_toDate`, and beyond `_toDate`.

### 3. Logger Abstraction Unification

**Current State:**
- Complete code duplication exists in two namespaces:
  - `DemiCode.Logging` namespace (12 files in DemiCode.Shared project)
  - `DemiCode.Logging.Loggers` namespace (12 files in DemiCode.Logging project)
- Files are byte-for-byte identical except for namespace declarations
- Type mismatch: Scheduler expects `DemiCode.Logging.ILoggerFactory`, but `Log4netLoggerFactory` implements `DemiCode.Logging.Loggers.ILoggerFactory`
- Current usage: 22 files in Scheduler already use `DemiCode.Logging` namespace

**Duplicated Components:**
```
ILogger.cs
ILoggerFactory.cs
ConsoleLogger.cs
ConsoleLoggerFactory.cs
NilLogger.cs
NilLoggerFactory.cs
LoggerFactoryCollection.cs
SimpleNDC.cs
Configuration/LoggerElementBase.cs
Configuration/LoggerElement.cs
Configuration/ConsoleLoggerElement.cs
Configuration/LoggerElementCollection.cs
```

**Target State:**
- **Keep:** `DemiCode.Logging` namespace (canonical location in DemiCode.Shared)
- **Delete:** `DemiCode.Logging/Loggers/` directory entirely
- **Delete:** `DemiCode.Logging/Configuration/Loggers/` directory entirely
- **Update:** `Log4netLoggerFactory` and related classes to use `DemiCode.Logging`

**Implementation Steps:**
- Update `DemiCode.Logging.log4net/Log4netLoggerFactory.cs` namespace import from `DemiCode.Logging.Loggers` to `DemiCode.Logging`
- Update `DemiCode.Logging.log4net/Configuration/Log4netLoggerElement.cs` similarly
- Delete `DemiCode.Logging/Loggers/` directory (8 duplicate files)
- Delete `DemiCode.Logging/Configuration/Loggers/` directory (4 duplicate files)
- Update all test files that reference the old namespace
- Verify all projects build for `netstandard2.0` and `net48` targets

**Consumer Migration:**
- One-line change required: `using DemiCode.Logging.Loggers;` → `using DemiCode.Logging;`
- No other code changes needed
- Document in `MIGRATION_TO_2.0.md`

### 4. ApplicationTask Argument Handling

**Breaking Change:** Change `ProcessStartInfo.UseShellExecute` from `true` to `false` for reliable cross-platform argument handling. No current consumers depend on shell features, making this a safe breaking change.

**Implementation:**
- Set `UseShellExecute = false` (was `true`)
- Populate arguments using `ProcessStartInfo.ArgumentList` for .NET Core 2.1+ / .NET Framework 4.7.1+
- Implement `QuoteArguments()` fallback for older framework versions (escape embedded quotes, handle whitespace)
- Preserve the public API (`Path`, `Arguments` properties)
- Enable stdout/stderr redirection for better logging integration

**Testing:**
- Add tests for arguments containing spaces
- Add tests for arguments with quotes and special characters
- Add tests for shell-sensitive characters (|, >, <, &, etc.)
- Manual smoke test on Windows 10+
- Manual smoke test on Ubuntu 20.04+

**Consumer Impact:**
- Shell features (pipes, redirection, environment variable expansion) no longer work
- Consumers requiring shell features must invoke shell explicitly:
  ```xml
  <!-- Old (no longer works) -->
  <ApplicationTask path="dir" arguments="*.txt > output.txt" />

  <!-- New (explicit shell invocation) -->
  <ApplicationTask path="cmd.exe" arguments="/c dir *.txt > output.txt" />
  ```
- Document migration in `MIGRATION_TO_2.0.md`

## Design Notes

**CodeTask:**
- Detect static methods via `MethodInfo.IsStatic` before attempting instantiation
- Reuse existing reflection objects to minimize overhead
- Ensure `IDisposable` disposal only runs for actually instantiated objects
- Preserve existing constructor validation for instance methods

**MonthlySchedule:**
- Compute the candidate month and check `candidate <= _toDate` before assignment
- Avoid scheduling `_fromDate.AddMonths(monthDifference + 1)` if it exceeds `_toDate`
- Return `false` and set `CurrentOccurrence = DateTime.MaxValue` when beyond range
- Handle leap year edge cases (existing test coverage should be maintained)

**Logger Unification:**
- Delete entire `DemiCode.Logging/Loggers/` and `DemiCode.Logging/Configuration/Loggers/` directories
- Update all `using DemiCode.Logging.Loggers;` references to `using DemiCode.Logging;`
- Verify no runtime type mismatches after consolidation
- Update affected tests in both `DemiCode.Shared.Test` and `DemiCode.Logging.Test`
- Ensure configuration-based factory instantiation works correctly

**ApplicationTask:**
- Use `ProcessStartInfo.ArgumentList` for .NET Core 2.1+ / .NET Framework 4.7.1+
- Implement `QuoteArguments()` fallback for older targets:
  - Escape embedded double quotes (`"` → `\"`)
  - Wrap arguments containing spaces in quotes
  - Handle edge cases (trailing backslashes before quotes)
- Set `UseShellExecute = false`, `RedirectStandardOutput = true`, `RedirectStandardError = true`
- Update README.md to document shell behavior change and migration path for consumers using shell features

## Edge Cases
- Instance tasks without default ctor should continue to throw.
- Monthly schedules spanning leap years must still behave (existing tests cover).
- Logging refactor must not break public API for downstream consumers—confirm binary compatibility or coordinate release notes.
- ApplicationTask must still support absolute/relative paths and environment-specific process launching conventions.

## Risk Assessment

| Risk | Severity | Mitigation |
|------|----------|------------|
| MonthlySchedule boundary fix introduces new edge case bugs | 🟡 MEDIUM | Comprehensive test coverage including leap years, month-end dates, and boundary conditions |
| Logger namespace change breaks downstream consumers | 🟡 MEDIUM | Clear migration guide with simple find/replace instructions; advance notification to known consumers |
| ApplicationTask shell behavior change breaks existing tasks | 🟡 MEDIUM | Document migration path; no known consumers rely on shell features; provide explicit shell invocation pattern |
| Cross-platform testing gaps (Windows/Linux differences) | 🟡 LOW | Manual smoke tests on both platforms; automated CI on Windows and Linux agents |
| Timeline underestimated due to logger refactor scope | 🟡 LOW | 5-6 day estimate includes buffer; logger changes are mostly mechanical deletions |
| Test suite regression after 487 tests updated | 🟡 LOW | Run full test suite after each requirement; verify all tests pass before proceeding |
| Breaking changes discovered after release | 🔴 HIGH | Alpha/beta testing phases; version as 2.0.0 to signal breaking changes; comprehensive migration documentation |

**Risk Acceptance:**
- Breaking changes are acceptable for v2.0.0 major version bump
- Benefits (cross-platform reliability, type safety) outweigh migration costs
- Simple migration path reduces consumer friction

## Testing Strategy
- Unit tests for each requirement (CodeTask, MonthlySchedule, logging factory creation, ApplicationTask argument handling).
- Integration: `dotnet test DemiCode.Scheduling.sln` on Windows and Linux agents.
- Manual smoke for ApplicationTask with tricky arguments and verifying schedule stop edge case in a sample app.

## Deliverables
- Updated C# source files noted above.
- Added/updated NUnit/MSTest coverage.
- Documentation changelog or README entry if behaviour changes.
- Test run evidence captured in PR description.

## Timeline

**Updated Estimate: 5-6 working days** (revised from 3-4 days)

| Task | Estimate | Notes |
|------|----------|-------|
| **Requirement 1:** CodeTask static invocation | 0.5 days | Straightforward reflection check; minimal test additions |
| **Requirement 2:** MonthlySchedule boundary guard | 0.5 days | Implementation + comprehensive boundary test coverage |
| **Requirement 3:** Logger unification | 2.0 days | Delete 12 files + update imports across all projects + verify 487 tests pass |
| **Requirement 4:** ApplicationTask argument handling | 1.5 days | Cross-platform logic + ArgumentList/fallback + Windows/Linux testing |
| **Documentation** | 0.5 days | MIGRATION_TO_2.0.md + README.md updates + CHANGELOG |
| **QA/Integration** | 0.5-1.0 days | Full test suite validation + cross-platform smoke tests + buffer |

**Total:** 5.5-6.5 days

**Rationale for revision:**
- Logger refactoring touches many files across multiple projects (DemiCode.Shared, DemiCode.Logging, DemiCode.Logging.log4net, all test projects)
- Cross-platform ApplicationTask testing requires manual verification on Windows and Linux
- Full regression testing (487 tests) must pass after logger namespace changes
- Migration documentation requires careful attention for v2.0.0 breaking changes

**QA Validation:**
- Same sprint as development
- Regression focus: scheduler alarms, boundary conditions, process launching, cross-platform behavior
- Alpha testing with internal consumers before beta release
