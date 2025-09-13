using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Xml;
using DemiCode.Scheduling.Alarms;
using DemiCode.Scheduling.Test;
using DemiCode.Scheduling.Test.Mocks;
using NUnit.Framework;

namespace DemiCode.Scheduling.Schedules.Test
{
    /// <summary>
    ///This is a test class for DemiCode.Scheduling.IntervalSchedule and is intended
    ///to contain all DemiCode.Scheduling.IntervalSchedule Unit Tests
    ///</summary>
    [TestFixture]
    public class IntervalScheduleTest
    {
        /// <summary>
        /// Test loading an 24-hour interval schedule from config and via construction.
        /// </summary>
        [Test]
        public void TwentyFourHourIntervalTest()
        {
            var schedule = new IntervalSchedule(new DateTime(2008, 1, 1, 0, 0, 0), new TimeSpan(24, 0, 0));
            using (Scheduler scheduler = SchedulerFactory.CreateFromConfig("TwentyFourHourInterval"))
            {
            }
        }

        /// <summary>
        /// This test is ment to excercise what happens if the system clock is changed during an interval.
        /// If the clock is moved forward, eg 1 second, the observed events will fire as expected and SignalTime will
        /// be "correct". ActualSignalTime +1 second and the real difference between the previous and next
        /// event will be reduced by 1 second.
        /// 
        /// If the clock is moved back one second, the observed events will for that interval seem to fire twice at one
        /// event, thus the SignalTime will be equal for two consequtive events. The ActualSignalTime will be -1 second
        /// compared to the expected time and the real difference between the previous and next
        /// event will be reduced by 1 second.
        /// </summary>
        [Test, Category("SystemTimeChanges")]
        public void ChangeSystemClockDuringIntervalTest()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                Assert.Ignore("System time manipulation is only supported on Windows");

            DateTime previous = DateTime.MinValue;
            bool invalidDiff = false;
            var sw = new Stopwatch();

            TimeSpan skewTime = TimeSpan.FromSeconds(-1);
            TimeSpan intervalTime = TimeSpan.FromSeconds(2);

            using (TimeHelper.Change(new DateTime(2008, 1, 1, 0, 0, 0, DateTimeKind.Utc)))
            {
                var schedules = new ISchedule[0];
                using (Scheduler scheduler = SchedulerFactory.Create(schedules))
                {
                    var sched =
                        new IntervalSchedule(new DateTime(2008, 1, 1, 0, 0, 0), intervalTime);
                    scheduler.Add(sched);

                    sched.Alarm +=
                        delegate(object sender, AlarmEventArgs e)
                            {
                                sw.Stop();
                                Console.WriteLine(
                                    "{0:yyyyMMdd HH:mm:ss.fffffff} - {1:yyyyMMdd HH:mm:ss.fffffff} - elapsed {2}",
                                    e.ActualSignalTime, e.SignalTime, sw.ElapsedMilliseconds);
                                if (previous != DateTime.MinValue)
                                {
                                    double diff = e.SignalTime.Subtract(previous).TotalSeconds;
                                    if (diff != 2)
                                        invalidDiff = true;
                                }
                                sw.Reset();
                                sw.Start();
                                previous = e.SignalTime;
                            };

                    sw.Start();
                    scheduler.Start();
                    Thread.Sleep(1000);

                    // Wait until we're in the middle of an interval
                    TimeSpan toWait = sched.CurrentOccurrence.AddSeconds(1).Subtract(DateTime.Now);
                    Thread.Sleep(toWait);

                    Console.WriteLine("Scewing time +1 second at {0:hh:mm:ss.ffff}", DateTime.Now);

                    // Then scew the clock
                    DateTime now = TimeHelper.GetTime();
                    TimeHelper.SetTime(now.Add(skewTime));

                    Thread.Sleep(6000);

                    scheduler.Stop();
                    sw.Stop();
                }
            }
            Assert.IsFalse(invalidDiff);
        }

        [Test, Category("SystemTimeChanges")]
        public void MidnightCrossingTest()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                Assert.Ignore("System time manipulation is only supported on Windows");

