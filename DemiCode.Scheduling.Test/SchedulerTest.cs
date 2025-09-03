using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Xml;
using DemiCode.Configuration;
using DemiCode.Logging;
using DemiCode.Logging.log4net;
using DemiCode.Scheduling.Alarms;
using DemiCode.Scheduling.Schedules;
using DemiCode.Scheduling.Tasks;
using DemiCode.Scheduling.Test.Mocks;
using log4net;
using NUnit.Framework;

namespace DemiCode.Scheduling.Test
{
    [TestFixture]
    public class SchedulerTest
    {
        /// <summary>
        /// This test checks that <see cref="Scheduler.Stop"/> can be called from a schedule event handler.
        /// Proves that case 9080 is fixed.
        /// </summary>
        [Test]
        public void Stop_CalledDuringOccurence_StopsTheScheduler()
        {
            var now = DateTime.Now.AddMilliseconds(500);
            var list = new List<ISchedule> { new IntervalSchedule("one", now, TimeSpan.FromSeconds(1)) };

            using (var scheduler = SchedulerFactory.Create(list))
            {
                var alarmListener = new ScheduledAlarmListener(scheduler);

                var isStarted = true;
                var wasStopped = false;
                scheduler[0].Alarm += ((sender, args) =>
                                           {
                                               if (isStarted)
                                               {
                                                   isStarted = false;
                                                   wasStopped = true;
                                                   scheduler.Stop();
                                               }
                                               else
                                               {
                                                   wasStopped = false;
                                               }
                                           });

                scheduler.Start();

                Thread.Sleep(3000);
                alarmListener.AssertScheduledAlarm(5000);

                wasStopped.Should().BeTrue();
                scheduler.IsRunning.Should().BeFalse();
            }
        }

        /// <summary>
        /// This test checks that two schedules that <i>almost</i> matches on occurence is actually fired.
        /// The test proves that case 1190 is fixed.
        /// </summary>
        [Test]
        public void SchedulesDifferingByTicksTest()
        {
            var now = DateTime.Now.AddMilliseconds(2000);
            var list = new List<ISchedule>
                           {new OneShotSchedule("one", now), new OneShotSchedule("two", now.AddTicks(10))};

            int oneCount = 0;
            int twoCount = 0;
            

            using (var scheduler = SchedulerFactory.Create(list))
            {
                var alarmListener = new ScheduledAlarmListener(scheduler, 2, 1);

                scheduler[0].Alarm += delegate { oneCount++; };
                scheduler[1].Alarm += delegate { twoCount++; };

                scheduler.Start();
                alarmListener.WaitForEndOfSchedule(3000).Should().BeTrue();
                alarmListener.AssertExpectedTriggerCount();

                scheduler.Stop();
            }

            oneCount.Should().Be(1);
            twoCount.Should().Be(1);
        }

        [Test]
        public void SchedulesDifferingByMillisecondsTest()
        {
            var interval = TimeSpan.FromSeconds(1);
            var now = DateTime.Now;
            now = now.AddMilliseconds(500);
            var list = new List<ISchedule>
                           {
                               new IntervalSchedule("one", now, interval),
                               new IntervalSchedule("two", now.AddMilliseconds(10), interval)
                           };

            var oneCount = 0;
            var twoCount = 0;
            var oneSignalTime = new List<DateTime>(4);
            var twoSignalTime = new List<DateTime>(4);

            ILoggerFactory logger = new Log4netLoggerFactory();
            using (var scheduler = SchedulerFactory.Create(list, logger))
            {
                var alarmListener1 = new ScheduledAlarmListener(scheduler, 2, -1);

                scheduler[0].Alarm += delegate(object sender, AlarmEventArgs e)
                                          {
                                              oneSignalTime.Add(e.SignalTime);
                                              oneCount++;
                                          };
                scheduler[1].Alarm += delegate(object sender, AlarmEventArgs e)
                                          {
                                              twoSignalTime.Add(e.SignalTime);
                                              twoCount++;
                                          };

                scheduler.Start();
                alarmListener1.WaitForScheduledAlarm(2700).Should().BeTrue();
                scheduler.Stop();
            }

            (oneCount >= 1 && oneCount < 3).Should().BeTrue();
            (twoCount >= 1 && twoCount < 3).Should().BeTrue();
        }

