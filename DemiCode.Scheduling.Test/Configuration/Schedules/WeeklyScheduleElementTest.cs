using System;
using DemiCode.Scheduling.Schedules;
using NUnit.Framework;

namespace DemiCode.Scheduling.Configuration.Schedules.Test
{
    [TestFixture]
    public class WeeklyScheduleElementTest
    {

        [Test]
        public void Constructor1Test()
        {
            new WeeklyScheduleElement();
        }

        [Test]
        public void Constructor2Test()
        {
            var now = new DateTime(2008, 7, 6, 5, 4, 3, 2);
            var ts = TimeSpan.FromSeconds(5);
            var days = new[] { DayOfWeek.Tuesday, DayOfWeek.Friday };

            var target = new WeeklyScheduleElement("weekly", now, ts, days);

            Assert.AreEqual("weekly", target.Name);
            Assert.AreEqual(new DateTime(2008, 7, 6, 5, 4, 3, 2), target.FromDate.Value);
            Assert.AreEqual(DateTime.MinValue, target.ToDate.Value);
            Assert.AreEqual(TimeSpan.FromSeconds(5), target.TimeOfDay.Value);
            Assert.AreEqual(2, target.Days.Value.Length);
            Assert.AreEqual(DayOfWeek.Tuesday, target.Days.Value[0]);
            Assert.AreEqual(DayOfWeek.Friday, target.Days.Value[1]);
        }

        [Test]
        public void ScheduleTypeTest()
        {
            var now = DateTime.Now;
            var ts = TimeSpan.FromSeconds(5);
            var days = new[] { DayOfWeek.Tuesday, DayOfWeek.Friday };

            var target = new WeeklyScheduleElement("weekly", now, ts, days);

            Assert.IsInstanceOf<WeeklySchedule>(target.GetScheduleInstance());
        }

        [Test]
        public void Equals_WithNoParamGivesTrue()
        {
            var left = new WeeklyScheduleElement();
            var right = new WeeklyScheduleElement();
            Assert.AreEqual(left, right);
        }

        [Test]
        public void Equals_WithEqualParamsGivesTrue()
        {
            var left = new WeeklyScheduleElement("weekly", new DateTime(2008, 7, 6, 5, 4, 3, 2), TimeSpan.FromSeconds(5), new[] { DayOfWeek.Tuesday, DayOfWeek.Friday });
            var right = new WeeklyScheduleElement("weekly", new DateTime(2008, 7, 6, 5, 4, 3, 2), TimeSpan.FromSeconds(5), new[] { DayOfWeek.Tuesday, DayOfWeek.Friday });

            Assert.AreEqual(left, right);
        }

        [Test]
        public void Equals_WithEqualParamsGivesTrue2()
        {
            var left = new WeeklyScheduleElement("weekly", new DateTime(2008, 7, 6, 5, 4, 3, 2), new DateTime(2009, 7, 6, 5, 4, 3, 2), TimeSpan.FromSeconds(5), new[] { DayOfWeek.Tuesday, DayOfWeek.Friday });
            var right = new WeeklyScheduleElement("weekly", new DateTime(2008, 7, 6, 5, 4, 3, 2), new DateTime(2009, 7, 6, 5, 4, 3, 2), TimeSpan.FromSeconds(5), new[] { DayOfWeek.Tuesday, DayOfWeek.Friday });

            Assert.AreEqual(left, right);
        }

        [Test]
        public void Equals_WithDifferentParamsGivesFalse()
        {
            var left = new WeeklyScheduleElement("weekly", new DateTime(2008, 7, 6, 5, 4, 3, 2), new DateTime(2009, 7, 6, 5, 4, 3, 2), TimeSpan.FromSeconds(5), new[] { DayOfWeek.Tuesday, DayOfWeek.Friday });
            var right = new WeeklyScheduleElement("weeklie", new DateTime(2008, 7, 6, 5, 4, 3, 2), new DateTime(2009, 7, 6, 5, 4, 3, 2), TimeSpan.FromSeconds(5), new[] { DayOfWeek.Tuesday, DayOfWeek.Friday });
            Assert.AreNotEqual(left, right);

            left = new WeeklyScheduleElement("weekly", new DateTime(2008, 7, 6, 5, 4, 3, 2), new DateTime(2009, 7, 6, 5, 4, 3, 2), TimeSpan.FromSeconds(5), new[] { DayOfWeek.Tuesday, DayOfWeek.Friday });
            right = new WeeklyScheduleElement("weekly", new DateTime(2008, 8, 6, 5, 4, 3, 2), new DateTime(2009, 7, 6, 5, 4, 3, 2), TimeSpan.FromSeconds(5), new[] { DayOfWeek.Tuesday, DayOfWeek.Friday });
            Assert.AreNotEqual(left, right);

            left = new WeeklyScheduleElement("weekly", new DateTime(2008, 7, 6, 5, 4, 3, 2), new DateTime(2009, 7, 6, 5, 4, 3, 2), TimeSpan.FromSeconds(5), new[] { DayOfWeek.Tuesday, DayOfWeek.Friday });
            right = new WeeklyScheduleElement("weekly", new DateTime(2008, 7, 6, 5, 4, 3, 2), new DateTime(2010, 7, 6, 5, 4, 3, 2), TimeSpan.FromSeconds(5), new[] { DayOfWeek.Tuesday, DayOfWeek.Friday });
            Assert.AreNotEqual(left, right);

            left = new WeeklyScheduleElement("weekly", new DateTime(2008, 7, 6, 5, 4, 3, 2), new DateTime(2009, 7, 6, 5, 4, 3, 2), TimeSpan.FromSeconds(5), new[] { DayOfWeek.Tuesday, DayOfWeek.Friday });
            right = new WeeklyScheduleElement("weekly", new DateTime(2008, 7, 6, 5, 4, 3, 2), new DateTime(2009, 7, 6, 5, 4, 3, 2), TimeSpan.FromSeconds(10), new[] { DayOfWeek.Tuesday, DayOfWeek.Friday });
            Assert.AreNotEqual(left, right);

            left = new WeeklyScheduleElement("weekly", new DateTime(2008, 7, 6, 5, 4, 3, 2), new DateTime(2009, 7, 6, 5, 4, 3, 2), TimeSpan.FromSeconds(5), new[] { DayOfWeek.Tuesday, DayOfWeek.Friday });
            right = new WeeklyScheduleElement("weekly", new DateTime(2008, 7, 6, 5, 4, 3, 2), new DateTime(2009, 7, 6, 5, 4, 3, 2), TimeSpan.FromSeconds(5), new[] { DayOfWeek.Tuesday, DayOfWeek.Friday, DayOfWeek.Saturday });
            Assert.AreNotEqual(left, right);
        }

        [Test]
        public void LoadFromConfigTest()
        {
            SchedulerSettings settings = SchedulerSettingsManager.GetConfigurationSection("WeeklyScheduleConfig");

        }

    }
}
