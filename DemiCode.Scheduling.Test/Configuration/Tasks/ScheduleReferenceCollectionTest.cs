using NUnit.Framework;

namespace DemiCode.Scheduling.Configuration.Tasks.Test
{
    /// <summary>
    ///This is a test class for DemiCode.Scheduling.Configuration.ScheduleReferenceCollection and is intended
    ///to contain all DemiCode.Scheduling.Configuration.ScheduleReferenceCollection Unit Tests
    ///</summary>
    [TestFixture]
    public class ScheduleReferenceCollectionTest
    {

        /// <summary>
        ///A test for ScheduleReferenceCollection ()
        ///</summary>
        [Test]
        public void ConstructorTest()
        {
            var target = new ScheduleReferenceElementCollection();
        }

        [Test]
        public void Equals_WithNoParamGivesTrue()
        {
            var left = new ScheduleReferenceElementCollection();
            var right = new ScheduleReferenceElementCollection();
            right.Should().Be(left);
        }

        [Test]
        public void Equals_WithSameContentGivesTrue()
        {
            var left = new ScheduleReferenceElementCollection {new ScheduleReferenceElement("first")};
            var right = new ScheduleReferenceElementCollection {new ScheduleReferenceElement("first")};
            right.Should().Be(left);
        }

        [Test]
        public void Equals_WithDifferentContentGivesFalse()
        {
            var left = new ScheduleReferenceElementCollection {new ScheduleReferenceElement("first")};
            var right = new ScheduleReferenceElementCollection {new ScheduleReferenceElement("last")};
            right.Should().NotBe(left);
        }

    }
}
