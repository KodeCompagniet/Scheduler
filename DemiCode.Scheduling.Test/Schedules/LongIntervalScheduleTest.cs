using DemiCode.Scheduling.Test;
using DemiCode.Scheduling.Test.Mocks;
using NUnit.Framework;
using System;
using System.Threading;

namespace DemiCode.Scheduling.Schedules.Test
{
    [TestFixture]
    public class LongIntervalScheduleTest
    {

        /// <summary>
        /// Test loading an 24-hour interval schedule from config and via construction.
        /// </summary>
        [Test]
        public void LongIntervalTest()
        {
            var schedule = new IntervalSchedule(new DateTime(2008, 1, 1, 0, 0, 0), new TimeSpan(364, 0, 0, 0));
            using (var scheduler = SchedulerMock.Create())
            {
                scheduler.Add(schedule);
                scheduler.Start();
                Thread.Sleep(500);
                scheduler.Stop();
            }
        }
    }
}
