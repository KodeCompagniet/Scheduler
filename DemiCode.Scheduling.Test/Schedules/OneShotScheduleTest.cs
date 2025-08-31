using NUnit.Framework;
using System;
using System.Xml;

namespace DemiCode.Scheduling.Schedules.Test
{
	/// <summary>
	///This is a test class for DemiCode.Scheduling.OneShotSchedule and is intended
	///to contain all DemiCode.Scheduling.OneShotSchedule Unit Tests
	///</summary>
	[TestFixture]
	public class OneShotScheduleTest
	{

		/// <summary>
		///A test for MoveToFirstFrom (DateTime)
		///</summary>
		[Test]
		public void MoveToFirstFromTest()
		{
			//Initialize.
			//Testing a OneShotSchedule set to december 31. 2008, 23:59:59
			DateTime targetTime = new DateTime(2008, 12, 31, 23, 59, 59);
			OneShotSchedule schedule = new OneShotSchedule(targetTime, "additonal data");

			//Start testing
			//before
			TestMoveToFirstFrom(schedule, new DateTime(2007, 1, 1, 0, 0, 0), true, targetTime);
			TestMoveToFirstFrom(schedule, targetTime.AddTicks(-1), true, targetTime);

			//on time
			TestMoveToFirstFrom(schedule, targetTime, true, targetTime);

			//after
			TestMoveToFirstFrom(schedule, targetTime.AddTicks(1), false, DateTime.MaxValue);
			TestMoveToFirstFrom(schedule, new DateTime(2012, 1, 1, 0, 0, 0), false, DateTime.MaxValue);
		}

		private void TestMoveToFirstFrom(OneShotSchedule instance, DateTime targetTime, bool isExpectedSuccess, DateTime expectedTime)
		{
			Assert.AreEqual(isExpectedSuccess, instance.MoveToFirstFrom(targetTime));
			Assert.AreEqual(expectedTime, instance.CurrentOccurrence);
		}


		/// <summary>
		///A test for OneShotSchedule(XmlNode)
		///</summary>
		[Test]
		public void ConstructorTest3_1()
		{
			Assert.Throws<ArgumentException>(() =>
			{
				TestXmlConstructor("TestData/OneShotScheduleTest/schedule[@name='fail_1']");
			});
		}

		/// <summary>
		///A test for OneShotSchedule(XmlNode)
		///</summary>
		[Test]
		public void ConstructorTest3_2()
		{
			Assert.Throws<ArgumentException>(() =>
			{
				TestXmlConstructor("TestData/OneShotScheduleTest/schedule[@name='fail_2']");
			});
		}

		/// <summary>
		///A test for OneShotSchedule(XmlNode)
		///</summary>
		[Test]
		public void ConstructorTest3_3()
		{
			Assert.Throws<ArgumentException>(() =>
			{
				TestXmlConstructor("TestData/OneShotScheduleTest/schedule[@name='fail_3']");
			});
		}

		/// <summary>
		///A test for OneShotSchedule(XmlNode)
		///</summary>
		[Test]
		public void ConstructorTest3_4()
		{
			Assert.Throws<ArgumentNullException>(() =>
			{
				TestXmlConstructor("TestData/OneShotScheduleTest/schedule[@name='fail_4']");
			});
		}

		private static void TestXmlConstructor(string nodePath)
		{
			XmlDocument xd = new XmlDocument();
			xd.Load(@"TestData\ScheduleConfiguration.xml");
			XmlNode node = xd.SelectSingleNode(nodePath);

			OneShotSchedule target = new OneShotSchedule(node);
		}

		[Test]
		public void ToStringTest()
		{
			OneShotSchedule schedule = new OneShotSchedule(new DateTime(2007, 5, 23, 08, 15, 00));

			string retval = schedule.ToString();
			Assert.IsTrue(retval.StartsWith(schedule.GetType().ToString()));
			Assert.AreNotEqual(retval, schedule.GetType().ToString());
		}
	}
}
