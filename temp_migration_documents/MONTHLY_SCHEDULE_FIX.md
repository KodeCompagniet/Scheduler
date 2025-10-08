# MonthlySchedule Fix Documentation

**Date Fixed**: 2025-10-04
**Version**: 2.0.0
**Status**: ✅ Fixed and verified

## Summary

The historic `MonthlySchedule` implementation bug has been **fixed** in version 2.0.0. The class now correctly implements monthly scheduling as originally intended.

## What Was Fixed

### The Original Bug
Previously, `DateTimeHelper.GetFirstOfMonth` incorrectly preserved the input date's day when calculating the next month:
```csharp
// BUGGY CODE (historical):
return new DateTime(nextMonth.Year, nextMonth.Month, nextMonth.Day, 0, 0, 0, 0);
//                                                      ^^^^^^^^^^^^
//                                    BUG: Should be 1, not nextMonth.Day
```

### The Fix
The method now correctly returns the first day of the next month:
```csharp
// FIXED CODE (current):
return new DateTime(nextMonth.Year, nextMonth.Month, 1, 0, 0, 0, 0);
//                                                    ^
//                                    CORRECT: Always returns 1st of month
```

## Current Behavior (Version 2.0.0)

- ✅ `MonthlySchedule` runs on the same day each month (the day specified in `_fromDate`)
- ✅ Properly implements monthly scheduling semantics
- ✅ Correctly respects `toDate` boundaries without emitting extra occurrences
- ✅ All tests pass and verify the correct behavior

## Code References

- **Fix Location**: `DemiCode.Scheduling/DateTimeHelper.cs:31`
- **MonthlySchedule Implementation**: `DemiCode.Scheduling/Schedules/MonthlySchedule.cs:85-137`
- **Test Coverage**: `DemiCode.Scheduling.Test/Schedules/MonthlyScheduleTest.cs`

## Test Verification

The test suite comprehensively verifies the correct behavior:
- `EndDateIsOptionalTest`: Verifies schedules work with and without end dates
- Multiple `[TestCase]` scenarios: All expect first-of-month outcomes
- Boundary tests: Verify `toDate` guard prevents extra occurrences

## Migration Impact

For users upgrading from previous versions:
- **Most users**: No changes needed - the fix makes the behavior match what the class name implies
- **Edge cases**: If any code relied on the buggy behavior, it will need adjustment
- **Recommendation**: Review any `MonthlySchedule` usage to ensure expected behavior

## Historical Note

This bug existed in the original .NET Framework implementation and was discovered during the .NET 8 migration. Initially, there was uncertainty about whether to preserve it for backward compatibility. However, analysis showed:
1. The tests actually expect correct first-of-month behavior
2. The bug made the class not fulfill its intended purpose
3. Fixing it aligns with the principle of least surprise

The fix was implemented as part of the version 2.0.0 release.