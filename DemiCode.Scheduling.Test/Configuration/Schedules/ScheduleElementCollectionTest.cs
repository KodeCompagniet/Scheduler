using System;
using System.Configuration;
using NUnit.Framework;

namespace DemiCode.Scheduling.Configuration.Schedules.Test
{
    [TestFixture]
    public class ScheduleElementCollectionTest
    {

        [Test]
        public void Constructor1Test()
        {
            new ScheduleElementCollection();
        }

        [Test]
        public void Equals_WithNoParamGivesTrue()
        {
            var left = new ScheduleElementCollection();
            var right = new ScheduleElementCollection();
            right.Should().Be(left);
        }

        [Test]
        public void Add_DuplicateThrowsException()
        {
            Assert.Throws<ConfigurationErrorsException>(() =>
            {
                var defaultScheduler = new ScheduleElementCollection();
                defaultScheduler.Add(new IntervalScheduleElement("MyInterval1", new DateTime(2008, 6, 1), new TimeSpan(0, 5, 0)));
                defaultScheduler.Add(new IntervalScheduleElement("MyInterval1", new DateTime(2008, 7, 1), new TimeSpan(0, 10, 0)));
            });
        }
    }
}
