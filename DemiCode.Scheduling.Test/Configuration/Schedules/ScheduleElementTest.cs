using DemiCode.Scheduling.Schedules;
using DemiCode.Scheduling.Test.Mocks;
using NUnit.Framework;

namespace DemiCode.Scheduling.Configuration.Schedules.Test
{
    [TestFixture]
    public class ScheduleElementTest
    {
        [Test]
        public void Constructor1Test()
        {
            new ScheduleElement();
        }

        [Test]
        public void Constructor2Test()
        {
            var target = new ScheduleElement("generic", typeof(OneShotSchedule));
            target.Name.Should().Be("generic");
        }

        [Test]
        public void ScheduleTypeTest()
        {
            var target = new ScheduleElement("weekly", typeof(ScheduleMock));
            target.GetScheduleInstance().Should().BeOfType<ScheduleMock>();
        }

        [Test]
        public void Equals_WithNoParamGivesTrue()
        {
            var left = new ScheduleElement();
            var right = new ScheduleElement();
            right.Should().Be(left);
        }

        [Test]
        public void Equals_WithEqualParamsGivesTrue()
        {
            var left = new ScheduleElement("weekly", typeof(OneShotSchedule));
            var right = new ScheduleElement("weekly", typeof(OneShotSchedule));
            right.Should().Be(left);
        }

        [Test]
        public void Equals_WithEqualParamsGivesTrue2()
        {
            var left = new ScheduleElement("weekly", typeof(OneShotSchedule));
            left.AddAdditionalProperty("time", "2008-07-06 05:43:02");
            var right = new ScheduleElement("weekly", typeof(OneShotSchedule));
            right.AddAdditionalProperty("time", "2008-07-06 05:43:02");
            right.Should().Be(left);
        }

        [Test]
        public void Equals_WithDifferentParamsGivesFalse()
        {
            var left = new ScheduleElement("weekly", typeof(OneShotSchedule));
            var right = new ScheduleElement("weekie", typeof(OneShotSchedule));
            right.Should().NotBe(left);

            left = new ScheduleElement("weekly", typeof(OneShotSchedule));
            right = new ScheduleElement("weekly", typeof(IntervalSchedule));
            right.Should().NotBe(left);
        }

    }
}
