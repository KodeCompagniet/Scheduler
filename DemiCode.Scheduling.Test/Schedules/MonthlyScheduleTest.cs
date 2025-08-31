using NUnit.Framework;
using System;
using System.Text;
using System.Collections.Generic;
using DemiCode.Scheduling.Schedules;
using System.Xml;
using System.Globalization;

namespace DemiCode.Scheduling.Schedules.Test
{
    [TestFixture]
    public class MonthlyScheduleTest
    {

        /// <summary>
        ///A test for the algoritm of GetFirstOfMonth 
        ///</summary>
        
        [Test]
        public void GetFirstOfMonthTest()
        {
            DateTime month = new DateTime(2006, 12, 1);
            DateTime expected = new DateTime(2006, 12, 1);
            DateTime first = month.Subtract(TimeSpan.FromDays(month.Day - 1));
            Assert.AreEqual(expected, first);

            month = new DateTime(2006, 12, 3);
            expected = new DateTime(2006, 12, 1);
            first = month.Subtract(TimeSpan.FromDays(month.Day - 1));
            Assert.AreEqual(expected, first);

            month = new DateTime(2006, 12, 31);
            expected = new DateTime(2006, 12, 1);
            first = month.Subtract(TimeSpan.FromDays(month.Day - 1));
            Assert.AreEqual(expected, first);
        }

        [Test]
        public void EndDateIsOptionalTest()
        {
            //Initialize.
            MonthlySchedule schedule = new MonthlySchedule(new DateTime(2006, 1, 1), DateTime.MaxValue);
            AssertSuccess(schedule, new DateTime(2206, 9, 18, 11, 00, 00), new DateTime(2206, 10, 1, 0, 00, 00));
         
        }

        /// <summary>
        ///A test for MoveToFirstFrom (DateTime)
        ///</summary>
        [Test]
        public void MoveToFirstFromTest1()
        {
            //Initialize.
            MonthlySchedule schedule = new MonthlySchedule(new DateTime(2006, 1, 1), new DateTime(2006, 12, 31));

            //Start testing
            //A monday
            AssertSuccess(schedule, new DateTime(2006, 9, 18, 11, 00, 00), new DateTime(2006, 10, 1, 0, 00, 00));
            AssertSuccess(schedule, new DateTime(2006, 9, 18, 20, 00, 00).AddTicks(-1L), new DateTime(2006, 10, 1, 0, 00, 00));
            AssertSuccess(schedule, new DateTime(2006, 9, 18, 20, 00, 00), new DateTime(2006, 9, 18, 20, 00, 00));
            AssertSuccess(schedule, new DateTime(2006, 9, 18, 20, 00, 00).AddTicks(1L), new DateTime(2006, 9, 20, 20, 00, 00));
            AssertSuccess(schedule, new DateTime(2006, 9, 18, 22, 00, 00), new DateTime(2006, 9, 20, 20, 00, 00));

            //A tuesday
            AssertSuccess(schedule, new DateTime(2006, 9, 19, 11, 00, 00), new DateTime(2006, 9, 20, 20, 00, 00));
            AssertSuccess(schedule, new DateTime(2006, 9, 19, 20, 00, 00).AddTicks(-1L), new DateTime(2006, 9, 20, 20, 00, 00));
            AssertSuccess(schedule, new DateTime(2006, 9, 19, 20, 00, 00), new DateTime(2006, 9, 20, 20, 00, 00));
            AssertSuccess(schedule, new DateTime(2006, 9, 19, 20, 00, 00).AddTicks(1L), new DateTime(2006, 9, 20, 20, 00, 00));
            AssertSuccess(schedule, new DateTime(2006, 9, 19, 22, 00, 00), new DateTime(2006, 9, 20, 20, 00, 00));

            //A saturday
            AssertSuccess(schedule, new DateTime(2010, 9, 25, 11, 00, 00), new DateTime(2010, 9, 25, 20, 00, 00));
            AssertSuccess(schedule, new DateTime(2010, 9, 25, 20, 00, 00).AddTicks(-1L), new DateTime(2010, 9, 25, 20, 00, 00));
            AssertSuccess(schedule, new DateTime(2010, 9, 25, 20, 00, 00), new DateTime(2010, 9, 25, 20, 00, 00));
            AssertSuccess(schedule, new DateTime(2010, 9, 25, 20, 00, 00).AddTicks(1L), new DateTime(2010, 9, 27, 20, 00, 00));
            AssertSuccess(schedule, new DateTime(2010, 9, 25, 22, 00, 00), new DateTime(2010, 9, 27, 20, 00, 00));
        }

        private void AssertSuccess(ISchedule instance, DateTime targetTime, DateTime expectedTime)
        {
            Assert.IsTrue(instance.MoveToFirstFrom(targetTime));
            Assert.AreEqual(expectedTime, instance.CurrentOccurrence);
        }

    }
}
