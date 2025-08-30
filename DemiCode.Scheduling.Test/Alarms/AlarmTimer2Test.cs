using NUnit.Framework;
using System;
using System.Threading;

namespace DemiCode.Scheduling.Alarms.Test
{
    [TestFixture]
    public class AlarmTimer2Test
    {

        /// <summary>
        ///A test for ~AlarmTimer2 ()
        ///</summary>
        [Test]
        public void FinalizerTest()
        {
            AlarmTimer a = new AlarmTimer();
            a.Dispose();
        }

        /// <summary>
        ///A test for AdditionalData
        ///</summary>
        [Test]
        public void AdditionalDataTest()
        {
            using (AlarmTimer target = new AlarmTimer())
            {
                object val = "test data";
                target.AdditionalData = val;

                Assert.AreEqual(val, target.AdditionalData, "DemiCode.Scheduling.AlarmTimer2.AdditionalData was not set correctly.");
            }
        }

        /// <summary>
        ///A test for AlarmTime
        ///</summary>
        [Test]
        public void AlarmTimeTest()
        {
            using (AlarmTimer target = new AlarmTimer())
            {
                DateTime val = DateTime.Now;
                target.AlarmTime = val;

                Assert.AreEqual(val, target.AlarmTime, "DemiCode.Scheduling.AlarmTimer2.AlarmTime was not set correctly.");
            }
        }

        /// <summary>
        ///A test for AlarmTime on an active alarm.
        ///</summary>
        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void AlarmTimeWhenEnabledTest()
        {
            using (AlarmTimer target = new AlarmTimer())
            {
                target.Enable(DateTime.Now.AddMilliseconds(100));
                target.AlarmTime = DateTime.Now;
            }
        }

        /// <summary>
        ///A test for AlarmTime on a disposed alarm.
        ///</summary>
        [Test]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void AlarmTimeWhenDisposedTest()
        {
            using (AlarmTimer target = new AlarmTimer())
            {
                target.Dispose();
                target.AlarmTime = DateTime.Now;
            }
        }

        /// <summary>
        ///A test for AlarmTimer2 ()
        ///</summary>
        [Test]
        public void ConstructorTest()
        {
            using (AlarmTimer target = new AlarmTimer())
            { }
        }

        /// <summary>
        ///A test for AlarmTimer2 (object)
        ///</summary>
        [Test]
        public void ConstructorTest1()
        {
            object additionalData = "test data";

            using (AlarmTimer target = new AlarmTimer(additionalData))
            {
                Assert.AreEqual(additionalData, target.AdditionalData);
            }
        }

        /// <summary>
        ///A test for Disable ()
        ///</summary>
        [Test]
        public void DisableTest()
        {
            using (AlarmTimer target = new AlarmTimer())
            {
                target.Disable();
                Assert.IsFalse(target.Enabled);
            }
        }

        /// <summary>
        ///A test for Dispose ()
        ///</summary>
        [Test]
        public void DisposeTest()
        {
            AlarmTimer target = new AlarmTimer();
            target.Dispose();
        }


        /// <summary>
        ///A test for Enable ()
        ///</summary>
        [Test]
        public void EnableTest()
        {
            using (AlarmTimer target = new AlarmTimer())
            {
                target.AlarmTime = DateTime.Now.AddMilliseconds(5);
                target.Enable();
                Assert.IsTrue(target.Enabled);
            }
        }

        /// <summary>
        ///A test for Enable (DateTime)
        ///</summary>
        [Test]
        public void EnableWithDateTimeTest()
        {
            using (AlarmTimer target = new AlarmTimer())
            {
                DateTime alarmTime = DateTime.Now.AddMilliseconds(5);

                target.Enable(alarmTime);
                Assert.AreEqual(alarmTime, target.AlarmTime);

            }
        }

        /// <summary>
        ///A test for Enabled=true without initializing AlarmTime. This yields immediate alarm "ring" 
        /// and thus Enabled = false as result.
        ///</summary>
        [Test]
        public void EnabledWithoutAlarmTimeTest()
        {
            using (AlarmTimer target = new AlarmTimer())
            {
                target.Enabled = true;

                Assert.AreEqual(false, target.Enabled);
            }
        }

        /// <summary>
        ///A test for Enabled=false.
        ///</summary>
        [Test]
        public void DisabledWithoutAlarmTimeTest()
        {
            using (AlarmTimer target = new AlarmTimer())
            {
                target.Enabled = false;
                Assert.AreEqual(false, target.Enabled);
            }
        }

        /// <summary>
        ///A test for TicksToMilliseconds (long)
        ///</summary>
        
        [Test]
        public void TicksToMillisecondsTest()
        {
            long ticks = 10000L;
            long expected = 1;
            long actual = AlarmTimer.TicksToMilliseconds(ticks);

            Assert.AreEqual(expected, actual, "DemiCode.Scheduling.AlarmTimer2.TicksToMilliseconds did not return the expected value.");
        }


        /// <summary>
        ///A test for ElapsedEvent
        ///</summary>
        [Test]
        public void ElapsedEventTest()
        {
            using (var trigger = new AutoResetEvent(false))
            {
                using (var timer = new AlarmTimer())
                {
                    timer.Elapsed += ((sender, e) => trigger.Set());

                    var alarmTime = DateTime.Now.AddMilliseconds(400);
                    timer.Enable(alarmTime);

                    var receivedSignal = trigger.WaitOne(1000, true);
                    Assert.IsTrue(receivedSignal, "Did not receive the expected event");
                }
            }
        }

        [Test]
        public void Enable_TimeInThePastReturnsFalse()
        {
            using (var timer = new AlarmTimer())
            {
                var alarmTime = DateTime.Now.AddMilliseconds(-10);
                Assert.IsFalse(timer.Enable(alarmTime));
            }
        }

        [Test]
        public void EventAccuracyTest()
        {
            const int roof = 200;
            const int timeout = roof + 10;
            const int minimumAllowedDiff = 200;

            using (var target = new AlarmTimer())
            {
                for (int i = 0; i < roof; i++)
                {
                    using (var trigger = new AutoResetEvent(false))
                    {
                        EventHandler<AlarmEventArgs> eventHandler = delegate { trigger.Set(); };
                        target.Elapsed += eventHandler;

                        var alarmTime = DateTime.Now.AddMilliseconds(i);
                        target.Enable(alarmTime);

                        var receivedSignal = trigger.WaitOne(timeout, true);

                        Assert.IsTrue((receivedSignal || i < minimumAllowedDiff), "Did not receive the expected event. Trigger milliseconds={0}", i);

                        target.Elapsed -= eventHandler;
                    }
                }
            }
        }

        [Test]
        public void LongIntervalTest()
        {
            var alarmTime = DateTime.Now.AddYears(1);
            using (var alarm = new AlarmTimer())
            {
                alarm.Enable(alarmTime);
            }
        }
    }
}
