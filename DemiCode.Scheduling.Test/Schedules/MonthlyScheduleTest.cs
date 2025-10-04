using DemiCode.Scheduling.Time;
using NUnit.Framework;
using System;

namespace DemiCode.Scheduling.Schedules.Test
{
    [TestFixture]
    public class MonthlyScheduleTest
    {
        [Test]
        public void EndDateIsOptionalTest()
        {
            var schedule = new MonthlySchedule(Dt.At(2006, 1, 1), DateTime.MaxValue);

            var result = schedule.MoveToFirstFrom(Dt.At(2206, 9, 18, 11));

            Assert.That(result, Is.True);
            Assert.That(schedule.CurrentOccurrence, Is.EqualTo(Dt.At(2206, 10, 1)));
        }

        [Test]
        public void MoveToFirstFrom_BeforeStart_MovesToStart()
        {
            var schedule = new MonthlySchedule(Dt.At(2006, 1, 1), DateTime.MaxValue);

            var result = schedule.MoveToFirstFrom(Dt.At(1906, 9, 18, 11));

            Assert.That(result, Is.True);
            Assert.That(schedule.CurrentOccurrence, Is.EqualTo(Dt.At(2006, 1, 1)));
        }


        [Test]
        public void MoveToFirstFrom_AfterEnd_YieldsNoResult()
        {
            var schedule = new MonthlySchedule(Dt.At(2006, 1, 1), Dt.At(2006, 12, 1));

            var result = schedule.MoveToFirstFrom(Dt.At(2010));

            Assert.That(result, Is.False);
            Assert.That(schedule.CurrentOccurrence, Is.EqualTo(DateTime.MaxValue));
        }

        /// <summary>
        /// Validate the behavior of the MonthlySchedule class.
        /// Depending on the starting date, the behavior of the schedule may vary.
        /// For regular days, the schedule will move to the same day of the next month.
        /// For end-of-the-month days, the schedule will move to the last day of month we are moving to.
        /// Exception to this rule is February (28th or 29th on Leap Years), which will always move to the 28th or 29th of the target month.
        /// </summary>
        [TestCase(2001, 1, 1, 9, 3, 10, 1, TestName = "When schedule starts at January 1st, moving from September 3rd will end on October 1st")]
        [TestCase(2001, 1, 1, 10, 1, 11, 1, TestName = "When schedule starts at January 1st, moving from October 1st will end on November 1st")]
        [TestCase(2001, 1, 1, 10, 2, 11, 1, TestName = "When schedule starts at January 1st, moving from October 2nd will end on November 1st")]
        [TestCase(2001, 1, 8, 1, 2, 1, 8, TestName = "When schedule starts at January 8th, moving from January 2nd will end on January 8th")]
        [TestCase(2001, 1, 31, 2, 3, 2, 28, TestName = "When schedule starts at January 31st, moving from February 3rd will end on February 28th")]
        [TestCase(2001, 1, 31, 9, 30, 10, 31, TestName = "When schedule starts at January 31st, moving from September 30th will end on October 31st")]
        [TestCase(2001, 1, 31, 10, 1, 10, 31, TestName = "When schedule starts at January 31st, moving from October 1st will end on October 31st")]
        [TestCase(2001, 1, 31, 10, 31, 11, 30, TestName = "When schedule starts at January 31st, moving from October 31st will move to November 30th (because November 31st does not exist)")]
        [TestCase(2001, 1, 31, 11, 1, 11, 30, TestName = "When schedule starts at January 31st, moving from November 1st will move to November 30th (because November 31st does not exist)")]
        [TestCase(2001, 4, 30, 9, 30, 10, 30, TestName = "When schedule starts at April 30th, moving from September 30th will end on October 30th")]
        [TestCase(2001, 4, 30, 10, 1, 10, 30, TestName = "When schedule starts at April 30th, moving from October 1st will end on October 30th")]
        [TestCase(2001, 4, 30, 10, 31, 11, 30, TestName = "When schedule starts at April 30th, moving from October 31st will move to November 30th")]
        [TestCase(2001, 4, 30, 11, 1, 11, 30, TestName = "When schedule starts at April 30th, moving from November 1st will move to November 30th")]
        [TestCase(2001, 2, 28, 9, 30, 10, 28, TestName = "When schedule starts at February 28th, moving from September 30th will end on October 28th")]
        [TestCase(2001, 2, 28, 10, 1, 10, 28, TestName = "When schedule starts at February 28th, moving from October 1st will end on October 28th")]
        [TestCase(2001, 2, 28, 10, 31, 11, 28, TestName = "When schedule starts at February 28th, moving from October 31st will move to November 28th")]
        [TestCase(2001, 2, 28, 11, 1, 11, 28, TestName = "When schedule starts at February 28th, moving from November 1st will move to November 28th")]
        [TestCase(2000, 2, 29, 9, 30, 10, 29, TestName = "When schedule starts at February 29th, moving from September 30th will end on October 29th")]
        [TestCase(2000, 2, 29, 10, 1, 10, 29, TestName = "When schedule starts at February 29th, moving from October 1st will end on October 29th")]
        [TestCase(2000, 2, 29, 10, 31, 11, 29, TestName = "When schedule starts at February 29th, moving from October 31st will move to November 29th")]
        [TestCase(2000, 2, 29, 11, 1, 11, 29, TestName = "When schedule starts at February 29th, moving from November 1st will move to November 29th")]
        public void MoveToFirstFrom(int year, int fromMonth, int fromDay, int moveToFirstFromMonth, int moveToFirstFromDay, int expectedMonth, int expectedDay)
        {
            var schedule = new MonthlySchedule(Dt.At(year, fromMonth, fromDay), Dt.At(year, 12, 31));

            var result = schedule.MoveToFirstFrom(Dt.At(year, moveToFirstFromMonth, moveToFirstFromDay));

            Assert.That(result, Is.True);
            Assert.That(schedule.CurrentOccurrence.Year, Is.EqualTo(year));
            Assert.That(schedule.CurrentOccurrence.Month, Is.EqualTo(expectedMonth));
            Assert.That(schedule.CurrentOccurrence.Day, Is.EqualTo(expectedDay));
        }

