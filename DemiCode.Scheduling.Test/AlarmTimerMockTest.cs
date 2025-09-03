using System;
using NUnit.Framework;
using System.Threading;
using DemiCode.Scheduling.Test.Mocks;
using DemiCode.Scheduling.Alarms;

namespace DemiCode.Scheduling.Test
{
    /// <summary>
    /// Summary description for AlarmTimerMockTest
    /// </summary>
    [TestFixture]
    public class AlarmTimerMockTest
    {
        /// <summary>
        ///A test for ElapsedEvent in AlarmTimerMock
        ///</summary>
        [Test]
        public void ElapsedEventMockTest()
        {
            using (AutoResetEvent trigger = new AutoResetEvent(false))
            {
                AlarmTimerMock target = new AlarmTimerMock();
                target.Elapsed += ((sender, e) => trigger.Set());

                DateTime alarmTime = DateTime.Now.AddDays(100);
                target.Enable(alarmTime);
                target.FireEvent();

                bool receivedSignal = trigger.WaitOne(500, false);
                receivedSignal.Should().BeTrue();
            }
        }
    }
}
