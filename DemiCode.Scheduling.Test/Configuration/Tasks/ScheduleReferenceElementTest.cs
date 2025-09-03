using NUnit.Framework;

namespace DemiCode.Scheduling.Configuration.Tasks.Test
{
    /// <summary>
    ///This is a test class for DemiCode.Scheduling.Configuration.ScheduleReferenceElement and is intended
    ///to contain all DemiCode.Scheduling.Configuration.ScheduleReferenceElement Unit Tests
    ///</summary>
    [TestFixture]
    public class ScheduleReferenceElementTest
    {

        /// <summary>
        ///A test for ScheduleReferenceElement ()
        ///</summary>
        [Test]
        public void ConstructorTest()
        {
            var target = new ScheduleReferenceElement();
        }

        /// <summary>
        ///A test for ScheduleReferenceElement ()
        ///</summary>
        [Test]
        public void Constructor2Test()
        {
            var target = new ScheduleReferenceElement("schedule");
            target.ScheduleName.Should().Be("schedule");
        }

        [Test]
        public void Equals_WithNoParamGivesTrue()
        {
            var left = new ScheduleReferenceElement();
            var right = new ScheduleReferenceElement();
            right.Should().Be(left);
        }

        [Test]
        public void Equals_WithSameScheduleGivesTrue()
        {
            var left = new ScheduleReferenceElement("first");
            var right = new ScheduleReferenceElement("first");
            right.Should().Be(left);
        }

        [Test]
        public void Equals_WithDifferentTypeGivesFalse()
        {
            var left = new ScheduleReferenceElement("first");
            var right = new ScheduleReferenceElement("last");
            right.Should().NotBe(left);
        }
    }
}
