using NUnit.Framework;
using System;
using System.IO;
using System.Xml;

namespace DemiCode.Scheduling.Schedules.Test
{
    /// <summary>
    ///This is a test class for DemiCode.Scheduling.IntervalCountedSchedule and is intended
    ///to contain all DemiCode.Scheduling.IntervalCountedSchedule Unit Tests
    ///</summary>
    [TestFixture]
    public class IntervalCountedScheduleTest
    {

        /// <summary>
        ///A test for MoveToFirstFrom (DateTime)
        ///</summary>
        [Test]
        public void MoveToFirstFromTest1()
        {
            //Initialize.
            //Set schedule to 2006-09-15 at 12:00:00 and every 20th minute for 6 times. (last time = 13:40:00)
            DateTime firstOccurence = new DateTime(2006, 9, 18, 12, 00, 00);
            int numberOfOccurences = 6;
            TimeSpan interval = TimeSpan.FromMinutes(20);
            IntervalCountedSchedule schedule = new IntervalCountedSchedule(firstOccurence, numberOfOccurences, interval);

            //Start testing
            TestMoveToFirstFrom(schedule, new DateTime(2006, 9, 18, 11, 00, 00), true, new DateTime(2006, 9, 18, 12, 00, 00), 6L);
            TestMoveToFirstFrom(schedule, new DateTime(2006, 9, 18, 12, 00, 00).AddTicks(-1L), true, new DateTime(2006, 9, 18, 12, 00, 00), 6L);
            TestMoveToFirstFrom(schedule, new DateTime(2006, 9, 18, 12, 00, 00), true, new DateTime(2006, 9, 18, 12, 20, 00), 5L);
            TestMoveToFirstFrom(schedule, new DateTime(2006, 9, 18, 12, 00, 00).AddTicks(1L), true, new DateTime(2006, 9, 18, 12, 20, 00), 5L);
            TestMoveToFirstFrom(schedule, new DateTime(2006, 9, 18, 12, 10, 00), true, new DateTime(2006, 9, 18, 12, 20, 00), 5L);
            TestMoveToFirstFrom(schedule, new DateTime(2006, 9, 18, 12, 20, 00), true, new DateTime(2006, 9, 18, 12, 40, 00), 4L);
            TestMoveToFirstFrom(schedule, new DateTime(2006, 9, 18, 12, 30, 00), true, new DateTime(2006, 9, 18, 12, 40, 00), 4L);
            TestMoveToFirstFrom(schedule, new DateTime(2006, 9, 18, 13, 30, 00), true, new DateTime(2006, 9, 18, 13, 40, 00), 1L);
            TestMoveToFirstFrom(schedule, new DateTime(2006, 9, 18, 13, 40, 00).AddTicks(-1L), true, new DateTime(2006, 9, 18, 13, 40, 00), 1L);
            TestMoveToFirstFrom(schedule, new DateTime(2006, 9, 18, 13, 40, 00), false, DateTime.MaxValue, 0L);
            TestMoveToFirstFrom(schedule, new DateTime(2006, 9, 18, 13, 45, 00), false, DateTime.MaxValue, 0L);
        }

