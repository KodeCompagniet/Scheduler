using System.Collections.ObjectModel;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace DemiCode.Scheduling.Schedules.Test
{
	/// <summary>
	///This is a test class for DemiCode.Scheduling.WeeklySchedule and is intended
	///to contain all DemiCode.Scheduling.WeeklySchedule Unit Tests
	///</summary>
    [TestFixture]
    public class WeeklyScheduleTest
	{

        /// <summary>
        /// Provoke null argument exception when passing list of days.
        /// </summary>
        [Test]
        public void NullDaysArrayTest()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                DateTime fromDate = new DateTime(2007, 1, 1);
                DateTime toDate = new DateTime(2007, 1, 31);
                TimeSpan timeOfDay = new TimeSpan(00, 00, 00);
                List<DayOfWeek> days = null;
                WeeklySchedule schedule = new WeeklySchedule(fromDate, toDate, timeOfDay, days);
            });
        }


		/// <summary>
		///A test for MoveToFirstFrom (DateTime)
		///</summary>
		[Test]
		public void MoveToFirstFromTest1()
		{
			//Initialize.
			//Set schedule to every monday, wednesday, thursday and saturday at 20:00:00 with no start or end dates.
			TimeSpan timeOfDay = new TimeSpan(20, 00, 00);
			List<DayOfWeek> days = new List<DayOfWeek>();
			days.Add(DayOfWeek.Thursday);
			days.Add(DayOfWeek.Wednesday);
			days.Add(DayOfWeek.Saturday);
			days.Add(DayOfWeek.Monday);
			WeeklySchedule schedule = new WeeklySchedule(timeOfDay, days);

			//Start testing
			//A monday
			TestMoveToFirstFrom(schedule, new DateTime(2006, 9, 18, 11, 00, 00), true, new DateTime(2006, 9, 18, 20, 00, 00));
			TestMoveToFirstFrom(schedule, new DateTime(2006, 9, 18, 20, 00, 00).AddTicks(-1L), true, new DateTime(2006, 9, 18, 20, 00, 00));
			TestMoveToFirstFrom(schedule, new DateTime(2006, 9, 18, 20, 00, 00), true, new DateTime(2006, 9, 18, 20, 00, 00));
			TestMoveToFirstFrom(schedule, new DateTime(2006, 9, 18, 20, 00, 00).AddTicks(1L), true, new DateTime(2006, 9, 20, 20, 00, 00));
			TestMoveToFirstFrom(schedule, new DateTime(2006, 9, 18, 22, 00, 00), true, new DateTime(2006, 9, 20, 20, 00, 00));

			//A tuesday
			TestMoveToFirstFrom(schedule, new DateTime(2006, 9, 19, 11, 00, 00), true, new DateTime(2006, 9, 20, 20, 00, 00));
			TestMoveToFirstFrom(schedule, new DateTime(2006, 9, 19, 20, 00, 00).AddTicks(-1L), true, new DateTime(2006, 9, 20, 20, 00, 00));
			TestMoveToFirstFrom(schedule, new DateTime(2006, 9, 19, 20, 00, 00), true, new DateTime(2006, 9, 20, 20, 00, 00));
			TestMoveToFirstFrom(schedule, new DateTime(2006, 9, 19, 20, 00, 00).AddTicks(1L), true, new DateTime(2006, 9, 20, 20, 00, 00));
			TestMoveToFirstFrom(schedule, new DateTime(2006, 9, 19, 22, 00, 00), true, new DateTime(2006, 9, 20, 20, 00, 00));

			//A saturday
			TestMoveToFirstFrom(schedule, new DateTime(2010, 9, 25, 11, 00, 00), true, new DateTime(2010, 9, 25, 20, 00, 00));
			TestMoveToFirstFrom(schedule, new DateTime(2010, 9, 25, 20, 00, 00).AddTicks(-1L), true, new DateTime(2010, 9, 25, 20, 00, 00));
			TestMoveToFirstFrom(schedule, new DateTime(2010, 9, 25, 20, 00, 00), true, new DateTime(2010, 9, 25, 20, 00, 00));
			TestMoveToFirstFrom(schedule, new DateTime(2010, 9, 25, 20, 00, 00).AddTicks(1L), true, new DateTime(2010, 9, 27, 20, 00, 00));
			TestMoveToFirstFrom(schedule, new DateTime(2010, 9, 25, 22, 00, 00), true, new DateTime(2010, 9, 27, 20, 00, 00));
		}

        [Test]
        public void PropertiesTest()
        {
            DateTime fromDate = new DateTime(2007, 1, 1);
            DateTime toDate = new DateTime(2007, 1, 31);
            TimeSpan timeOfDay = new TimeSpan(22, 00, 10);
            List<DayOfWeek> days = new List<DayOfWeek>();
            days.Add(DayOfWeek.Tuesday);
            days.Add(DayOfWeek.Friday);

            WeeklySchedule schedule = new WeeklySchedule(fromDate, toDate, timeOfDay, days);
            Assert.AreEqual(fromDate, schedule.FromDate);
            Assert.AreEqual(toDate, schedule.ToDate);
            Assert.AreEqual(timeOfDay, schedule.TimeOfDay);
            ReadOnlyCollection<DayOfWeek> daysCollection = schedule.Days;
            Assert.AreEqual(days.Count, daysCollection.Count);
            foreach (DayOfWeek dayOfWeek in days)
            {
                Assert.IsTrue(daysCollection.Contains(dayOfWeek));
            }
        }

        [Test]
        public void PropertiesFromConfigTest()
        {
            DateTime fromDate = new DateTime(2006, 1, 1);
            DateTime toDate = new DateTime(2006, 12, 31);
            TimeSpan timeOfDay = new TimeSpan(22, 00, 5);
            List<DayOfWeek> days = new List<DayOfWeek>();
            days.Add(DayOfWeek.Saturday);
            days.Add(DayOfWeek.Thursday);

            WeeklySchedule schedule = TestXmlConstructor("TestData/WeeklyScheduleTest/WeeklySchedule[@name='PropertiesTest']");

            Assert.AreEqual(fromDate, schedule.FromDate);
            Assert.AreEqual(toDate, schedule.ToDate);
            Assert.AreEqual(timeOfDay, schedule.TimeOfDay);
            ReadOnlyCollection<DayOfWeek> daysCollection = schedule.Days;
            Assert.AreEqual(days.Count, daysCollection.Count);
            foreach (DayOfWeek dayOfWeek in days)
            {
                Assert.IsTrue(daysCollection.Contains(dayOfWeek));    
            }
        }

	    [Test]
		public void MoveToFirstFromTest2()
		{
			//Initialize.
			//Set schedule to every tuesday and friday at 00:00:00 between 2007-01-01 and 2007-01-31.
			DateTime fromDate = new DateTime(2007, 1, 1);
			DateTime toDate = new DateTime(2007, 1, 31);
			TimeSpan timeOfDay = new TimeSpan(00, 00, 00);
			List<DayOfWeek> days = new List<DayOfWeek>();
			days.Add(DayOfWeek.Tuesday);
			days.Add(DayOfWeek.Friday);
			WeeklySchedule schedule = new WeeklySchedule(fromDate, toDate, timeOfDay, days);

			//Start testing
			//Before fromDate
			TestMoveToFirstFrom(schedule, new DateTime(2006, 11, 18, 11, 00, 00), true, new DateTime(2007, 1, 2, 0, 0, 0));
			TestMoveToFirstFrom(schedule, new DateTime(2006, 11, 18, 00, 00, 00).AddTicks(-1L), true, new DateTime(2007, 1, 2, 0, 0, 0));
			TestMoveToFirstFrom(schedule, new DateTime(2006, 11, 18, 00, 00, 00), true, new DateTime(2007, 1, 2, 0, 0, 0));
			TestMoveToFirstFrom(schedule, new DateTime(2006, 11, 18, 00, 00, 00).AddTicks(1L), true, new DateTime(2007, 1, 2, 0, 0, 0));
			TestMoveToFirstFrom(schedule, new DateTime(2006, 11, 18, 22, 00, 00), true, new DateTime(2007, 1, 2, 0, 0, 0));

			//A saturday within the period
			TestMoveToFirstFrom(schedule, new DateTime(2007, 1, 13, 11, 00, 00), true, new DateTime(2007, 1, 16, 0, 0, 0));
			TestMoveToFirstFrom(schedule, new DateTime(2007, 1, 13, 00, 00, 00).AddTicks(-1L), true, new DateTime(2007, 1, 16, 0, 0, 0));
			TestMoveToFirstFrom(schedule, new DateTime(2007, 1, 13, 00, 00, 00), true, new DateTime(2007, 1, 16, 0, 0, 0));
			TestMoveToFirstFrom(schedule, new DateTime(2007, 1, 13, 00, 00, 00).AddTicks(1L), true, new DateTime(2007, 1, 16, 0, 0, 0));
			TestMoveToFirstFrom(schedule, new DateTime(2007, 1, 13, 22, 00, 00), true, new DateTime(2007, 1, 16, 0, 0, 0));

			//A friday within the period
			TestMoveToFirstFrom(schedule, new DateTime(2007, 1, 26, 11, 00, 00), true, new DateTime(2007, 1, 30, 0, 0, 0));
			TestMoveToFirstFrom(schedule, new DateTime(2007, 1, 26, 00, 00, 00).AddTicks(-1L), true, new DateTime(2007, 1, 26, 0, 0, 0));
			TestMoveToFirstFrom(schedule, new DateTime(2007, 1, 26, 00, 00, 00), true, new DateTime(2007, 1, 26, 0, 0, 0));
			TestMoveToFirstFrom(schedule, new DateTime(2007, 1, 26, 00, 00, 00).AddTicks(1L), true, new DateTime(2007, 1, 30, 0, 0, 0));
			TestMoveToFirstFrom(schedule, new DateTime(2007, 1, 26, 22, 00, 00), true, new DateTime(2007, 1, 30, 0, 0, 0));

			//After toDate
			TestMoveToFirstFrom(schedule, new DateTime(2007, 9, 25, 11, 00, 00), false, DateTime.MaxValue);
			TestMoveToFirstFrom(schedule, new DateTime(2007, 9, 25, 00, 00, 00).AddTicks(-1L), false, DateTime.MaxValue);
			TestMoveToFirstFrom(schedule, new DateTime(2007, 9, 25, 00, 00, 00), false, DateTime.MaxValue);
			TestMoveToFirstFrom(schedule, new DateTime(2007, 9, 25, 00, 00, 00).AddTicks(1L), false, DateTime.MaxValue);
			TestMoveToFirstFrom(schedule, new DateTime(2007, 9, 25, 22, 00, 00), false, DateTime.MaxValue);
		}

		[Test]
		public void MoveToNextTest()
		{
			DateTime fromDate = new DateTime(2000, 12, 24);
			DateTime toDate = new DateTime(2001, 1, 24);
			TimeSpan timeOfDay = new TimeSpan(7, 59, 59);
			List<DayOfWeek> days = new List<DayOfWeek>();
			days.Add(DayOfWeek.Sunday);
			WeeklySchedule schedule = new WeeklySchedule(fromDate, toDate, timeOfDay, days);

			Assert.IsTrue(schedule.MoveToFirstFrom(DateTime.MinValue));
			Assert.AreEqual(new DateTime(2000, 12, 24, 7, 59, 59), schedule.CurrentOccurrence);

			Assert.IsTrue(schedule.MoveToNext());
			Assert.AreEqual(new DateTime(2000, 12, 31, 7, 59, 59), schedule.CurrentOccurrence);

			Assert.IsTrue(schedule.MoveToNext());
			Assert.AreEqual(new DateTime(2001, 1, 7, 7, 59, 59), schedule.CurrentOccurrence);

			Assert.IsTrue(schedule.MoveToNext());
			Assert.AreEqual(new DateTime(2001, 1, 14, 7, 59, 59), schedule.CurrentOccurrence);

			Assert.IsTrue(schedule.MoveToNext());
			Assert.AreEqual(new DateTime(2001, 1, 21, 7, 59, 59), schedule.CurrentOccurrence);

			Assert.IsFalse(schedule.MoveToNext());
			Assert.AreEqual(DateTime.MaxValue, schedule.CurrentOccurrence);
		}

        [Test]
        public void EndDateIsOptionalTest()
        {
            List<DayOfWeek> days = new List<DayOfWeek>();
            days.Add(DayOfWeek.Thursday);
            WeeklySchedule schedule = new WeeklySchedule(new DateTime(2006, 1, 1), new TimeSpan(22, 0, 0), days);

            Assert.IsTrue(schedule.MoveToNext());
            Assert.AreEqual(new DateTime(2006, 1, 12, 22, 0, 0), schedule.CurrentOccurrence);
        }

		private static void TestMoveToFirstFrom(WeeklySchedule instance, DateTime targetTime, bool isExcpectedSuccess, DateTime expectedTime)
		{
			Assert.AreEqual(isExcpectedSuccess, instance.MoveToFirstFrom(targetTime));
			Assert.AreEqual(expectedTime, instance.CurrentOccurrence);
		}

		[Test]
		public void ConstructWithDuplicateDaysTest()
		{
			Assert.Throws<ArgumentException>(() =>
			{
				//Should fail because saturday is listed twice
				TimeSpan timeOfDay = new TimeSpan(20, 00, 00);
				List<DayOfWeek> days = new List<DayOfWeek>();
				days.Add(DayOfWeek.Thursday);
				days.Add(DayOfWeek.Wednesday);
				days.Add(DayOfWeek.Saturday);
				days.Add(DayOfWeek.Saturday);
				days.Add(DayOfWeek.Monday);
				WeeklySchedule schedule = new WeeklySchedule(timeOfDay, days);
			});
		}

		[Test]
		public void ConstructorTest2()
		{
			Assert.Throws<ArgumentException>(() =>
			{
				//Should fail because no days are selected
				TimeSpan timeOfDay = new TimeSpan(20, 00, 00);
				List<DayOfWeek> days = new List<DayOfWeek>();
				WeeklySchedule schedule = new WeeklySchedule(timeOfDay, days);
			});
		}

		[Test]
		public void ConstructorTest3()
		{
			Assert.Throws<ArgumentException>(() =>
			{
				//Should fail because toDate is before fromDate
				TimeSpan timeOfDay = new TimeSpan(20, 00, 00);
				List<DayOfWeek> days = new List<DayOfWeek>();
				days.Add(DayOfWeek.Monday);
				days.Add(DayOfWeek.Tuesday);
				days.Add(DayOfWeek.Wednesday);
				DateTime fromDate = new DateTime(2006, 9, 23);
				DateTime toDate = new DateTime(2006, 9, 21);
				WeeklySchedule schedule = new WeeklySchedule(fromDate, toDate, timeOfDay, days);
			});
		}

		[Test]
		public void ConstructorTest4()
		{
			//Try to use something else than List<DayOfWeek> for the days parameter
			TimeSpan timeOfDay = new TimeSpan(20, 00, 00);
			DayOfWeek[] days = new DayOfWeek[] { DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday };
			WeeklySchedule schedule = new WeeklySchedule(timeOfDay, days);
		}

		/// <summary>
		///A test for WeeklySchedule(XmlNode)
		///</summary>
		[Test]
		public void ConstructorTest5_1()
		{
			Assert.Throws<ArgumentException>(() =>
			{
				TestXmlConstructor("TestData/WeeklyScheduleTest/schedule[@name='fail_1']");
			});
		}

		/// <summary>
		///A test for WeeklySchedule(XmlNode)
		///</summary>
		[Test]
		public void ConstructorTest5_2()
		{
			Assert.Throws<ArgumentException>(() =>
			{
				TestXmlConstructor("TestData/WeeklyScheduleTest/schedule[@name='fail_2']");
			});
		}

		/// <summary>
		///A test for WeeklySchedule(XmlNode)
		///</summary>
		[Test]
		public void ConstructorTest5_3()
		{
			Assert.Throws<ArgumentException>(() =>
			{
				TestXmlConstructor("TestData/WeeklyScheduleTest/schedule[@name='fail_3']");
			});
		}

		/// <summary>
		///A test for WeeklySchedule(XmlNode)
		///</summary>
		[Test]
		public void ConstructorTest5_4()
		{
			Assert.Throws<ArgumentException>(() =>
			{
				TestXmlConstructor("TestData/WeeklyScheduleTest/schedule[@name='fail_4']");
			});
		}

		/// <summary>
		///A test for WeeklySchedule(XmlNode)
		///</summary>
		[Test]
		public void ConstructorTest5_5()
		{
			Assert.Throws<ArgumentNullException>(() =>
			{
				TestXmlConstructor("TestData/WeeklyScheduleTest/schedule[@name='fail_5']");
			});
		}

		/// <summary>
		///A test for WeeklySchedule(XmlNode)
		///</summary>
		[Test]
		public void ConstructorTest5_7()
		{
			Assert.Throws<ArgumentNullException>(() =>
			{
				TestXmlConstructor("TestData/WeeklyScheduleTest/schedule[@name='fail_7']");
			});
		}

		/// <summary>
		///A test for WeeklySchedule(XmlNode)
		///</summary>
		[Test]
		public void ConstructorTest5_8()
		{
			Assert.Throws<ArgumentException>(() =>
			{
				TestXmlConstructor("TestData/WeeklyScheduleTest/schedule[@name='fail_8']");
			});
		}

		/// <summary>
		///A test for WeeklySchedule(XmlNode)
		///</summary>
		[Test]
		public void ConstructorTest5_9()
		{
			Assert.Throws<ArgumentException>(() =>
			{
				TestXmlConstructor("TestData/WeeklyScheduleTest/schedule[@name='fail_9']");
			});
		}

		/// <summary>
		///A test for WeeklySchedule(XmlNode)
		///</summary>
		[Test]
		public void ConstructorTest5_10()
		{
			Assert.Throws<ArgumentException>(() =>
			{
				TestXmlConstructor("TestData/WeeklyScheduleTest/schedule[@name='fail_10']");
			});
		}

		/// <summary>
		///A test for WeeklySchedule(XmlNode)
		///</summary>
		[Test]
		public void ConstructorTest5_11()
		{
			Assert.Throws<ArgumentNullException>(() =>
			{
				TestXmlConstructor("TestData/WeeklyScheduleTest/schedule[@name='fail_11']");
			});
		}

        /// <summary>
        ///A test for WeeklySchedule(XmlNode)
        ///</summary>
        [Test]
        public void ConstructWithEmptyToDate()
        {
            WeeklySchedule schedule = TestXmlConstructor("TestData/WeeklyScheduleTest/WeeklySchedule[@name='WithoutEndDate']");
            Assert.AreEqual(DateTime.MaxValue, schedule.ToDate);
        }

        private static WeeklySchedule TestXmlConstructor(string nodePath)
		{
			XmlDocument xd = new XmlDocument();
			xd.Load(Path.Combine("TestData", "ScheduleConfiguration.xml"));
			XmlNode node = xd.SelectSingleNode(nodePath);

			return new WeeklySchedule(node);
		}

		[Test]
		public void ToStringTest()
		{
			TimeSpan timeOfDay = new TimeSpan(20, 00, 00);
			List<DayOfWeek> days = new List<DayOfWeek>();
			days.Add(DayOfWeek.Monday);
			days.Add(DayOfWeek.Tuesday);
			days.Add(DayOfWeek.Wednesday);
			WeeklySchedule schedule = new WeeklySchedule(timeOfDay, days);

			string retval = schedule.ToString();
			Assert.IsTrue(retval.StartsWith(schedule.GetType().ToString()));
			Assert.AreNotEqual(retval, schedule.GetType().ToString());
		}

	}
}
