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
        [Test, Category("TimingSensitive")]
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

                Assert.That(wasStopped);

                // In CI environments, stopping might take a bit longer due to virtualization overhead
                // Wait up to 2 seconds for the scheduler to fully stop
                var stopTimeout = DateTime.Now.AddSeconds(2);
                while (scheduler.IsRunning && DateTime.Now < stopTimeout)
                {
                    Thread.Sleep(100);
                }

                Assert.That(scheduler.IsRunning, Is.False, "Scheduler should have stopped after calling Stop() in the event handler");
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
                Assert.IsTrue(alarmListener.WaitForEndOfSchedule(3000), "Did not receive expected event");
                alarmListener.AssertExpectedTriggerCount();

                scheduler.Stop();
            }

            Assert.AreEqual(1, oneCount);
            Assert.AreEqual(1, twoCount);
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

                scheduler[0].Alarm += delegate (object sender, AlarmEventArgs e)
                                          {
                                              oneSignalTime.Add(e.SignalTime);
                                              oneCount++;
                                          };
                scheduler[1].Alarm += delegate (object sender, AlarmEventArgs e)
                                          {
                                              twoSignalTime.Add(e.SignalTime);
                                              twoCount++;
                                          };

                scheduler.Start();
                Assert.IsTrue(alarmListener1.WaitForScheduledAlarm(2700), "Did not receive expected event");
                scheduler.Stop();
            }

            Assert.IsTrue(oneCount >= 1 && oneCount < 3);
            Assert.IsTrue(twoCount >= 1 && twoCount < 3);
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
            Assert.AreEqual(expected, actual);
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

                Assert.AreEqual(1, target.Count);
            }
        }

        [Test]
        public void AddTaskTest()
        {
            using (var target = new Scheduler())
            {
                target.Tasks.Add(new TaskMock());
                Assert.AreEqual(1, target.Tasks.Count);
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
                Assert.AreEqual(1, target.Tasks.Count);
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

            Assert.AreEqual(2, target.Count);
        }

        [Test]
        public void IsRunningTest()
        {
            using (var target = SchedulerMock.Create())
            {
                target.Add(new OneShotSchedule(DateTime.Now.AddDays(1)));
                Assert.IsFalse(target.IsRunning);

                target.Start();
                Assert.IsTrue(target.IsRunning);

                target.Stop();
                Assert.IsFalse(target.IsRunning);
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
                Assert.IsFalse(target.IsRunning);
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
                Assert.IsNotNull(actual,
                                 "DemiCode.Scheduling.SchedulerFactory.CreateFromConfig did not return the expected value.");
                Assert.AreEqual(2, actual.Count);
            }
        }

        /// <summary>
        /// Use this test to measure memory consumption during repeatedly allocation/disposal of schedulers.
        /// </summary>
        [Test]
        public void MemoryUsageTest()
        {
            // "Touch" our library to load it
            var sc = new[] { typeof(Scheduler), typeof(DateElement), typeof(LogManager) };

            long memoryBefore = GC.GetTotalMemory(true);

            int count = 100;
            while (--count > 0)
            {
                using (var scheduler = SchedulerFactory.CreateFromConfig("scheduling"))
                {
                    scheduler.Start();
                    Thread.Sleep(2);
                    scheduler.Stop();
                }
            }

            Thread.Sleep(500);
            long memoryAfter = GC.GetTotalMemory(true);

            long diff = memoryAfter - memoryBefore;
            Assert.IsTrue(diff < 70000, "Memory difference should be about or below 64K, was {0} bytes", diff);
        }

        /// <summary>
        ///Create Scheduler from config. Uses a named scheduler.
        ///</summary>
        [Test]
        public void CreateNamedSchedulerFromConfigTest()
        {
            using (Scheduler actual = SchedulerFactory.CreateFromConfig("scheduling", "test"))
            {
                Assert.IsNotNull(actual,
                                 "DemiCode.Scheduling.SchedulerFactory.CreateFromConfig did not return the expected value.");
                Assert.AreEqual(0, actual.Count);
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
                Assert.IsNotNull(schedule);
                schedule = actual["counted"];
                Assert.IsNotNull(schedule);
                schedule = actual["weekly"];
                Assert.IsNotNull(schedule);
                schedule = actual["oneshot"];
                Assert.IsNotNull(schedule);
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
                Assert.IsNotNull(actual["WeeklySchedule"]);
                Assert.IsNotNull(actual["WeeklySchedule2"]);
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
                Assert.IsFalse(target.Enabled, "DemiCode.Scheduling.Scheduler.Enabled was not set correctly.");

                target.Enabled = true;
                Assert.IsTrue(target.Enabled, "DemiCode.Scheduling.Scheduler.Enabled was not set correctly.");
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
                    Assert.IsNotNull(schedule as OneShotSchedule);
                }

                IEnumerator enumerator = ((IEnumerable)scheduler).GetEnumerator();
                Assert.IsNotNull(enumerator);
                Assert.IsTrue(enumerator.MoveNext());
            }
        }

        [Test]
        public void IndexByNameTest()
        {
            using (var target = new Scheduler())
            {
                target.Add(new OneShotSchedule("FirstSchedule", DateTime.Now));
                target.Add(new OneShotSchedule("SecondSchedule", DateTime.Now.AddDays(1)));

                Assert.AreEqual("FirstSchedule", target["FirstSchedule"].Name);
                Assert.AreEqual("SecondSchedule", target["SecondSchedule"].Name);
            }
        }

        [Test]
        public void IndexByIndexTest()
        {
            using (var target = new Scheduler())
            {
                target.Add(new OneShotSchedule("FirstSchedule", DateTime.Now));
                target.Add(new OneShotSchedule("SecondSchedule", DateTime.Now.AddDays(1)));

                Assert.AreEqual("FirstSchedule", target[0].Name);
                Assert.AreEqual("SecondSchedule", target[1].Name);
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

                Assert.AreEqual(180, eventCounter);
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
                int timeout = 3 * triggerMilliseconds - (int)diff;
                bool result = alarmListener.WaitForScheduledAlarm(timeout);

                Assert.IsTrue(result, "Did not receive expected event within timeout {0} milliseconds. Diff was {1}",
                              timeout, diff);
                alarmListener.AssertExpectedTriggerCount();
            }
        }

        /// <summary>
        ///A test for Scheduler (IEnumerable&lt;ISchedule&gt;)
        ///</summary>
        [Test]
        public void ConstructorWithScheduleArrayTest()
        {
            IEnumerable<ISchedule> schedules = new ISchedule[] { new OneShotSchedule(DateTime.Now) };
            using (var target = new Scheduler(schedules))
            {
                Assert.AreEqual(1, target.Count);
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
                Assert.AreEqual(1, target.Count);
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
            //     Assert.AreEqual(3, target.Count);
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

                Assert.IsFalse(actual, "DemiCode.Scheduling.Scheduler.Start did not return the expected value.");
            }
        }

        [Test]
        public void StartWithScheduleAlreadyOverTest()
        {
            using (var target = SchedulerMock.Create())
            {
                target.Add(new OneShotSchedule(DateTime.Now.AddDays(-1)));
                bool actual = target.Start();

                Assert.AreEqual(0, target.Count);
                Assert.IsFalse(actual, "DemiCode.Scheduling.Scheduler.Start did not return the expected value.");
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
                Assert.IsFalse(target.Enabled, "DemiCode.Scheduling.Scheduler.Stop did not yield the expected state.");
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

                Assert.IsNotNull(actual, "DemiCode.Scheduling.Scheduler.ToString did not return the expected value.");
                Assert.IsTrue(actual.Length > 0,
                              "DemiCode.Scheduling.Scheduler.ToString did not return the expected value.");
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
                Assert.IsTrue(alarmListener.WaitForScheduledAlarm(2000));

                Thread.Sleep(500);

                // Check that the task was executed
                Assert.IsTrue(task.WasExecuted);

                alarmListener.AssertExpectedTriggerCount();
            }
        }

        [Test]
        public void ExecuteTaskFromConfigTest()
        {
            using (var scheduler = SchedulerFactory.CreateFromConfig("SimpleWithLogging"))
            {
                var alarmListener = new ScheduledAlarmListener(scheduler, 1, -1);
                scheduler.Add(new OneShotSchedule(DateTime.Now.AddSeconds(1)));

                var task = new TaskMock();
                scheduler.Tasks.Add(task);

                scheduler.Start();

                // Wait for the schedule to trigger
                Assert.IsTrue(alarmListener.WaitForScheduledAlarm(2000));

                // We must sleep for the events to finish handling BEFORE we dispose our Scheduler
                Thread.Sleep(500);

                // Check that the task was executed
                Assert.IsTrue(task.WasExecuted);
                alarmListener.AssertExpectedTriggerCount();
            }
        }

        [Test]
        public void ExecuteTaskOnSpecificScheduleFromConfigTest()
        {
            using (
                var target = SchedulerFactory.CreateFromConfig("TaskAndLoggingSection",
                                                                     "unittest_task_execution_withschedulerefs_unittest")
                )
            {
                var task = (TaskMock)target.Tasks[0];
                var alarmListener = new ScheduledAlarmListener(target, "interval", 1, -1);

                target.Start();

                // Wait for the schedule to trigger (wait max 6 seconds for the intervalschedule to trigger)
                Assert.IsTrue(alarmListener.WaitForScheduledAlarm(6000));
                alarmListener.AssertExpectedTriggerCount();

                // Check that the task was executed
                Assert.IsTrue(task.WasExecuted);
            }
        }

        [Test]
        public void ExecuteWithLog4netLoggingTest()
        {
            using (
                var target = SchedulerFactory.CreateFromConfig("TaskAndLoggingSection",
                                                                     "config_with_log4netlogger_used_in_unittests"))
            {
                var alarmListener = new ScheduledAlarmListener(target, "interval", 1, -1);

                target.Start();

                // Wait for the schedule to trigger (wait max 6 seconds for the intervalschedule to trigger)
                Assert.IsTrue(alarmListener.WaitForScheduledAlarm(6000));
                alarmListener.AssertExpectedTriggerCount();
            }
        }
    }
}
