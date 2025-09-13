using System;
using DemiCode.Scheduling.Time;
using NUnit.Framework;

namespace DemiCode.Scheduling.Test
{
    [TestFixture]
    public class DateTimeHelperTest
    {
        [TestCase(2021, 1, 2021, 3, 2, TestName = "From Jan 2021 to Mar 2021")]
        [TestCase(2020, 2, 2020, 8, 6, TestName = "From Feb 2020 to Aug 2020")]
        public void GetMonthDifference_Within_SameYear(int startYear, int startMonth, int endYear, int endMonth, int expectedMonths)
        {
            var start = Dt.At(startYear, startMonth, 15);
            var end = Dt.At(endYear, endMonth, 10);

            var result = start.GetMonthDifference(end);

            Assert.That(result, Is.EqualTo(expectedMonths));
        }

        [TestCase(2021, 1, 31, 2021, 11, 10, TestName = "From Jan 2021 to Nov 2021")]
        public void GetMonthDifference_Within_SameYear_With_Different_Days(int startYear, int startMonth, int startDay, int endYear, int endMonth, int expectedMonths)
        {
            var start = Dt.At(startYear, startMonth, startDay);
            var end = Dt.At(endYear, endMonth, 10);

            var result = start.GetMonthDifference(end);

            Assert.That(result, Is.EqualTo(expectedMonths));
        }

        [TestCase(2021, 12, 2025, 3, 36 + 1 + 2, TestName = "From Dec 2021 to Mar 2025")]
        [TestCase(2020, 1, 2021, 1, 12, TestName = "From Jan 2020 to Jan 2021")]
        public void GetMonthDifference_Across_Years(int startYear, int startMonth, int endYear, int endMonth, int expectedMonths)
        {
            var start = Dt.At(startYear, startMonth, 10);
            var end = Dt.At(endYear, endMonth, 15);

            var result = start.GetMonthDifference(end);

            Assert.That(result, Is.EqualTo(expectedMonths));
        }

        [Test]
        public void GetMonthDifference_End_Before_Start_Throws()
        {
            var start = Dt.At(2021, 1, 10);
            var end = Dt.At(2020, 12, 15);

            Assert.That(() => start.GetMonthDifference(end), Throws.ArgumentException);
        }

        /// <summary>
        /// Confirm that GetFirstOfMonth returns the first of the next month.
        /// </summary>
        [TestCase(1, 1, 2, TestName = "From 1st Jan to 1st Feb")]
        [TestCase(1, 15, 2, TestName = "From 15th Jan to 1st Feb")]
        [TestCase(1, 31, 2, TestName = "From 31st Jan to 1st Feb")]
        [TestCase(2, 1, 3, TestName = "From 1st Feb to 1st Mar")]
        [TestCase(2, 15, 3, TestName = "From 15th Feb to 1st Mar")]
        [TestCase(2, 28, 3, TestName = "From 28th Feb to 1st Mar")]
        [TestCase(3, 1, 4, TestName = "From 1st Mar to 1st Apr")]
        [TestCase(3, 15, 4, TestName = "From 15th Mar to 1st Apr")]
        public void GetFirstOfMonth_FromAnyDay_YieldFirstOfNextMonth(int fromMonth, int fromDay, int expectedMonth)
        {
            var date = Dt.At(2007, fromMonth, fromDay);

            var newDate = date.GetFirstOfMonth();

            Assert.That(newDate, Is.EqualTo(Dt.At(2007, expectedMonth, 1)));
        }
    }
}