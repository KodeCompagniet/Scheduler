using System;
using NUnit.Framework;

namespace DemiCode.Scheduling.Test
{
    [TestFixture]
    public class DateTimeTest
    {
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
