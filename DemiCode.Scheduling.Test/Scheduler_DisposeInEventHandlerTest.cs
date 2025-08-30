using System;
using System.Collections.Generic;
using System.Threading;
using DemiCode.Scheduling.Alarms;
using DemiCode.Scheduling.Schedules;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace DemiCode.Scheduling.Test
{
    /// <summary>
    /// This test proves case 7797 fixed. Originally, one could not dispose and recreate a scheduler from with eg the <see cref="Scheduler.ScheduledAlarm"/> event.
    /// This would cause the disposed instance to live on and invoke its schedules.
    /// </summary>
    [TestFixture]
    public class Scheduler_DisposeInEventHandlerTest
    {
        private Dictionary<string, int> _dispose_InAlarmEventHandler_dict;
        private Scheduler _dispose_InAlarmEventHandler_scheduler;

        [Test]
        public void Dispose_InAlarmEventHandler()
        {
            _dispose_InAlarmEventHandler_dict = new Dictionary<string, int>(2);
            _dispose_InAlarmEventHandler_dict["first"] = 0;
            _dispose_InAlarmEventHandler_dict["second"] = 0;


            try
            {
                _dispose_InAlarmEventHandler_scheduler = SchedulerFactory.Create();

                var intervalCountedSchedule = new IntervalCountedSchedule("first", DateTime.Now.AddSeconds(1), 10, TimeSpan.FromSeconds(1), "first");
                _dispose_InAlarmEventHandler_scheduler.Add(intervalCountedSchedule);
                intervalCountedSchedule.Alarm += Dispose_InAlarmEventHandler_EventHandler;
                _dispose_InAlarmEventHandler_scheduler.Start();

                Thread.Sleep(3000);

            }
            finally
            {
                if (_dispose_InAlarmEventHandler_scheduler != null)
                    _dispose_InAlarmEventHandler_scheduler.Dispose();
            }

            Assert.That(_dispose_InAlarmEventHandler_dict["first"], Is.EqualTo(1));
            Assert.That(_dispose_InAlarmEventHandler_dict["second"], Is.GreaterThan(0));
        }

        void Dispose_InAlarmEventHandler_EventHandler(object sender, AlarmEventArgs e)
        {
            var schedule = e.AdditionalData as string;
            if (schedule == null) return;

            if (schedule == "first")
            {
                if (_dispose_InAlarmEventHandler_scheduler != null)
                {
                    _dispose_InAlarmEventHandler_scheduler.Dispose();

                    _dispose_InAlarmEventHandler_scheduler = SchedulerFactory.Create();
                    var interval = new IntervalSchedule("second", DateTime.Now.AddDays(-1), TimeSpan.FromSeconds(1), "second");
                    _dispose_InAlarmEventHandler_scheduler.Add(interval);
                    interval.Alarm += Dispose_InAlarmEventHandler_EventHandler;
                    _dispose_InAlarmEventHandler_scheduler.Start();

                }

            }

            _dispose_InAlarmEventHandler_dict[schedule] += 1;
        }
    }
}