        /// <summary>
        ///A test for MoveToFirstFrom (DateTime)
        ///</summary>
        [Test]
        public void MoveToFirstFromTest2()
        {
            //Initialize.
            //Set schedule to 2006-09-15 at 12:00:00 and every 20th minute for ever.
            DateTime firstOccurence = new DateTime(2006, 9, 18, 12, 00, 00);
            int numberOfOccurences = -1;
            TimeSpan interval = TimeSpan.FromMinutes(20);
            IntervalCountedSchedule schedule = new IntervalCountedSchedule(firstOccurence, numberOfOccurences, interval, "additional data");

            //Start testing
            TestMoveToFirstFrom(schedule, new DateTime(1985, 9, 18, 11, 00, 00), true, new DateTime(2006, 9, 18, 12, 00, 00), -1L);
            TestMoveToFirstFrom(schedule, new DateTime(2006, 9, 18, 12, 00, 00).AddTicks(-1L), true, new DateTime(2006, 9, 18, 12, 00, 00), -1L);
            TestMoveToFirstFrom(schedule, new DateTime(2006, 9, 18, 12, 00, 00), true, new DateTime(2006, 9, 18, 12, 20, 00), -1L);
            TestMoveToFirstFrom(schedule, new DateTime(2006, 9, 18, 12, 00, 00).AddTicks(1L), true, new DateTime(2006, 9, 18, 12, 20, 00), -1L);
            TestMoveToFirstFrom(schedule, new DateTime(2006, 9, 18, 12, 10, 00), true, new DateTime(2006, 9, 18, 12, 20, 00), -1L);
            TestMoveToFirstFrom(schedule, new DateTime(2006, 9, 18, 12, 20, 00), true, new DateTime(2006, 9, 18, 12, 40, 00), -1L);
            TestMoveToFirstFrom(schedule, new DateTime(2006, 9, 18, 12, 30, 00), true, new DateTime(2006, 9, 18, 12, 40, 00), -1L);
            TestMoveToFirstFrom(schedule, new DateTime(9995, 9, 18, 13, 30, 00), true, new DateTime(9995, 9, 18, 13, 40, 00), -1L);
            TestMoveToFirstFrom(schedule, new DateTime(9995, 9, 18, 13, 40, 00).AddTicks(-1L), true, new DateTime(9995, 9, 18, 13, 40, 00), -1L);
            TestMoveToFirstFrom(schedule, new DateTime(9995, 9, 18, 13, 40, 00), true, new DateTime(9995, 9, 18, 14, 00, 00), -1L);
            TestMoveToFirstFrom(schedule, new DateTime(9995, 9, 18, 13, 45, 00), true, new DateTime(9995, 9, 18, 14, 00, 00), -1L);
        }

        private void TestMoveToFirstFrom(IntervalCountedSchedule instance, DateTime targetTime, bool isExpectedSuccess, DateTime expectedTime, long expectedRemainingOccurrences)
        {
            Assert.AreEqual(isExpectedSuccess, instance.MoveToFirstFrom(targetTime));
            Assert.AreEqual(expectedTime, instance.CurrentOccurrence);
            Assert.AreEqual(expectedRemainingOccurrences, instance.RemainingOccurences);
        }

        [Test]
        public void MoveToNextTest()
        {
            //Set schedule to 2003-07-1 at 19:15:00 and every 15th minute for 3 times.
            DateTime firstOccurence = new DateTime(2003, 7, 1, 19, 15, 00);
            int numberOfOccurences = 3;
            TimeSpan interval = TimeSpan.FromMinutes(15);
            IntervalCountedSchedule schedule = new IntervalCountedSchedule(firstOccurence, numberOfOccurences, interval, new object());

            Assert.IsTrue(schedule.MoveToFirstFrom(DateTime.MinValue));
            Assert.AreEqual(firstOccurence, schedule.CurrentOccurrence);

            Assert.IsTrue(schedule.MoveToNext());
            Assert.AreEqual(firstOccurence + interval, schedule.CurrentOccurrence);

            Assert.IsTrue(schedule.MoveToNext());
            Assert.AreEqual(firstOccurence + interval + interval, schedule.CurrentOccurrence);

            Assert.IsFalse(schedule.MoveToNext());
            Assert.AreEqual(DateTime.MaxValue, schedule.CurrentOccurrence);
        }

