using System;
using System.Threading;
using DemiCode.Scheduling.Schedules;
using NUnit.Framework;

namespace DemiCode.Scheduling.Test
{
    internal class ScheduledAlarmListener
    {
        private readonly AutoResetEvent[] _triggers = new[]{
                new AutoResetEvent(false),
                new AutoResetEvent(false)
            };

        private readonly int[] _triggerCounts = new[] { 0, 0 };
        private readonly int _expectedScheduledAlarmCount;
        private readonly int _expectedEndOfScheduleCount;

        private readonly bool _globalAlarm;
        private readonly Scheduler _scheduler;

        public int ScheduledAlarmCount { get { return _triggerCounts[0]; } }
        public int EndOfScheduleCount { get { return _triggerCounts[1]; } }

        /// <summary>
        /// Listen for the <see cref="Scheduler.ScheduledAlarm"/> and <see cref="Scheduler.EndOfSchedule"/> alarms.
        /// </summary>
        public ScheduledAlarmListener(Scheduler scheduler)
            : this(scheduler, -1, -1)
        { }

        /// <summary>
        /// Listen for the <see cref="Scheduler.ScheduledAlarm"/> and <see cref="Scheduler.EndOfSchedule"/> alarms.
        /// </summary>
        public ScheduledAlarmListener(Scheduler scheduler, int expectedScheduledAlarmCount, int expectedEndOfScheduleCount)
        {
            _expectedScheduledAlarmCount = expectedScheduledAlarmCount;
            _expectedEndOfScheduleCount = expectedEndOfScheduleCount;
            _scheduler = scheduler;
            _globalAlarm = true;
            _scheduler.ScheduledAlarm += delegate
                                             {
                                                 try
                                                 {
                                                     _triggerCounts[0]++;
                                                     if (expectedScheduledAlarmCount == -1 || expectedScheduledAlarmCount == _triggerCounts[0])
                                                     {
                                                         _triggers[0].Set();
                                                     }
                                                 }
                                                 catch (Exception)
                                                 {
                                                 }
                                             };

            _scheduler.EndOfSchedule += delegate
                                            {
                                                try
                                                {
                                                    _triggerCounts[1]++;
                                                    _triggers[1].Set();
                                                }
                                                catch (Exception)
                                                {
                                                }
                                            };
        }

        /// <summary>
        /// Listen for the <see cref="ISchedule.Alarm"/> on the named schedule.
        /// </summary>
        public ScheduledAlarmListener(Scheduler scheduler, string scheduleName)
            : this(scheduler, scheduleName, -1, -1)
        {
        }

        /// <summary>
        /// Listen for the <see cref="ISchedule.Alarm"/> on the named schedule.
        /// </summary>
        public ScheduledAlarmListener(Scheduler scheduler, string scheduleName, int expectedScheduledAlarmCount, int expectedEndOfScheduleCount)
        {
            _expectedScheduledAlarmCount = expectedScheduledAlarmCount;
            _expectedEndOfScheduleCount = expectedEndOfScheduleCount;
            _scheduler = scheduler;
            _globalAlarm = false;
            _scheduler[scheduleName].Alarm += delegate
                                                  {
                                                      _triggerCounts[0]++;
                                                      _triggers[0].Set();
                                                  };
        }

        /// <summary>
        /// Wait for <see cref="Scheduler.ScheduledAlarm"/> and <see cref="Scheduler.EndOfSchedule"/>.
        /// Only use with the <see cref="ScheduledAlarmListener(Scheduler)"/> constructor.
        /// </summary>
        /// <param name="milliseconds">Milliseconds to wait</param>
        /// <returns>true if both events was fired, false if not or timeout occured</returns>
        public bool Wait(int milliseconds)
        {
            //NOTE: would love to use the WaitHandle.WaitAll here to get the atomic operation
            //of waiting for both trigger. But MSTest runs us in a STA apartment and thus WaitAll
            //fails since it requires multithreaded apt.

            if (!_globalAlarm)
            {
                throw new InvalidOperationException("Cannot wait for global events. Use correct constructor");
            }
            var result = _triggers[0].WaitOne(milliseconds, true);
            return result && _triggers[1].WaitOne(milliseconds, true);
        }

        /// <summary>
        /// Wait for <see cref="Scheduler.ScheduledAlarm"/> or <see cref="ISchedule.Alarm"/>.
        /// </summary>
        /// <param name="milliseconds">Milliseconds to wait</param>
        /// <returns>true if the event was fired, false if not or timeout occured</returns>
        public bool WaitForScheduledAlarm(int milliseconds)
        {
            return _triggers[0].WaitOne(milliseconds, true);
        }

        /// <summary>
        /// Assert that <see cref="Scheduler.ScheduledAlarm"/> or <see cref="ISchedule.Alarm"/> is fired within the given timeout.
        /// </summary>
        /// <param name="milliseconds">Timeout in milliseconds</param>
        public void AssertScheduledAlarm(int milliseconds)
        {
            Assert.That(WaitForScheduledAlarm(milliseconds),
                        "ScheduledAlarm/Alarm event not received within {0} milliseconds", milliseconds);
        }

        /// <summary>
        /// Wait for <see cref="Scheduler.EndOfSchedule"/>.
        /// </summary>
        /// <param name="milliseconds">Milliseconds to wait</param>
        /// <returns>true if the event was fired, false if not or timeout occured</returns>
        public bool WaitForEndOfSchedule(int milliseconds)
        {
            return _triggers[1].WaitOne(milliseconds, true);
        }

        /// <summary>
        /// Assert on <see cref="DidMatchExpectedTriggerCount"/>.
        /// </summary>
        public void AssertExpectedTriggerCount()
        {
            Assert.IsTrue(DidMatchExpectedTriggerCount, "ScheduledAlarm/Alarm triggered {0} times (expected {2}), EndOfSchedule triggered {1} times (expected {3})", ScheduledAlarmCount, EndOfScheduleCount, _expectedScheduledAlarmCount, _expectedEndOfScheduleCount);
        }

        public bool DidMatchExpectedTriggerCount
        {
            get
            {
                var res = true;
                if (_expectedScheduledAlarmCount != -1)
                {
                    res = (_expectedScheduledAlarmCount == ScheduledAlarmCount);
                }

                if (_expectedEndOfScheduleCount != -1)
                {
                    res = res && (_expectedEndOfScheduleCount == EndOfScheduleCount);
                }

                return res;
            }
        }
    }
}
