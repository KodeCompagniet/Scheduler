# Migration Guide: DemiCode Scheduler 1.x → 2.0

This guide covers breaking changes introduced in DemiCode Scheduler 2.0.0 and how to migrate your code.

## Breaking Changes

### 1. Logger Namespace Consolidation

**Change:** The duplicate `DemiCode.Logging.Loggers` namespace has been removed. All logging types now live in `DemiCode.Logging`.

**Binary Compatibility:** Type forwarders have been added to `DemiCode.Logging.dll`, so existing compiled binaries that reference the old types will continue to work without recompilation.

**Migration:**
```csharp
// Before (1.x)
using DemiCode.Logging.Loggers;

// After (2.0)
using DemiCode.Logging;
```

**Impact:** This is a simple find-replace operation. No other code changes are needed. Existing binaries continue to work via type forwarders.

---

### 2. ApplicationTask Shell Execution Behavior

**Change:** `ApplicationTask` no longer uses shell execution by default. `ProcessStartInfo.UseShellExecute` is now `false`.

**Why:** Enables reliable cross-platform argument handling and better security by avoiding shell interpretation.

**What Still Works:**
- Simple commands: `<ApplicationTask path="myapp.exe" arguments="arg1 arg2" />`
- Arguments with spaces: `<ApplicationTask path="myapp.exe" arguments="--file 'path with spaces.txt'" />`
- Quotes and special characters are now properly escaped on all platforms

**What No Longer Works:**
- Shell features: pipes (`|`), redirection (`>`, `<`), environment variable expansion (`$VAR`, `%VAR%`)
- Command chaining with `&&` or `||`

**Migration for Shell Features:**

If you were using shell features, explicitly invoke the shell:

**Windows:**
```xml
<!-- Before (1.x) -->
<ApplicationTask path="dir" arguments="*.txt > output.txt" />

<!-- After (2.0) -->
<ApplicationTask path="cmd.exe" arguments="/c dir *.txt > output.txt" />
```

**Linux/macOS:**
```xml
<!-- Before (1.x) -->
<ApplicationTask path="ls" arguments="-la | grep .txt" />

<!-- After (2.0) -->
<ApplicationTask path="/bin/sh" arguments="-c 'ls -la | grep .txt'" />
```

**Code-First API:**
```csharp
// Before (1.x)
scheduler.Tasks.Add(new ApplicationTask("dir", "*.txt > output.txt"));

// After (2.0) - Windows
scheduler.Tasks.Add(new ApplicationTask("cmd.exe", "/c dir *.txt > output.txt"));

// After (2.0) - Linux/macOS
scheduler.Tasks.Add(new ApplicationTask("/bin/sh", "-c 'ls -la | grep .txt'"));
```

**Benefits:**
- ✅ Cross-platform argument handling now works consistently
- ✅ No more quoting issues on non-Windows platforms
- ✅ Better security (no accidental shell injection)
- ✅ stdout/stderr redirection now available for logging

---

### 3. MonthlySchedule Boundary Enforcement

**Change:** `MonthlySchedule` now correctly stops at the configured `toDate` and will not emit an extra alarm beyond the boundary.

**Why:** This was a bug where schedules would generate one additional occurrence past their configured end date.

**Migration:**

If your code relied on the buggy behavior (extra occurrence), you may need to adjust your `toDate`:

```csharp
// Before (1.x) - got extra occurrence on March 1
var schedule = new MonthlySchedule("Monthly",
    fromDate: new DateTime(2025, 1, 1),
    toDate: new DateTime(2025, 2, 28),
    dayOfMonth: 1);

// After (2.0) - if you want March 1 occurrence, extend toDate
var schedule = new MonthlySchedule("Monthly",
    fromDate: new DateTime(2025, 1, 1),
    toDate: new DateTime(2025, 3, 1),  // Explicitly include March
    dayOfMonth: 1);
```

**Impact:** Most users will not need changes—this fix makes the implementation match the documented behavior.

---

### 4. CodeTask Static Method Support

**Change:** `CodeTask` now supports static methods on classes without parameterless constructors.

**Why:** Previously, scheduling a static method on a utility class without a default constructor would throw a reflection error.

**Migration:**

No migration needed—this is a pure enhancement. Code that previously failed will now work:

```csharp
// Now works in 2.0 (failed in 1.x)
public static class UtilityClass
{
    // No parameterless constructor needed for static methods
    public static void ScheduledTask(string param)
    {
        // Task implementation
    }
}

scheduler.Tasks.Add(new CodeTask(typeof(UtilityClass), nameof(UtilityClass.ScheduledTask), "value"));
```

---

## Semantic Versioning Justification

Version 2.0.0 was chosen because:

1. **Binary incompatibility**: Namespace change (`DemiCode.Logging.Loggers` → `DemiCode.Logging`)
2. **Behavior changes**: Process execution model, schedule boundary enforcement
3. **Public API impact**: Consumers must update imports and potentially shell command configurations

---

## Testing Your Migration

After migrating, verify:

1. **Build succeeds** with no namespace errors
2. **All scheduled tasks execute** as expected
3. **ApplicationTask commands** still launch correctly (especially if using shell features)
4. **MonthlySchedule boundaries** behave as documented

---

## Support

If you encounter migration issues:

1. Review this guide thoroughly
2. Check the [CHANGELOG](CHANGELOG.md) for detailed changes
3. Review test examples in `DemiCode.Scheduling.Test/`
4. Report issues at the project repository

---

**Last Updated:** 2025-10-04
**Version:** 2.0.0
