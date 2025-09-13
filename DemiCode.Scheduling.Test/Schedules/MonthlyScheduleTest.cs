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
    }
}