        [Test]
        public void ConstructorTest1()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                //Should fail because the interval is 0
                DateTime firstOccurence = new DateTime(2006, 9, 18, 12, 00, 00);
                int numberOfOccurences = 6;
                TimeSpan interval = new TimeSpan(0);
                IntervalCountedSchedule schedule = new IntervalCountedSchedule(firstOccurence, numberOfOccurences, interval);
            });
        }

        [Test]
        public void ConstructorTest2()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                //Should fail because numberOfOccurrences is set to 0 or lower than -1
                DateTime firstOccurence = new DateTime(2006, 9, 18, 12, 00, 00);
                int numberOfOccurences = 0;
                TimeSpan interval = TimeSpan.FromMinutes(20);
                IntervalCountedSchedule schedule = new IntervalCountedSchedule(firstOccurence, numberOfOccurences, interval);
            });
        }

        [Test]
        public void ConstructorTest3()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                //Should fail because numberOfOccurrences is set to 0 or lower than -1
                DateTime firstOccurence = new DateTime(2006, 9, 18, 12, 00, 00);
                int numberOfOccurences = -2;
                TimeSpan interval = TimeSpan.FromMinutes(20);
                IntervalCountedSchedule schedule = new IntervalCountedSchedule(firstOccurence, numberOfOccurences, interval);
            });
        }

        [Test]
        public void NoLessThanSecondsIntervalTest()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                new IntervalCountedSchedule(DateTime.Now, 10, TimeSpan.FromTicks(1));
            });
        }

        [Test]
        public void MinimumSecondIntervalTest()
        {
            new IntervalCountedSchedule(DateTime.Now, 10, TimeSpan.FromSeconds(1));
            new IntervalCountedSchedule(DateTime.Now, 10, TimeSpan.FromMilliseconds(1000));
        }

        /// <summary>
        ///A test for IntervalCountedSchedule(XmlNode)
        ///</summary>
        [Test]
        public void ConstructorTest4_1()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                TestXmlConstructor("TestData/IntervalCountedScheduleTest/schedule[@name='fail_1']");
            });
        }

        /// <summary>
        ///A test for IntervalCountedSchedule(XmlNode)
        ///</summary>
        [Test]
        public void ConstructorTest4_2()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                TestXmlConstructor("TestData/IntervalCountedScheduleTest/schedule[@name='fail_2']");
            });
        }

        /// <summary>
        ///A test for IntervalCountedSchedule(XmlNode)
        ///</summary>
        [Test]
        public void ConstructorTest4_3()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                TestXmlConstructor("TestData/IntervalCountedScheduleTest/schedule[@name='fail_3']");
            });
        }

        /// <summary>
        ///A test for IntervalCountedSchedule(XmlNode)
        ///</summary>
        [Test]
        public void ConstructorTest4_4()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                TestXmlConstructor("TestData/IntervalCountedScheduleTest/schedule[@name='fail_4']");
            });
        }

        /// <summary>
        ///A test for IntervalCountedSchedule(XmlNode)
        ///</summary>
        [Test]
        public void ConstructorTest4_5()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                TestXmlConstructor("TestData/IntervalCountedScheduleTest/schedule[@name='fail_5']");
            });
        }

        /// <summary>
        ///A test for IntervalCountedSchedule(XmlNode)
        ///</summary>
        [Test]
        public void ConstructorTest4_6()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                TestXmlConstructor("TestData/IntervalCountedScheduleTest/schedule[@name='fail_6']");
            });
        }

        /// <summary>
        ///A test for IntervalCountedSchedule(XmlNode)
        ///</summary>
        [Test]
        public void ConstructorTest4_7()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                TestXmlConstructor("TestData/IntervalCountedScheduleTest/schedule[@name='fail_7']");
            });
        }

        private static void TestXmlConstructor(string nodePath)
        {
            XmlDocument xd = new XmlDocument();
            xd.Load(Path.Combine("TestData", "ScheduleConfiguration.xml"));
            XmlNode node = xd.SelectSingleNode(nodePath);

            IntervalCountedSchedule target = new IntervalCountedSchedule(node);
        }

        [Test]
        public void ToStringTest()
        {
            DateTime firstOccurence = new DateTime(2006, 9, 18, 12, 00, 00);
            int numberOfOccurences = 6;
            TimeSpan interval = TimeSpan.FromMinutes(20);
            IntervalCountedSchedule schedule = new IntervalCountedSchedule(firstOccurence, numberOfOccurences, interval);

            string retval = schedule.ToString();
            Assert.IsTrue(retval.StartsWith(schedule.GetType().ToString()));
            Assert.AreNotEqual(retval, schedule.GetType().ToString());
        }
    }
}