            DateTime previous = DateTime.MinValue;
            bool invalidDiff = false;

            using (TimeHelper.Change(new DateTime(2006, 1, 1, 23, 59, 49)))
            {
                var schedules = new List<ISchedule>(1);
                using (Scheduler scheduler = SchedulerFactory.Create(schedules))
                {
                    var sched =
                        new IntervalSchedule(new DateTime(2006, 1, 1, 23, 59, 50), TimeSpan.FromSeconds(2));
                    scheduler.Add(sched);

                    sched.Alarm +=
                        delegate(object sender, AlarmEventArgs e)
                            {
                                Console.WriteLine("{0:yyyyMMdd HH:mm:ss.fffffff} - {1:yyyyMMdd HH:mm:ss.fffffff}",
                                                  e.ActualSignalTime, e.SignalTime);
                                if (previous != DateTime.MinValue)
                                {
                                    if (e.SignalTime.Subtract(previous).TotalSeconds != 2)
                                        invalidDiff = true;
                                }
                                previous = e.SignalTime;
                            };

                    scheduler.Start();

                    Thread.Sleep(14000);

                    scheduler.Stop();
                }
            }

            Assert.IsFalse(invalidDiff);
        }

        /// <summary>
        /// In the U.S., clocks change at 2:00 a.m. local time. 
        /// In spring, clocks spring forward from 1:59 a.m. to 3:00 a.m.; in fall, clocks fall back from 1:59 a.m. to 1:00 a.m. 
        /// 
        /// In the EU, clocks change at 1:00 a.m. Universal Time. 
        /// In spring, clocks spring forward from 12:59 a.m. to 2:00 a.m.; in fall, clocks fall back from 1:59 a.m. to 1:00 a.m.
        /// </summary>
        [Test, Category("SystemTimeChanges")]
        public void DaylightSavingCrossingTest()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                Assert.Ignore("System time manipulation is only supported on Windows");

            DateTime previous = DateTime.MinValue;
            bool invalidDiff = false;

            using (TimeHelper.Change(new DateTime(2008, 3, 30, 0, 59, 56, DateTimeKind.Utc)))
            {
                using (Scheduler scheduler = SchedulerFactory.CreateFromConfig("EmptyWithLogging"))
                {
                    var sched =
                        new IntervalSchedule(new DateTime(2008, 1, 1, 0, 0, 0), TimeSpan.FromSeconds(2));
                    scheduler.Add(sched);

                    sched.Alarm +=
                        delegate(object sender, AlarmEventArgs e)
                            {
                                Console.WriteLine("{0:yyyyMMdd HH:mm:ss.fffffff} - {1:yyyyMMdd HH:mm:ss.fffffff}",
                                                  e.ActualSignalTime, e.SignalTime);
                                if (previous != DateTime.MinValue)
                                {
                                    double diff = e.SignalTime.Subtract(previous).TotalSeconds;

                                    //NOTE: we'll allow a 1 hour + 2 sec diff since this will occur when the system "daylight-saves"
                                    if (diff != 2 && diff != 3602)
                                        invalidDiff = true;
                                }
                                previous = e.SignalTime;
                            };

                    scheduler.Start();

                    Thread.Sleep(10000);

                    scheduler.Stop();
                }
            }

            Assert.IsFalse(invalidDiff);
        }

        [Test]
        public void TwoHourDaylightSavingCrossingTest()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                Assert.Ignore("System time manipulation is only supported on Windows");

            DateTime previous = DateTime.MinValue;
            bool invalidDiff = false;

            using (TimeHelper.Change(new DateTime(2008, 3, 30, 0, 59, 54, DateTimeKind.Utc)))
            {
                using (Scheduler scheduler = SchedulerFactory.CreateFromConfig("EmptyWithLogging"))
                {
                    var sched =
                        new IntervalSchedule(new DateTime(2008, 3, 30, 0, 0, 0), TimeSpan.FromHours(2));
                    scheduler.Add(sched);
                    scheduler.Add(new OneShotSchedule(new DateTime(2008, 3, 30, 2, 0, 2)));

                    sched.Alarm +=
                        delegate(object sender, AlarmEventArgs e)
                            {
                                Console.WriteLine("{0:yyyyMMdd HH:mm:ss.fffffff} - {1:yyyyMMdd HH:mm:ss.fffffff}",
                                                  e.ActualSignalTime, e.SignalTime);
                                if (previous != DateTime.MinValue)
                                {
                                    double diff = e.SignalTime.Subtract(previous).TotalSeconds;

                                    //NOTE: we'll allow a 1 hour + 2 sec diff since this will occur when the system "daylight-saves"
                                    if (diff != 2 && diff != 3602)
                                        invalidDiff = true;
                                }
                                previous = e.SignalTime;
                            };

                    scheduler.Start();

                    Thread.Sleep(10000);

                    scheduler.Stop();
                }
            }

            Assert.IsFalse(invalidDiff);
        }

        /// <summary>
        ///A test for Scheduler (IAlarmTimer)
        ///</summary>
        [Test]
        public void IntervalScheduleElapsedTest()
        {
            var trigger = new AutoResetEvent(false);
            using (var target = SchedulerMock.Create())
            {
                int count = 3;

                target.ScheduledAlarm += ((sender, e) =>
                                              {
                                                  count--;
                                                  if (count != 0) return;
                                                  target.Stop();
                                                  trigger.Set();
                                              });

                target.Add(new IntervalSchedule(DateTime.Now, TimeSpan.FromSeconds(1)));
                target.Start();

                bool result = trigger.WaitOne(4000, true);
                Assert.IsTrue(result, "Did not receive expected event");
                Assert.AreEqual(0, count);
            }
        }


        [Test]
        public void NoLessThanSecondsIntervalTest()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                new IntervalSchedule(DateTime.Now, TimeSpan.FromTicks(1));
            });
        }

        [Test]
        public void MinimumSecondIntervalTest()
        {
            new IntervalSchedule(DateTime.Now, TimeSpan.FromSeconds(1));
            new IntervalSchedule(DateTime.Now, TimeSpan.FromMilliseconds(1000));
        }


        [Test]
        public void NoDuplicateIntervalsTest()
        {
            using (var scheduler = SchedulerMock.CreateFromConfig("IntervalScheduleConfig"))
            {
                int count = 0;
                int count2 = 0;
                bool _previousSet = false;
                bool duplicateDetected = false;
                DateTime _previousAlarm = DateTime.MinValue;

                scheduler.ScheduledAlarm += ((sender, e) => count++);
                scheduler["Interval"].Alarm += ((sender, e) =>
                                                    {
                                                        Console.WriteLine(
                                                            "{0:yyyyMMdd HH:mm:ss.fffffff} - {1:yyyyMMdd HH:mm:ss.fffffff}",
                                                            e.ActualSignalTime, e.SignalTime);
                                                        count2++;
                                                        if (!_previousSet)
                                                        {
                                                            _previousAlarm = e.SignalTime;
                                                            _previousSet = true;
                                                        }
                                                        else
                                                        {
                                                            if (_previousAlarm == e.SignalTime)
                                                            {
                                                                duplicateDetected = true;
                                                                //Debugger.Break();
                                                            }
                                                            _previousAlarm = e.SignalTime;
                                                        }
                                                    });

                // Wait until we're aligned with the schedules start time
                DateTime start = DateTime.Now;
                int align = (5 - (start.Millisecond%5));
                if (align < 5)
                    Thread.Sleep(align);

                scheduler.Start();

                Thread.Sleep(32000);

                scheduler.Stop();

                Assert.IsFalse(duplicateDetected);
            }
        }

        /// <summary>
        ///Test what time the first interval will fire
        ///</summary>
        [Test]
        public void TestFirstOccurence()
        {
            //Set schedule to 2006-09-15 at 12:00:00 and every 20th minute for infinite times. (last time = 13:40:00)
            DateTime now = DateTime.Now;
            DateTime firstOccurence = now.AddMinutes(20);
            TimeSpan interval = TimeSpan.FromMinutes(20);
            var schedule = new IntervalSchedule(firstOccurence, interval);

            //Start testing
            TestMoveToFirstFrom(schedule, now, true, firstOccurence);
        }

        /// <summary>
        ///Test what time the first interval will fire
        ///</summary>
        [Test]
        public void TestFirstOccurenceMoveNext()
        {
            //Set schedule to 2006-09-15 at 12:00:00 and every 20th minute for infinite times. (last time = 13:40:00)
            DateTime now = DateTime.Now;
            DateTime firstOccurence = now.AddMinutes(20);
            TimeSpan interval = TimeSpan.FromMinutes(20);
            var schedule = new IntervalSchedule(firstOccurence, interval);

            //Start testing
            TestMoveToFirstFrom(schedule, now, true, firstOccurence);
        }

        /// <summary>
        ///A test for MoveToFirstFrom (DateTime)
        ///</summary>
        [Test]
        public void MoveToFirstFromTest()
        {
            //Initialize.
            //Set schedule to 2006-09-15 at 12:00:00 and every 20th minute for infinite times. (last time = 13:40:00)
            var firstOccurence = new DateTime(2006, 9, 18, 12, 00, 00);
            TimeSpan interval = TimeSpan.FromMinutes(20);
            var schedule = new IntervalSchedule(firstOccurence, interval);

            //Start testing
            TestMoveToFirstFrom(schedule, new DateTime(2006, 9, 18, 11, 00, 00), true,
                                new DateTime(2006, 9, 18, 12, 00, 00));
            TestMoveToFirstFrom(schedule, new DateTime(2006, 9, 18, 12, 00, 00).AddTicks(-1L), true,
                                new DateTime(2006, 9, 18, 12, 00, 00));
            TestMoveToFirstFrom(schedule, new DateTime(2006, 9, 18, 12, 00, 00), true,
                                new DateTime(2006, 9, 18, 12, 20, 00));
            TestMoveToFirstFrom(schedule, new DateTime(2006, 9, 18, 12, 00, 00).AddTicks(1L), true,
                                new DateTime(2006, 9, 18, 12, 20, 00));
            TestMoveToFirstFrom(schedule, new DateTime(2006, 9, 18, 12, 10, 00), true,
                                new DateTime(2006, 9, 18, 12, 20, 00));
            TestMoveToFirstFrom(schedule, new DateTime(2006, 9, 18, 12, 20, 00), true,
                                new DateTime(2006, 9, 18, 12, 40, 00));
            TestMoveToFirstFrom(schedule, new DateTime(2006, 9, 18, 12, 30, 00), true,
                                new DateTime(2006, 9, 18, 12, 40, 00));
            TestMoveToFirstFrom(schedule, new DateTime(2006, 9, 18, 13, 30, 00), true,
                                new DateTime(2006, 9, 18, 13, 40, 00));
            TestMoveToFirstFrom(schedule, new DateTime(2006, 9, 18, 13, 40, 00).AddTicks(-1L), true,
                                new DateTime(2006, 9, 18, 13, 40, 00));
        }


        private void TestMoveToFirstFrom(IntervalSchedule instance, DateTime targetTime, bool isExpectedSuccess,
                                         DateTime expectedTime)
        {
            Assert.AreEqual(isExpectedSuccess, instance.MoveToFirstFrom(targetTime));
            Assert.AreEqual(expectedTime, instance.CurrentOccurrence);
        }

        [Test]
        public void MoveToNextTest()
        {
            //Set schedule to 2003-07-1 at 19:15:00 and every 15th minute for X times.
            var firstOccurence = new DateTime(2003, 7, 1, 19, 15, 00);
            TimeSpan interval = TimeSpan.FromMinutes(15);
            var schedule = new IntervalSchedule(firstOccurence, interval, new object());

            Assert.IsTrue(schedule.MoveToFirstFrom(DateTime.MinValue));
            Assert.AreEqual(firstOccurence, schedule.CurrentOccurrence);

            DateTime expected = firstOccurence;
            for (int i = 10; i > 0; i--)
            {
                expected += interval;
                Assert.IsTrue(schedule.MoveToNext());
                Assert.AreEqual(expected, schedule.CurrentOccurrence);
            }
        }

        [Test]
        public void MoveToNextRepeatTest()
        {
            //Set schedule to 2003-07-1 at 19:15:00 and every 15th minute for X times.
            var firstOccurence = new DateTime(2008, 01, 21, 09, 30, 00, 20);
            TimeSpan interval = TimeSpan.FromSeconds(1);
            var schedule = new IntervalSchedule(firstOccurence, interval, new object());

            Assert.IsTrue(schedule.MoveToFirstFrom(DateTime.MinValue));
            Assert.AreEqual(firstOccurence, schedule.CurrentOccurrence);

            DateTime expected = firstOccurence;
            for (int i = 180; i > 0; i--)
            {
                expected += interval;
                Assert.IsTrue(schedule.MoveToNext());
                Assert.AreEqual(expected, schedule.CurrentOccurrence);
                Console.WriteLine("{0:yyyyMMdd HH:mm:ss.fffffff}", schedule.CurrentOccurrence);
            }
        }

        [Test]
        public void ConstructByParametersTest()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                //Should fail because the interval is 0
                var firstOccurence = new DateTime(2006, 9, 18, 12, 00, 00);
                var interval = new TimeSpan(0);
                var schedule = new IntervalSchedule(firstOccurence, interval);
            });
        }

        /// <summary>
        ///A test for IntervalSchedule(XmlNode) with valid configuration data.
        ///</summary>
        [Test]
        public void ConstructorByConfigurationTest()
        {
            var scheduler = TestXmlConstructor("TestData/IntervalScheduleTest/schedule[@name='ok']");
            Assert.AreEqual(DateTime.Parse("2006-01-01 12:00:00"), scheduler.FirstOccurrence);
            Assert.AreEqual(TimeSpan.Parse("1.00:30:00"), scheduler.Interval);
        }

        /// <summary>
        ///A test for IntervalSchedule(XmlNode)
        ///</summary>
        [Test]
        public void ConstructorTest4_1()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                TestXmlConstructor("TestData/IntervalScheduleTest/schedule[@name='fail_1']");
            });
        }

        /// <summary>
        ///A test for IntervalSchedule(XmlNode)
        ///</summary>
        [Test]
        public void ConstructorTest4_2()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                TestXmlConstructor("TestData/IntervalScheduleTest/schedule[@name='fail_2']");
            });
        }

        /// <summary>
        ///A test for IntervalSchedule(XmlNode)
        ///</summary>
        [Test]
        public void ConstructorTest4_3()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                TestXmlConstructor("TestData/IntervalScheduleTest/schedule[@name='fail_3']");
            });
        }

        /// <summary>
        ///A test for IntervalSchedule(XmlNode)
        ///</summary>
        [Test]
        public void ConstructorTest4_4()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                TestXmlConstructor("TestData/IntervalScheduleTest/schedule[@name='fail_4']");
            });
        }

        /// <summary>
        ///A test for IntervalSchedule(XmlNode)
        ///</summary>
        [Test]
        public void ConstructorTest4_6()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                TestXmlConstructor("TestData/IntervalScheduleTest/schedule[@name='fail_6']");
            });
        }

        /// <summary>
        ///A test for IntervalSchedule(XmlNode)
        ///</summary>
        [Test]
        public void ConstructorTest4_7()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                TestXmlConstructor("TestData/IntervalScheduleTest/schedule[@name='fail_7']");
            });
        }

        private static IntervalSchedule TestXmlConstructor(string nodePath)
        {
            var xd = new XmlDocument();
            xd.Load(Path.Combine("TestData", "ScheduleConfiguration.xml"));
            XmlNode node = xd.SelectSingleNode(nodePath);

            return new IntervalSchedule(node);
        }

        [Test]
        public void ToStringTest()
        {
            var firstOccurence = new DateTime(2006, 9, 18, 12, 00, 00);
            TimeSpan interval = TimeSpan.FromMinutes(20);
            var schedule = new IntervalSchedule(firstOccurence, interval);

            string retval = schedule.ToString();
            Assert.IsTrue(retval.StartsWith(schedule.GetType().ToString()));
            Assert.AreNotEqual(retval, schedule.GetType().ToString());
        }
    }
}