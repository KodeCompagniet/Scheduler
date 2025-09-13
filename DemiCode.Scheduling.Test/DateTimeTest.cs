using System;
using DemiCode.Scheduling.Time;
using NUnit.Framework;

namespace DemiCode.Scheduling.Test
{
    [TestFixture]
    public class DateTimeTest
    {
        [Test]
        public void ValidateAddingMonthFromJanuaryToFebruaryLeapYear()
        {
            var date = Dt.At(2008, 1, 31);

            var result = date.AddMonths(1);

            Assert.That(result, Is.EqualTo(new DateTime(2008, 2, 29)));
        }

        [Test]
        public void ValidateAddingMonthFromJanuaryToFebruaryNonLeapYear()
        {
            var date = Dt.At(2007, 1, 31);

            var result = date.AddMonths(1);

            Assert.That(result, Is.EqualTo(new DateTime(2007, 2, 28)));
        }

        [Test]
        public void ValidateAddingMonthFromFebruaryToMarch()
        {
            var date = Dt.At(2007, 2, 28);

            var result = date.AddMonths(1);

            Assert.That(result, Is.EqualTo(new DateTime(2007, 3, 28)));
        }

        [Test]
        public void ValidateAddingMonthFromJanuary31stToNovember()
        {
            var date = Dt.At(2007, 1, 31);

            var result = date.AddMonths(10);

            Assert.That(result, Is.EqualTo(Dt.At(2007, 11, 30)));
        }

        /// <summary>
        /// Confirm that adding a 24 hour timespan yields the same time of day the next day.
        /// </summary>
        [Test]
        public void AddHoursTest()
        {
            DateTime date = new DateTime(2007, 1, 1, 8, 0, 0); 
            TimeSpan hours = new TimeSpan(24, 0, 0);
            DateTime newDate = date.Add(hours);

            DateTime expected = new DateTime(2007, 1, 2, 8, 0, 0);

            Assert.AreEqual(expected, newDate);
        }
    }
}