        /// <summary>
        /// Test that MonthlySchedule respects the toDate boundary and does not emit occurrences beyond it.
        /// This tests the bug where schedules emit one extra occurrence past their configured end date.
        /// </summary>
        [Test]
        public void MoveToFirstFrom_OnToDate_ReturnsToDateOccurrence()
        {
            // Schedule from Jan 1 to Feb 1, 2025
            // Occurrences are on the 1st of each month
            var schedule = new MonthlySchedule(Dt.At(2025, 1, 1), Dt.At(2025, 2, 1));

            // Move to first occurrence from January 15 (mid-month)
            var result = schedule.MoveToFirstFrom(Dt.At(2025, 1, 15));

            // Should return true and set occurrence to February 1 (next occurrence, equals toDate)
            Assert.That(result, Is.True);
            Assert.That(schedule.CurrentOccurrence, Is.EqualTo(Dt.At(2025, 2, 1)));
        }

        [Test]
        public void MoveToFirstFrom_OnToDateExactly_ReturnsToDateOccurrence()
        {
            // Schedule from Jan 1 to Feb 1, 2025
            // Occurrences are on the 1st of each month
            var schedule = new MonthlySchedule(Dt.At(2025, 1, 1), Dt.At(2025, 2, 1));

            // Move to first occurrence from the exact toDate (Feb 1)
            // Since Feb 1 IS an occurrence, "moving from" it means moving to the NEXT occurrence
            // But the next occurrence (March 1) is beyond toDate, so should return false
            var result = schedule.MoveToFirstFrom(Dt.At(2025, 2, 1));

            // Should return false because moving from Feb 1 would require going to March 1 (beyond toDate)
            Assert.That(result, Is.False);
            Assert.That(schedule.CurrentOccurrence, Is.EqualTo(DateTime.MaxValue));
        }

        [Test]
        public void MoveToFirstFrom_AtEndOfToDateMonth_DoesNotExceedBoundary()
        {
            // BUG TEST: Schedule from Jan 1 to Feb 1, 2025
            // Occurrences are on the 1st of each month
            var schedule = new MonthlySchedule(Dt.At(2025, 1, 1), Dt.At(2025, 2, 1));

            // Move from Jan 2 (after the Jan 1 occurrence)
            // This should move to Feb 1 (the toDate), not beyond
            var result = schedule.MoveToFirstFrom(Dt.At(2025, 1, 2));

            Assert.That(result, Is.True);
            Assert.That(schedule.CurrentOccurrence, Is.EqualTo(Dt.At(2025, 2, 1)),
                "Should move to February 1 (toDate), not beyond");
        }

        [Test]
        public void MoveToFirstFrom_AfterLastValidOccurrence_ReturnsFalse()
        {
            // Schedule from Jan 1 to Feb 1, 2025
            // Occurrences are on the 1st of each month
            var schedule = new MonthlySchedule(Dt.At(2025, 1, 1), Dt.At(2025, 2, 1));

            // Move from Jan 15 (mid-month) - should give us Feb 1
            var result = schedule.MoveToFirstFrom(Dt.At(2025, 1, 15));
            Assert.That(result, Is.True);
            Assert.That(schedule.CurrentOccurrence, Is.EqualTo(Dt.At(2025, 2, 1)));

            // BUG TEST: Now move from Feb 2 (after the last valid occurrence).
            // The buggy implementation would calculate March 1 and return it,
            // even though March 1 exceeds toDate (Feb 1).
            // EXPECTED: Should return false and set CurrentOccurrence to DateTime.MaxValue
            result = schedule.MoveToFirstFrom(Dt.At(2025, 2, 2));

            Assert.That(result, Is.False, "Should return false when next occurrence would exceed toDate");
            Assert.That(schedule.CurrentOccurrence, Is.EqualTo(DateTime.MaxValue),
                "Should set CurrentOccurrence to DateTime.MaxValue when schedule is complete");
        }

        [Test]
        public void MoveToFirstFrom_BoundaryCase_LastDayOfMonth()
        {
            // Schedule from Jan 31 to Feb 28, 2025 (non-leap year)
            var schedule = new MonthlySchedule(Dt.At(2025, 1, 31), Dt.At(2025, 2, 28));

            // Move from Feb 1 - should give us Feb 28 (last day of Feb that aligns with Jan 31)
            var result = schedule.MoveToFirstFrom(Dt.At(2025, 2, 1));

            Assert.That(result, Is.True);
            Assert.That(schedule.CurrentOccurrence, Is.EqualTo(Dt.At(2025, 2, 28)));

            // Now try to move beyond - should return false
            result = schedule.MoveToFirstFrom(Dt.At(2025, 3, 1));
            Assert.That(result, Is.False);
            Assert.That(schedule.CurrentOccurrence, Is.EqualTo(DateTime.MaxValue));
        }
    }
}
