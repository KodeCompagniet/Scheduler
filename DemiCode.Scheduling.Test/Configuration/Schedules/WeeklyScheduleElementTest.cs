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
            var days = new[] {DayOfWeek.Tuesday, DayOfWeek.Friday};

            var target = new WeeklyScheduleElement("weekly", now, ts, days);

            target.Name.Should().Be("weekly");
            target.FromDate.Value.Should().Be(now);
            target.ToDate.Value.Should().Be(DateTime.MinValue);
            target.TimeOfDay.Value.Should().Be(TimeSpan.FromSeconds(5));
            target.Days.Value.Length.Should().Be(2);
            target.Days.Value[0].Should().Be(DayOfWeek.Tuesday);
            target.Days.Value[1].Should().Be(DayOfWeek.Friday);
        }

        [Test]
        public void ScheduleTypeTest()
        {
            var now = DateTime.Now;
            var ts = TimeSpan.FromSeconds(5);
            var days = new[] { DayOfWeek.Tuesday, DayOfWeek.Friday };

            var target = new WeeklyScheduleElement("weekly", now, ts, days);

            target.GetScheduleInstance().Should().BeOfType<WeeklySchedule>();
        }

        [Test]
        public void Equals_WithNoParamGivesTrue()
        {
            var left = new WeeklyScheduleElement();
            var right = new WeeklyScheduleElement();
            right.Should().Be(left);
        }

        [Test]
        public void Equals_WithEqualParamsGivesTrue()
        {
            var left = new WeeklyScheduleElement("weekly", new DateTime(2008, 7, 6, 5, 4, 3, 2), TimeSpan.FromSeconds(5), new[] { DayOfWeek.Tuesday, DayOfWeek.Friday });
            var right = new WeeklyScheduleElement("weekly", new DateTime(2008, 7, 6, 5, 4, 3, 2), TimeSpan.FromSeconds(5), new[] { DayOfWeek.Tuesday, DayOfWeek.Friday });

            right.Should().Be(left);
        }

        [Test]
        public void Equals_WithEqualParamsGivesTrue2()
        {
            var left = new WeeklyScheduleElement("weekly", new DateTime(2008, 7, 6, 5, 4, 3, 2), new DateTime(2009, 7, 6, 5, 4, 3, 2), TimeSpan.FromSeconds(5), new[] { DayOfWeek.Tuesday, DayOfWeek.Friday });
            var right = new WeeklyScheduleElement("weekly", new DateTime(2008, 7, 6, 5, 4, 3, 2), new DateTime(2009, 7, 6, 5, 4, 3, 2), TimeSpan.FromSeconds(5), new[] { DayOfWeek.Tuesday, DayOfWeek.Friday });

            right.Should().Be(left);
        }

        [Test]
        public void Equals_WithDifferentParamsGivesFalse()
        {
            var left = new WeeklyScheduleElement("weekly", new DateTime(2008, 7, 6, 5, 4, 3, 2), new DateTime(2009, 7, 6, 5, 4, 3, 2), TimeSpan.FromSeconds(5), new[] { DayOfWeek.Tuesday, DayOfWeek.Friday });
            var right = new WeeklyScheduleElement("weeklie", new DateTime(2008, 7, 6, 5, 4, 3, 2), new DateTime(2009, 7, 6, 5, 4, 3, 2), TimeSpan.FromSeconds(5), new[] { DayOfWeek.Tuesday, DayOfWeek.Friday });
            right.Should().NotBe(left);

            left = new WeeklyScheduleElement("weekly", new DateTime(2008, 7, 6, 5, 4, 3, 2), new DateTime(2009, 7, 6, 5, 4, 3, 2), TimeSpan.FromSeconds(5), new[] { DayOfWeek.Tuesday, DayOfWeek.Friday });
            right = new WeeklyScheduleElement("weekly", new DateTime(2008, 8, 6, 5, 4, 3, 2), new DateTime(2009, 7, 6, 5, 4, 3, 2), TimeSpan.FromSeconds(5), new[] { DayOfWeek.Tuesday, DayOfWeek.Friday });
            right.Should().NotBe(left);

            left = new WeeklyScheduleElement("weekly", new DateTime(2008, 7, 6, 5, 4, 3, 2), new DateTime(2009, 7, 6, 5, 4, 3, 2), TimeSpan.FromSeconds(5), new[] { DayOfWeek.Tuesday, DayOfWeek.Friday });
            right = new WeeklyScheduleElement("weekly", new DateTime(2008, 7, 6, 5, 4, 3, 2), new DateTime(2010, 7, 6, 5, 4, 3, 2), TimeSpan.FromSeconds(5), new[] { DayOfWeek.Tuesday, DayOfWeek.Friday });
            right.Should().NotBe(left);

            left = new WeeklyScheduleElement("weekly", new DateTime(2008, 7, 6, 5, 4, 3, 2), new DateTime(2009, 7, 6, 5, 4, 3, 2), TimeSpan.FromSeconds(5), new[] { DayOfWeek.Tuesday, DayOfWeek.Friday });
            right = new WeeklyScheduleElement("weekly", new DateTime(2008, 7, 6, 5, 4, 3, 2), new DateTime(2009, 7, 6, 5, 4, 3, 2), TimeSpan.FromSeconds(10), new[] { DayOfWeek.Tuesday, DayOfWeek.Friday });
            right.Should().NotBe(left);

            left = new WeeklyScheduleElement("weekly", new DateTime(2008, 7, 6, 5, 4, 3, 2), new DateTime(2009, 7, 6, 5, 4, 3, 2), TimeSpan.FromSeconds(5), new[] { DayOfWeek.Tuesday, DayOfWeek.Friday });
            right = new WeeklyScheduleElement("weekly", new DateTime(2008, 7, 6, 5, 4, 3, 2), new DateTime(2009, 7, 6, 5, 4, 3, 2), TimeSpan.FromSeconds(5), new[] { DayOfWeek.Tuesday, DayOfWeek.Friday, DayOfWeek.Saturday });
            right.Should().NotBe(left);
        }

        [Test]
        public void LoadFromConfigTest()
        {
            SchedulerSettings settings = SchedulerSettingsManager.GetConfigurationSection("WeeklyScheduleConfig");
            
        }

    }
}
