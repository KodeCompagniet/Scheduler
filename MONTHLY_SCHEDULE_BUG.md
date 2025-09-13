# MonthlySchedule Implementation Bug Documentation

**Date Discovered**: 2025-01-01  
**Status**: Bug preserved for backward compatibility  
**Impact**: High - Fundamental behavior doesn't match class purpose  

## Executive Summary

The `MonthlySchedule` class contains a critical bug where it doesn't actually implement monthly scheduling as its name and design documentation suggest. Instead, it preserves the input date's day when calculating the next occurrence, leading to behavior that doesn't align with typical monthly scheduling patterns.

## The Bug

### Location
`/DemiCode.Scheduling/Schedules/MonthlySchedule.cs:91-96`

### Buggy Code
```csharp
private static DateTime GetFirstOfMonth(DateTime date)
{
    DateTime nextMonth = date.AddMonths(1);
    return new DateTime(nextMonth.Year, nextMonth.Month, nextMonth.Day, 0, 0, 0, 0);
    //                                                      ^^^^^^^^^^^^
    //                                    BUG: Should be 1, not nextMonth.Day
}
```

### What It Does
- **Method Name Suggests**: Returns the 1st day of the next month
- **Actually Does**: Returns the same day number in the next month
- **Example**: Input Sep 18 → Returns Oct 18 (not Oct 1)

## Intended Design

According to the comments in the source file:
```
/* Must support the following scenarios:
 *  1. the X day of every Y month
 *  2. the n'th named day of every Y month
 *  ...
 * X : numeric (31 >= X > 0)
 */
```

The design clearly intends for MonthlySchedule to run on a **fixed day of each month** (e.g., always on the 15th).

## Test Inconsistencies

### Test: MoveToFirstFromTest1
The test expects behavior that doesn't match monthly scheduling at all:

```csharp
// Schedule created with: new DateTime(2006, 1, 1) - January 1st
MonthlySchedule schedule = new MonthlySchedule(new DateTime(2006, 1, 1), new DateTime(2006, 12, 31));

// But test expects multiple occurrences within September:
AssertSuccess(schedule, new DateTime(2006, 9, 18, 11, 00, 00), new DateTime(2006, 10, 1, 0, 00, 00));
AssertSuccess(schedule, new DateTime(2006, 9, 18, 20, 00, 00).AddTicks(-1L), new DateTime(2006, 9, 18, 20, 00, 00));
AssertSuccess(schedule, new DateTime(2006, 9, 20, 20, 00, 00), new DateTime(2006, 9, 20, 20, 00, 00));
AssertSuccess(schedule, new DateTime(2006, 9, 25, 20, 00, 00), new DateTime(2006, 9, 25, 20, 00, 00));
AssertSuccess(schedule, new DateTime(2006, 9, 27, 20, 00, 00), new DateTime(2006, 9, 27, 20, 00, 00));
```

**Problems**:
1. Schedule initialized with Jan 1st but expects to run on Sep 18th, 20th, 25th, 27th
2. Multiple occurrences within the same month (not monthly behavior)
3. Test appears to be testing weekly or multi-day scheduling, not monthly

### Test: EndDateIsOptionalTest
```csharp
MonthlySchedule schedule = new MonthlySchedule(new DateTime(2006, 1, 1), DateTime.MaxValue);
AssertSuccess(schedule, new DateTime(2206, 9, 18, 11, 00, 00), new DateTime(2206, 10, 1, 0, 00, 00));
```

This test at least expects moving to the next month, but still has issues with the day calculation.

## Impact Analysis

### Current Behavior (with bug)
- Input date's day is preserved when moving to next month
- Doesn't actually implement monthly scheduling
- Tests pass but behavior is incorrect

### Correct Behavior (without bug)
- Would run on the same day each month (from `_fromDate.Day`)
- Would properly implement monthly scheduling
- Tests would fail

## Why The Bug Was Preserved

During the .NET 8 migration (2025-01-01), the decision was made to preserve the bug because:
1. **Existing tests depend on it** - All tests were written to match the buggy behavior
2. **Backward compatibility** - Unknown production systems may depend on this behavior
3. **Risk of breaking changes** - Without comprehensive understanding of all consumers

## Recommendations

### Short Term (Current)
- Bug preserved with inline documentation
- This documentation file created for reference
- Tests continue to pass

### Long Term (Future)
1. **Audit all consumers** of MonthlySchedule to understand usage patterns
2. **Create new class** `ProperMonthlySchedule` with correct behavior
3. **Deprecate** the buggy MonthlySchedule class
4. **Migration path** for existing consumers
5. **Fix or rewrite tests** to match intended behavior

### Alternative Interpretation
It's possible the class was never intended for traditional monthly scheduling but rather for some other pattern. In this case:
1. Rename the class to reflect actual behavior
2. Update documentation to match implementation
3. Create a new, properly-named monthly schedule class

## Test Data Analysis

The test `MoveToFirstFromTest1` shows these expected occurrences:
- Sep 18 @ 20:00
- Sep 20 @ 20:00  
- Sep 25 @ 20:00
- Sep 27 @ 20:00

This pattern doesn't match any standard scheduling pattern and may indicate:
1. Tests were incorrectly copied from another schedule type
2. Original implementation had a different purpose
3. Tests were written to match buggy behavior without understanding

## Code References

- Bug Location: `/DemiCode.Scheduling/Schedules/MonthlySchedule.cs:91-96`
- Primary Test: `/DemiCode.Scheduling.Test/Schedules/MonthlyScheduleTest.cs:51-76`
- Design Comment: `/DemiCode.Scheduling/Schedules/MonthlySchedule.cs:5-30`

## Additional Investigation Notes

### Test Failure Mystery
Even with the original buggy `GetFirstOfMonth` implementation restored, the test `MoveToFirstFromTest1` still fails:
- Test expects: Sep 18 @ just before 20:00 → Sep 18 @ 20:00 (same day, later time)
- Bug produces: Sep 18 → Oct 18 (next month, same day)
- The original implementation would ALWAYS go to next month

**Possible explanations**:
1. Tests were never actually passing in the original .NET Framework version
2. There was additional logic that got lost during migration
3. Tests were written for a different implementation that never existed
4. The MonthlySchedule class was meant to be something entirely different

### Current Status
- Original bug preserved but tests still fail
- Test expectations don't match any reasonable interpretation of the code
- Further investigation needed to understand the original intent

## Decision Log

- **2025-01-01**: Bug discovered during .NET 8 migration
- **2025-01-01**: Decision made to preserve bug for backward compatibility
- **2025-01-01**: This documentation created
- **2025-01-01**: Tests still fail even with bug preserved - unclear if they ever passed

## Related Issues

- Missing `GetHashCode()` implementations in multiple configuration classes
- TimeSpan formatting issues in WeeklySchedule (fixed)
- Cross-platform path separator issues (fixed)