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

                target.AdditionalData.Should().Be(val);
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

                target.AlarmTime.Should().Be(val);
            }
        }

        /// <summary>
        ///A test for AlarmTime on an active alarm.
        ///</summary>
        [Test]
        public void AlarmTimeWhenEnabledTest()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                using (AlarmTimer target = new AlarmTimer())
                {
                    target.Enable(DateTime.Now.AddMilliseconds(100));
                    target.AlarmTime = DateTime.Now;
                }
            });
        }

        /// <summary>
        ///A test for AlarmTime on a disposed alarm.
        ///</summary>
        [Test]
        public void AlarmTimeWhenDisposedTest()
        {
            Assert.Throws<ObjectDisposedException>(() =>
            {
                using (AlarmTimer target = new AlarmTimer())
                {
                    target.Dispose();
                    target.AlarmTime = DateTime.Now;
                }
            });
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
                target.AdditionalData.Should().Be(additionalData);
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
                target.Enabled.Should().BeFalse();
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
                target.Enabled.Should().BeTrue();
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
                target.AlarmTime.Should().Be(alarmTime);

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

                target.Enabled.Should().Be(false);
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
                target.Enabled.Should().Be(false);
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

            actual.Should().Be(expected);
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
                    receivedSignal.Should().BeTrue();
                }
            }
        }

        [Test]
        public void Enable_TimeInThePastReturnsFalse()
        {
            using (var timer = new AlarmTimer())
            {
                var alarmTime = DateTime.Now.AddMilliseconds(-10);
                timer.Enable(alarmTime).Should().BeFalse();
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

                        (receivedSignal || i < minimumAllowedDiff).Should().BeTrue();

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
