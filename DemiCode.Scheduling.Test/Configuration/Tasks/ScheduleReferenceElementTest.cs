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
            Assert.AreEqual("schedule", target.ScheduleName);
        }

        [Test]
        public void Equals_WithNoParamGivesTrue()
        {
            var left = new ScheduleReferenceElement();
            var right = new ScheduleReferenceElement();
            Assert.AreEqual(left, right);
        }

        [Test]
        public void Equals_WithSameScheduleGivesTrue()
        {
            var left = new ScheduleReferenceElement("first");
            var right = new ScheduleReferenceElement("first");
            Assert.AreEqual(left, right);
        }

        [Test]
        public void Equals_WithDifferentTypeGivesFalse()
        {
            var left = new ScheduleReferenceElement("first");
            var right = new ScheduleReferenceElement("last");
            Assert.AreNotEqual(left, right);
        }
    }
}
