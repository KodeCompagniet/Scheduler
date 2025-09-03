using NUnit.Framework;
using System;
using System.Xml;
using System.Threading;
using DemiCode.Scheduling.Alarms;

namespace DemiCode.Scheduling.Schedules.Test
{
	/// <summary>
	///This is a test class for DemiCode.Scheduling.ScheduleBase and is intended
	///to contain all DemiCode.Scheduling.ScheduleBase Unit Tests
	///</summary>
	[TestFixture]
	public class ScheduleBaseTest
	{

		/// <summary>
		///A test for FirstOccurrence, CurrentOccurrence and AdditionalData properties
		///</summary>
		[Test]
		public void PropertiesTest()
		{
			//Initialize
			object additionalData = new object();
			DateTime targetTime = new DateTime(2007, 5, 23, 08, 15, 00);
			OneShotSchedule schedule = new OneShotSchedule(targetTime, additionalData);

			//AdditionalData
			schedule.AdditionalData.Should().BeSameAs(additionalData);
			schedule.AdditionalData = "additional data test";
			schedule.AdditionalData.Should().Be("additional data test");

			//FirstOccurrence
			schedule.FirstOccurrence.Should().Be(targetTime);

			//CurrentOccurrence
			schedule.MoveToFirstFrom(targetTime.AddSeconds(-1));
			schedule.CurrentOccurrence.Should().Be(targetTime);
		}

		/// <summary>
		///A test for CompareTo methods of IComparable interface
		///</summary>
		[Test]
		public void CompareToTest1()
		{
			//Initialize
			OneShotSchedule schedule1 = new OneShotSchedule(new DateTime(2007, 1, 1, 12, 0, 0));
			OneShotSchedule schedule2 = new OneShotSchedule(new DateTime(2007, 1, 1, 12, 0, 1));
			OneShotSchedule schedule3 = new OneShotSchedule(new DateTime(2007, 1, 1, 12, 0, 1));

			schedule1.MoveToFirstFrom(DateTime.MinValue);
			schedule2.MoveToFirstFrom(DateTime.MinValue);
			schedule3.MoveToFirstFrom(DateTime.MinValue);

			(0 > schedule1.CompareTo(schedule2)).Should().BeTrue();
			(0 < schedule2.CompareTo(schedule1)).Should().BeTrue();
			schedule2.CompareTo(schedule3).Should().Be(0);
		}

		/// <summary>
		///A test for CompareTo methods of IComparable interface
		///</summary>
		///<remarks>
		///Comparing to null should yield an exception
		///</remarks>
		[Test]
		public void CompareToTest2()
		{
			Assert.Throws<ArgumentNullException>(() =>
			{
				OneShotSchedule schedule1 = new OneShotSchedule(new DateTime(1999, 7, 27, 8, 30, 0));
				schedule1.CompareTo(null);
			});
		}

		#region Test ScheduleBase's Alarm event
		private AutoResetEvent itsTimeEventTestWaitHandle = null;
		/// <summary>
		///A test for the Alarm event
		///</summary>
		[Test]
		public void ItsTimeEventTest()
		{
			using (itsTimeEventTestWaitHandle = new AutoResetEvent(false))
			{
				//Set up eventhandler
				OneShotSchedule schedule = new OneShotSchedule(new DateTime(2069, 10, 13, 6, 0, 0));
				schedule.Alarm += new EventHandler<AlarmEventArgs>(ItsTimeEventTestHandler);

				//Fire the event
				schedule.ItsTime(DateTime.Now);

				//Wait for event (max 0.5 sec)
				bool receivedSignal = itsTimeEventTestWaitHandle.WaitOne(500, false);
				receivedSignal.Should().BeTrue();
			}
		}

		/// <summary>
		/// Handler to receive the event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ItsTimeEventTestHandler(object sender, AlarmEventArgs e)
		{
			if (itsTimeEventTestWaitHandle != null)
				itsTimeEventTestWaitHandle.Set();
		}
		#endregion

		#region Test ScheduleBase's EndOfSchedule event
		private AutoResetEvent endOfScheduleEventTestWaitHandle = null;
		/// <summary>
		///A test for the EndOfSchedule event
		///</summary>
		[Test]
		public void EndOfScheduleEventTest()
		{
			using (endOfScheduleEventTestWaitHandle = new AutoResetEvent(false))
			{
				//Set up eventhandler
				OneShotSchedule schedule = new OneShotSchedule(new DateTime(2070, 11, 15, 7, 20, 0));
				schedule.EndOfSchedule += new EventHandler<EventArgs>(EndOfScheduleEventTestHandler);

				//Fire the event
				schedule.ItsOver();

				//Wait for event (max 0.5 sec)
				bool receivedSignal = endOfScheduleEventTestWaitHandle.WaitOne(500, false);
				receivedSignal.Should().BeTrue();
			}
		}

		/// <summary>
		/// Handler to receive the event
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void EndOfScheduleEventTestHandler(object sender, EventArgs e)
		{
			if (endOfScheduleEventTestWaitHandle != null)
				endOfScheduleEventTestWaitHandle.Set();
		}
		#endregion

		/// <summary>
		///A test for CreateFromXml (XmlNode)
		///</summary>
		[Test]
		public void CreateFromXmlTest()
		{
			Assert.Throws<NullReferenceException>(() =>
			{
				ScheduleBase.CreateFromXml(null);
			});
		}

        
        [Test]
        public void ConstructWithNullXmlNodeTest()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                XmlNode node = null;
                new MockSchedule(node);
            });
        }

        private class MockSchedule : ScheduleBase
        {
            public MockSchedule(XmlNode node) : base(node){}

            public override bool MoveToFirstFrom(DateTime time)
            {
                throw new NotImplementedException();
            }
        }
	}


}
