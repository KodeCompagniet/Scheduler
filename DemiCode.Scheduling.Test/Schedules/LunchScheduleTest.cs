using System;
using System.Xml;
using DemiCode.Scheduling.Test;
using NUnit.Framework;

namespace DemiCode.Scheduling.Schedules.Test
{

    public class LunchSchedule : ScheduleBase
    {
        private readonly TimeSpan _lunchTimeOfDay;
        public TimeSpan LunchTimeOfDay
        {
            get { return _lunchTimeOfDay; }
        }

        public LunchSchedule(string name, TimeSpan lunchTimeOfDay) : base(name)
        {
            _lunchTimeOfDay = lunchTimeOfDay;
        }

        public override bool MoveToFirstFrom(DateTime time)
        {
			if (time.TimeOfDay > LunchTimeOfDay)
			{
                //If we're over lunch time, move to next workday
				time = time.Date.AddDays(1);
			}

            //Set our lunch time as exact time of day
            CurrentOccurrence = time.Date.Add(LunchTimeOfDay);

            return true;
        }


        public LunchSchedule(XmlNode config) : base(config)
        {
            if (config == null)
                throw new ArgumentNullException("config");
 
            _lunchTimeOfDay = TimeSpan.Parse(config["lunchTimeOfDay"].InnerText);
        }
    }


	[TestFixture]
    public class LunchScheduleTest
	{
	  
	    [Test]
        public void MoveToFirstFromTest()
        {
            LunchSchedule schedule = new LunchSchedule("MyLunchSchedule", new TimeSpan(12,00,00));
            
            DateTime earlyMondayMorning = new DateTime(2007, 1, 1, 8, 0, 0);
            DateTime mondayLunch = new DateTime(2007, 1, 1, 12, 0, 0);
            DateTime tuesdayLunch = new DateTime(2007, 1, 2, 12, 0, 0);

            schedule.MoveToFirstFrom(earlyMondayMorning);
            schedule.CurrentOccurrence.Should().Be(mondayLunch);

            schedule.MoveToNext();
            schedule.CurrentOccurrence.Should().Be(tuesdayLunch);
        }

        [Test]
        public void ConfigTest()
        {
            TimeSpan lunchTime = new TimeSpan(16, 0, 0);

            using (Scheduler scheduler = Scheduler.CreateFromConfig("scheduling", "lunch"))
            {
                ISchedule schedule = scheduler["lunchschedule"];
                schedule.Should().BeOfType<LunchSchedule>();

                LunchSchedule lunch = (LunchSchedule)schedule;
                lunch.LunchTimeOfDay.Should().Be(lunchTime);

                scheduler.Start();

                DateTime now = (DateTime.Now.TimeOfDay > lunchTime ? DateTime.Now.AddDays(1) : DateTime.Now);
                lunch.CurrentOccurrence.Should().Be(now.Date.Add(lunchTime));
                
                scheduler.Stop();
            }
        }

	}
}