        [Test]
        public void RoundToNearestSecondTest()
        {
            RoundToNearestSecondAssert(new DateTime(2008, 1, 1, 1, 1, 2, 0), new DateTime(2008, 1, 1, 1, 1, 2, 0));
            RoundToNearestSecondAssert(new DateTime(2008, 1, 1, 1, 1, 2, 499), new DateTime(2008, 1, 1, 1, 1, 2, 0));
            RoundToNearestSecondAssert(new DateTime(2008, 1, 1, 1, 1, 2, 500), new DateTime(2008, 1, 1, 1, 1, 2, 0));
            RoundToNearestSecondAssert(new DateTime(2008, 1, 1, 1, 1, 2, 501), new DateTime(2008, 1, 1, 1, 1, 3, 0));
            RoundToNearestSecondAssert(new DateTime(2008, 1, 1, 1, 1, 2, 510), new DateTime(2008, 1, 1, 1, 1, 3, 0));
            RoundToNearestSecondAssert(new DateTime(2008, 1, 1, 1, 1, 2, 900), new DateTime(2008, 1, 1, 1, 1, 3, 0));
            RoundToNearestSecondAssert(new DateTime(2008, 1, 1, 1, 1, 2, 999), new DateTime(2008, 1, 1, 1, 1, 3, 0));
            RoundToNearestSecondAssert(new DateTime(2008, 1, 1, 1, 1, 59, 999), new DateTime(2008, 1, 1, 1, 2, 0, 0));
        }

        private static void RoundToNearestSecondAssert(DateTime input, DateTime expected)
        {
            DateTime actual = Scheduler.RoundToNearestSecond(input);
            actual.Should().Be(expected);
        }

        /// <summary>
        ///A test for Add (ISchedule)
        ///</summary>
        [Test]
        public void AddTest()
        {
            using (var target = new Scheduler())
            {
                target.Add(new OneShotSchedule(DateTime.Now));

                target.Count.Should().Be(1);
            }
        }

        [Test]
        public void AddTaskTest()
        {
            using (var target = new Scheduler())
            {
                target.Tasks.Add(new TaskMock());
                target.Tasks.Count.Should().Be(1);
            }
        }

