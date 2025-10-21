using DemiCode.Logging.Configuration;
using DemiCode.Logging.log4net.Configuration;
using DemiCode.Scheduling.Configuration.Schedules;
using DemiCode.Scheduling.Configuration.Tasks;
using DemiCode.Scheduling.Schedules;
using DemiCode.Scheduling.Tasks;
using NUnit.Framework;

namespace DemiCode.Scheduling.Configuration.Test
{
    /// <summary>
    ///This is a test class for DemiCode.Scheduling.Configuration.SchedulerElement and is intended
    ///to contain all DemiCode.Scheduling.Configuration.SchedulerElement Unit Tests
    ///</summary>
    [TestFixture]
    public class SchedulerElementTest
    {

        [Test]
        public void ConstructorTest()
        {
            new SchedulerElement();
        }

        [Test]
        public void Equals_WithNoParamGivesTrue()
        {
            var left = new SchedulerElement();
            var right = new SchedulerElement();
            var actual = left.Equals(right);
            Assert.That(actual, Is.True);
        }

        [Test]
        public void Equals_WithSameNameGivesTrue()
        {
            var left = new SchedulerElement("first");
            var right = new SchedulerElement("first");
            var actual = left.Equals(right);
            Assert.That(actual, Is.True);
        }

        [Test]
        public void Equals_WithDifferentNamesGivesFalse()
        {
            var left = new SchedulerElement("first");
            var right = new SchedulerElement("last");
            var actual = left.Equals(right);
            Assert.That(actual, Is.False);
        }

        [Test]
        public void Equals_WithEqualSchedulesGivesTrue()
        {
            var left = new SchedulerElement { new ScheduleElement("first", typeof(OneShotSchedule)) };
            var right = new SchedulerElement { new ScheduleElement("first", typeof(OneShotSchedule)) };
            var actual = left.Equals(right);
            Assert.That(actual, Is.True);
        }

        [Test]
        public void Equals_WithDifferentSchedulesGivesFalse()
        {
            var left = new SchedulerElement { new ScheduleElement("first", typeof(OneShotSchedule)) };
            var right = new SchedulerElement { new ScheduleElement("last", typeof(OneShotSchedule)) };
            var actual = left.Equals(right);
            Assert.That(actual, Is.False);
        }

        [Test]
        public void Equals_WithEqualTasksGivesTrue()
        {
            var left = new SchedulerElement();
            left.Tasks.Add(new TaskElement(typeof(CodeTask)));
            var right = new SchedulerElement();
            right.Tasks.Add(new TaskElement(typeof(CodeTask)));

            var actual = left.Equals(right);
            Assert.That(actual, Is.True);
        }

        [Test]
        public void Equals_WithDifferentTasksGivesFalse()
        {
            var left = new SchedulerElement();
            left.Tasks.Add(new TaskElement(typeof(CodeTask)));
            var right = new SchedulerElement();
            right.Tasks.Add(new TaskElement(typeof(ApplicationTask)));

            var actual = left.Equals(right);
            Assert.That(actual, Is.False);
        }

        [Test]
        public void Equals_WithEqualLoggersGivesTrue()
        {
            var left = new SchedulerElement();
            left.Loggers.Add(new ConsoleLoggerElement());
            var right = new SchedulerElement();
            right.Loggers.Add(new ConsoleLoggerElement());

            var actual = left.Equals(right);
            Assert.That(actual, Is.True);
        }

        [Test]
        public void Equals_WithDifferentLoggersGivesFalse()
        {
            var left = new SchedulerElement();
            left.Loggers.Add(new ConsoleLoggerElement());
            var right = new SchedulerElement();
            right.Loggers.Add(new Log4netLoggerElement());
            right.Loggers.Add(new ConsoleLoggerElement());

            var actual = left.Equals(right);
            Assert.That(actual, Is.False);
        }
    }


}
