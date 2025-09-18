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
        [TestCase(2001, 4, 30, 4, 30, 5, 30, TestName = "When schedule starts at April 30th, moving from April 30th will move to May 30th")]
        [TestCase(2001, 4, 30, 5, 1, 5, 30, TestName = "When schedule starts at April 30th, moving from May 1st will move to May 30th")]
        [TestCase(2001, 4, 30, 5, 31, 6, 30, TestName = "When schedule starts at April 30th, moving from May 31st will move to June 30th")]
        [TestCase(2001, 6, 30, 6, 30, 7, 30, TestName = "When schedule starts at June 30th, moving from June 30th will move to July 30th")]
        [TestCase(2001, 6, 30, 7, 1, 7, 30, TestName = "When schedule starts at June 30th, moving from July 1st will move to July 30th")]
        [TestCase(2001, 9, 30, 9, 30, 10, 30, TestName = "When schedule starts at September 30th, moving from September 30th will move to October 30th")]
        [TestCase(2001, 11, 30, 11, 30, 12, 30, TestName = "When schedule starts at November 30th, moving from November 30th will move to December 30th")]
        [TestCase(2001, 11, 30, 12, 1, 12, 30, TestName = "When schedule starts at November 30th, moving from December 1st will move to December 30th")]
        [TestCase(2001, 2, 28, 2, 28, 3, 28, TestName = "When schedule starts at February 28th (non-leap), moving from February 28th will move to March 28th")]
        [TestCase(2001, 2, 28, 3, 1, 3, 28, TestName = "When schedule starts at February 28th (non-leap), moving from March 1st will move to March 28th")]
        [TestCase(2001, 2, 28, 3, 31, 4, 28, TestName = "When schedule starts at February 28th (non-leap), moving from March 31st will move to April 28th")]
        [TestCase(2001, 2, 28, 4, 1, 4, 28, TestName = "When schedule starts at February 28th (non-leap), moving from April 1st will move to April 28th")]
        [TestCase(2000, 2, 29, 2, 29, 3, 29, TestName = "When schedule starts at February 29th (leap year), moving from February 29th will move to March 29th")]
        [TestCase(2000, 2, 29, 3, 1, 3, 29, TestName = "When schedule starts at February 29th (leap year), moving from March 1st will move to March 29th")]
        [TestCase(2000, 2, 29, 3, 31, 4, 29, TestName = "When schedule starts at February 29th (leap year), moving from March 31st will move to April 29th")]
        [TestCase(2000, 2, 29, 4, 1, 4, 29, TestName = "When schedule starts at February 29th (leap year), moving from April 1st will move to April 29th")]
        [TestCase(2001, 3, 31, 3, 31, 4, 30, TestName = "When schedule starts at March 31st, moving from March 31st will move to April 30th (because April 31st does not exist)")]
        [TestCase(2001, 3, 31, 4, 1, 4, 30, TestName = "When schedule starts at March 31st, moving from April 1st will move to April 30th (because April 31st does not exist)")]
        [TestCase(2001, 5, 31, 5, 31, 6, 30, TestName = "When schedule starts at May 31st, moving from May 31st will move to June 30th (because June 31st does not exist)")]
        [TestCase(2001, 5, 31, 6, 1, 6, 30, TestName = "When schedule starts at May 31st, moving from June 1st will move to June 30th (because June 31st does not exist)")]
        [TestCase(2001, 8, 31, 8, 31, 9, 30, TestName = "When schedule starts at August 31st, moving from August 31st will move to September 30th (because September 31st does not exist)")]
        [TestCase(2001, 8, 31, 9, 1, 9, 30, TestName = "When schedule starts at August 31st, moving from September 1st will move to September 30th (because September 31st does not exist)")]
        [TestCase(2001, 7, 31, 7, 31, 8, 31, TestName = "When schedule starts at July 31st, moving from July 31st will move to August 31st")]
        [TestCase(2001, 7, 31, 8, 1, 8, 31, TestName = "When schedule starts at July 31st, moving from August 1st will move to August 31st")]
        [TestCase(2001, 10, 31, 10, 31, 11, 30, TestName = "When schedule starts at October 31st, moving from October 31st will move to November 30th (no 31st in November)")]
        [TestCase(2001, 10, 31, 11, 1, 11, 30, TestName = "When schedule starts at October 31st, moving from November 1st will move to November 30th (no 31st in November)")]
        public void MoveToFirstFrom(int year, int fromMonth, int fromDay, int moveToFirstFromMonth, int moveToFirstFromDay, int expectedMonth, int expectedDay)
        {
            var schedule = new MonthlySchedule(Dt.At(year, fromMonth, fromDay), Dt.At(year + 2, 12, 31));

            var result = schedule.MoveToFirstFrom(Dt.At(year, moveToFirstFromMonth, moveToFirstFromDay));

            Assert.That(result, Is.True);
            Assert.That(schedule.CurrentOccurrence.Year, Is.EqualTo(year));
            Assert.That(schedule.CurrentOccurrence.Month, Is.EqualTo(expectedMonth));
            Assert.That(schedule.CurrentOccurrence.Day, Is.EqualTo(expectedDay));
        }

        [TestCase(2000, 12, 31, 2000, 12, 31, 2001, 1, 31, TestName = "When schedule starts at December 31st 2000, moving from December 31st 2000 will move to January 31st 2001 (cross-year)")]
        [TestCase(2000, 12, 31, 2001, 1, 1, 2001, 1, 31, TestName = "When schedule starts at December 31st 2000, moving from January 1st 2001 will move to January 31st 2001 (cross-year)")]
        [TestCase(2000, 12, 31, 2001, 1, 31, 2001, 2, 28, TestName = "When schedule starts at December 31st 2000, moving from January 31st 2001 will move to February 28th 2001 (cross-year, non-leap)")]
        [TestCase(1999, 12, 31, 2000, 1, 31, 2000, 2, 29, TestName = "When schedule starts at December 31st 1999, moving from January 31st 2000 will move to February 29th 2000 (cross-year, leap)")]
        public void MoveToFirstFromCrossYear(int startYear, int fromMonth, int fromDay, int moveToYear, int moveToMonth, int moveToDay, int expectedYear, int expectedMonth, int expectedDay)
        {
            var schedule = new MonthlySchedule(Dt.At(startYear, fromMonth, fromDay), Dt.At(2010, 12, 31));

            var result = schedule.MoveToFirstFrom(Dt.At(moveToYear, moveToMonth, moveToDay));

            Assert.That(result, Is.True);
            Assert.That(schedule.CurrentOccurrence.Year, Is.EqualTo(expectedYear));
            Assert.That(schedule.CurrentOccurrence.Month, Is.EqualTo(expectedMonth));
            Assert.That(schedule.CurrentOccurrence.Day, Is.EqualTo(expectedDay));
        }
    }
}