        [Test]
        public void AddNullTaskTest()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                using (var target = new Scheduler())
                {
                    const ITask task = null;
                    target.Tasks.Add(task);
                }
            });
        }

        [Test]
        public void TaskCountTest()
        {
            using (var target = new Scheduler())
            {
                target.Tasks.Add(new TaskMock());
                target.Tasks.Count.Should().Be(1);
            }
        }

        /// <summary>
        ///A test for Add (ISchedule)
        ///</summary>
        [Test]
        public void AddToEnabledSchedulerTest()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                using (var target = SchedulerMock.Create())
                {
                    target.Add(new OneShotSchedule(DateTime.Now.AddDays(1)));
                    target.Start();
                    target.Add(new OneShotSchedule(DateTime.Now.AddDays(2)));
                }
            });
        }

        /// <summary>
        ///A test for Add (ISchedule)
        ///</summary>
        [Test]
        public void AddRangeToEnabledScheduler()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                var list = new List<ISchedule>
                               {
                                   new OneShotSchedule(DateTime.Now.AddDays(1)),
                                   new OneShotSchedule(DateTime.Now.AddDays(2))
                               };
                using (var target = SchedulerMock.Create())
                {
                    target.Add(new OneShotSchedule(DateTime.Now.AddDays(10)));
                    target.Start();
                    target.AddRange(list);
                }
            });
        }

        /// <summary>
        ///A test for AddRange (IEnumerable&lt;ISchedule&gt;)
        ///</summary>
        [Test]
        public void AddRangeTest()
        {
            var target = new Scheduler();
            IEnumerable<ISchedule> schedules = new ISchedule[]
                                                   {
                                                       new OneShotSchedule(DateTime.Now),
                                                       new OneShotSchedule(DateTime.Now.AddSeconds(5))
                                                   };
            target.AddRange(schedules);

            target.Count.Should().Be(2);
        }

        [Test]
        public void IsRunningTest()
        {
            using (var target = SchedulerMock.Create())
            {
                target.Add(new OneShotSchedule(DateTime.Now.AddDays(1)));
                target.IsRunning.Should().BeFalse();

                target.Start();
                target.IsRunning.Should().BeTrue();

                target.Stop();
                target.IsRunning.Should().BeFalse();
            }
        }

        /// <summary>
        /// No schedules should immediately stop the Scheduler.
        /// </summary>
        [Test]
        public void IsRunningGivesFalseWhenNoSchedules()
        {
            using (var target = SchedulerMock.Create())
            {
                target.Start();
                target.IsRunning.Should().BeFalse();
            }
        }

        /// <summary>
        ///Create Scheduler from config. Uses the defaultScheduler attribute.
        ///</summary>
        [Test]
        public void CreateDefaultSchedulerFromConfigTest()
        {
            using (Scheduler actual = SchedulerFactory.CreateFromConfig("scheduling"))
            {
                actual.Should().NotBeNull("DemiCode.Scheduling.Scheduler.CreateFromConfig did not return the expected value.");
                actual.Count.Should().Be(2);
            }
        }

        /// <summary>
        /// Use this test to measure memory consumption during repeatedly allocation/disposal of schedulers.
        /// </summary>
        [Test]
        public void MemoryUsageTest()
        {
            // "Touch" our library to load it
            var sc = new[] {typeof (Scheduler), typeof (DateElement), typeof (LogManager)};

            long memoryBefore = GC.GetTotalMemory(true);

            int count = 100;
            while (--count > 0)
            {
                using (var scheduler = Scheduler.CreateFromConfig("scheduling"))
                {
                    scheduler.Start();
                    Thread.Sleep(2);
                    scheduler.Stop();
                }
            }

            Thread.Sleep(500);
            long memoryAfter = GC.GetTotalMemory(true);

            long diff = memoryAfter - memoryBefore;
            (diff < 70000).Should().BeTrue();
        }

        /// <summary>
        ///Create Scheduler from config. Uses a named scheduler.
        ///</summary>
        [Test]
        public void CreateNamedSchedulerFromConfigTest()
        {
            using (Scheduler actual = SchedulerFactory.CreateFromConfig("scheduling", "test"))
            {
                actual.Should().NotBeNull("DemiCode.Scheduling.Scheduler.CreateFromConfig did not return the expected value.");
                actual.Count.Should().Be(0);
            }
        }


        /// <summary>
        /// Create Scheduler from config. Uses a named scheduler defined with the 
        /// short format.
        ///</summary>
        [Test]
        public void CreateFromShortFormatConfigTest()
        {
            using (Scheduler actual = SchedulerFactory.CreateFromConfig("shortformatconfig", "prod"))
            {
                ISchedule schedule = actual["interval"];
                schedule.Should().NotBeNull();
                schedule = actual["counted"];
                schedule.Should().NotBeNull();
                schedule = actual["weekly"];
                schedule.Should().NotBeNull();
                schedule = actual["oneshot"];
                schedule.Should().NotBeNull();
            }
        }

        /// <summary>
        ///Test schedule-by-name indexer
        ///</summary>
        [Test]
        public void ScheduleByNameIndexerTest()
        {
            using (Scheduler actual = SchedulerFactory.CreateFromConfig("scheduling", "prod"))
            {
                actual["WeeklySchedule"].Should().NotBeNull();
                actual["WeeklySchedule2"].Should().NotBeNull();
            }
        }

        /// <summary>
        ///A test for Enabled
        ///</summary>
        [Test]
        public void EnabledTest()
        {
            using (var target = new Scheduler())
            {
                target.Add(new OneShotSchedule(DateTime.Now.AddSeconds(5)));

                target.Enabled = false;
                target.Enabled.Should().BeFalse();

                target.Enabled = true;
                target.Enabled.Should().BeTrue();
            }
        }

        ///<summary>
        ///Test both generic and standard IEnumerable interfaces.
        ///</summary>
        [Test]
        public void EnumeratorTest()
        {
            using (
                var scheduler = new Scheduler
                                    {new OneShotSchedule(DateTime.Now), new OneShotSchedule(DateTime.Now.AddDays(1))})
            {
                foreach (ISchedule schedule in scheduler)
                {
                    (schedule as OneShotSchedule).Should().NotBeNull();
                }

                IEnumerator enumerator = ((IEnumerable) scheduler).GetEnumerator();
                enumerator.Should().NotBeNull();
                enumerator.MoveNext().Should().BeTrue();
            }
        }

        [Test]
        public void IndexByNameTest()
        {
            using (var target = new Scheduler())
            {
                target.Add(new OneShotSchedule("FirstSchedule", DateTime.Now));
                target.Add(new OneShotSchedule("SecondSchedule", DateTime.Now.AddDays(1)));

                target["FirstSchedule"].Name.Should().Be("FirstSchedule");
                target["SecondSchedule"].Name.Should().Be("SecondSchedule");
            }
        }

        [Test]
        public void IndexByIndexTest()
        {
            using (var target = new Scheduler())
            {
                target.Add(new OneShotSchedule("FirstSchedule", DateTime.Now));
                target.Add(new OneShotSchedule("SecondSchedule", DateTime.Now.AddDays(1)));

                target[0].Name.Should().Be("FirstSchedule");
                target[1].Name.Should().Be("SecondSchedule");
            }
        }

        ///<summary>
        ///Test schedule indexer.
        ///</summary>
        [Test]
        public void IndexerTest()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                using (var target = new Scheduler())
                {
                    target.Add(new OneShotSchedule(DateTime.Now));
                    target.Add(new OneShotSchedule(DateTime.Now.AddDays(1)));

                    ISchedule schedule = target["somebogusschedule"];
                }
            });
        }

        /// <summary>
        ///A test for Scheduler ()
        ///</summary>
        [Test]
        public void ConstructorTest()
        {
            using (var target = new Scheduler())
            {
            }
        }

        /// <summary>
        ///A test for Scheduler (IAlarmTimer alarmTimer)
        ///</summary>
        [Test]
        public void AlarmConstructorTest()
        {
            IAlarmTimer alarmTimer = new AlarmTimerMock();
            using (var target = new Scheduler(alarmTimer))
            {
            }
        }

        [Test]
        public void AlarmIntervalRepeatingTest()
        {
            var timeProvider = new TimeProviderMock();
            var alarmTimer = new AlarmTimerMock();
            using (var scheduler = new Scheduler(alarmTimer, timeProvider))
            {
                var alarmListener = new ScheduledAlarmListener(scheduler, 180, -1);
                var firstOccurence = new DateTime(2008, 01, 21, 09, 30, 00);
                TimeSpan interval = TimeSpan.FromSeconds(1);

                timeProvider.Now = firstOccurence;

                scheduler.Add(new IntervalSchedule(firstOccurence, interval));

                int eventCounter = 0;
                scheduler.ScheduledAlarm += delegate { eventCounter++; };
                scheduler.Start();

                // Simulate firing 3 minutes of events
                for (int i = 180; i > 0; i--)
                {
                    timeProvider.Now += interval;
                    if (alarmTimer.AlarmTime == timeProvider.Now)
                        alarmTimer.FireEvent();
                }

                eventCounter.Should().Be(180);
                alarmListener.AssertExpectedTriggerCount();
            }
        }

        [Test]
        public void SimpleElapsedTest()
        {
            const int triggerMilliseconds = 400;

            var schedules = new List<ISchedule>();
            using (Scheduler target = SchedulerFactory.Create(schedules, new Log4netLoggerFactory()))
            {
                var alarmListener = new ScheduledAlarmListener(target, 1, -1);

                DateTime then = DateTime.Now;
                target.Add(new OneShotSchedule(then.AddMilliseconds(triggerMilliseconds)));

                target.Start();

                double diff = TimeSpan.FromTicks(DateTime.Now.Ticks - then.Ticks).TotalMilliseconds;
                int timeout = 3*triggerMilliseconds - (int) diff;
                bool result = alarmListener.WaitForScheduledAlarm(timeout);

                result.Should().BeTrue();
                alarmListener.AssertExpectedTriggerCount();
            }
        }

        /// <summary>
        ///A test for Scheduler (IEnumerable&lt;ISchedule&gt;)
        ///</summary>
        [Test]
        public void ConstructorWithScheduleArrayTest()
        {
            IEnumerable<ISchedule> schedules = new ISchedule[] {new OneShotSchedule(DateTime.Now)};
            using (var target = new Scheduler(schedules))
            {
                target.Count.Should().Be(1);
            }
        }

        /// <summary>
        ///A test for Scheduler (ISchedule)
        ///</summary>
        [Test]
        public void ConstructorWithOneScheduleTest()
        {
            ISchedule schedule = new OneShotSchedule(DateTime.Now);
            using (var target = new Scheduler(schedule))
            {
                target.Count.Should().Be(1);
            }
        }

        /// <summary>
        ///A test for Scheduler (XmlNode)
        ///</summary>
        [Test]
        [Ignore("Scheduler(XmlNode) constructor has been removed. Use configuration-based approach instead.")]
        public void ConstructorWithXmlNodeConfigurationTest()
        {
            // The XML-based constructor was deprecated and removed in favor of configuration-based approach
            // Original test code:
            // var xd = new XmlDocument();
            // xd.Load(@"TestData\ScheduleConfiguration.xml");
            // XmlNode schedulerNode = xd.SelectSingleNode("TestData/SchedulerTest/schedules");
            //
            // using (var target = new Scheduler(schedulerNode))
            // {
            //     target.Count.Should().Be(3);
            //
            //     foreach (ISchedule schedule in target)
            //     {
            //         Assert.AreEqual(schedule.Name, schedule.GetType().Name);
            //     }
            // }
        }

        /// <summary>
        ///A test for Start ()
        ///</summary>
        [Test]
        public void StartTest()
        {
            using (var target = SchedulerMock.Create())
            {
                bool actual = target.Start();

                actual.Should().BeFalse();
            }
        }

        [Test]
        public void StartWithScheduleAlreadyOverTest()
        {
            using (var target = SchedulerMock.Create())
            {
                target.Add(new OneShotSchedule(DateTime.Now.AddDays(-1)));
                bool actual = target.Start();

                target.Count.Should().Be(0);
                actual.Should().BeFalse();
            }
        }

        /// <summary>
        ///A test for Stop ()
        ///</summary>
        [Test]
        public void StopTest()
        {
            using (var target = SchedulerMock.Create())
            {
                target.Stop();
                target.Enabled.Should().BeFalse();
            }
        }

        /// <summary>
        ///A test for ToString ()
        ///</summary>
        [Test]
        public void ToStringTest()
        {
            using (var target = SchedulerMock.Create())
            {
                target.Add(new OneShotSchedule(DateTime.Now));

                string actual = target.ToString();

                actual.Should().NotBeNull("DemiCode.Scheduling.Scheduler.ToString did not return the expected value.");
                (actual.Length > 0).Should().BeTrue();
            }
        }

        [Test]
        public void ExecuteTaskTest()
        {
            using (var scheduler = SchedulerMock.Create())
            {
                var alarmListener = new ScheduledAlarmListener(scheduler, 1, -1);

                scheduler.Add(new OneShotSchedule(DateTime.Now.AddMilliseconds(1000)));

                var task = new TaskMock();
                scheduler.Tasks.Add(task);

                scheduler.Start();

                // Wait for the schedule to trigger
                alarmListener.WaitForScheduledAlarm(2000).Should().BeTrue();

                Thread.Sleep(500);

                // Check that the task was executed
                task.WasExecuted.Should().BeTrue();

                alarmListener.AssertExpectedTriggerCount();
            }
        }

        [Test]
        public void ExecuteTaskFromConfigTest()
        {
            using (var scheduler = Scheduler.CreateFromConfig("SimpleWithLogging"))
            {
                var alarmListener = new ScheduledAlarmListener(scheduler, 1, -1);
                scheduler.Add(new OneShotSchedule(DateTime.Now.AddSeconds(1)));

                var task = new TaskMock();
                scheduler.Tasks.Add(task);

                scheduler.Start();

                // Wait for the schedule to trigger
                alarmListener.WaitForScheduledAlarm(2000).Should().BeTrue();

                // We must sleep for the events to finish handling BEFORE we dispose our Scheduler
                Thread.Sleep(500);

                // Check that the task was executed
                task.WasExecuted.Should().BeTrue();
                alarmListener.AssertExpectedTriggerCount();
            }
        }

        [Test]
        public void ExecuteTaskOnSpecificScheduleFromConfigTest()
        {
            using (
                var target = Scheduler.CreateFromConfig("TaskAndLoggingSection",
                                                                     "unittest_task_execution_withschedulerefs_unittest")
                )
            {
                var task = (TaskMock) target.Tasks[0];
                var alarmListener = new ScheduledAlarmListener(target, "interval", 1, -1);

                target.Start();

                // Wait for the schedule to trigger (wait max 6 seconds for the intervalschedule to trigger)
                alarmListener.WaitForScheduledAlarm(6000).Should().BeTrue();
                alarmListener.AssertExpectedTriggerCount();

                // Check that the task was executed
                task.WasExecuted.Should().BeTrue();
            }
        }

        [Test]
        public void ExecuteWithLog4netLoggingTest()
        {
            using (
                var target = Scheduler.CreateFromConfig("TaskAndLoggingSection",
                                                                     "config_with_log4netlogger_used_in_unittests"))
            {
                var alarmListener = new ScheduledAlarmListener(target, "interval", 1, -1);

                target.Start();

                // Wait for the schedule to trigger (wait max 6 seconds for the intervalschedule to trigger)
                alarmListener.WaitForScheduledAlarm(6000).Should().BeTrue();
                alarmListener.AssertExpectedTriggerCount();
            }
        }
    }
}
