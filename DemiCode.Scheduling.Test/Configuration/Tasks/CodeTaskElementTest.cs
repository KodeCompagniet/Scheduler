using DemiCode.Scheduling.Tasks;
using DemiCode.Scheduling.Test.Mocks;
using NUnit.Framework;

namespace DemiCode.Scheduling.Configuration.Tasks.Test
{
    [TestFixture]
    public class CodeTaskElementTest
    {

        [Test]
        public void ConstructorTest()
        {
            new CodeTaskElement();
        }

        [Test]
        public void ConstructorTest3()
        {
            var target = new CodeTaskElement(GetType(), "ConstructorTest3");
            var task = target.GetTaskInstance();
            task.Should().BeOfType<CodeTask>();
        }

        [Test]
        public void Equals_WithNoParamGivesTrue()
        {
            var left = new CodeTaskElement();
            var right = new CodeTaskElement();
            var actual = left.Equals(right);
            actual.Should().BeTrue();
        }

        [Test]
        public void Equals_WithSameTypeGivesTrue()
        {
            var left = new CodeTaskElement(typeof(SimpleClass), "SimpleMethod");
            var right = new CodeTaskElement(typeof(SimpleClass), "SimpleMethod");
            var actual = left.Equals(right);
            actual.Should().BeTrue();
        }

        [Test]
        public void Equals_WithDifferentTypeGivesFalse()
        {
            var left = new CodeTaskElement(typeof(SimpleClass), "SimpleMethod");
            var right = new CodeTaskElement(typeof(SimpleClass), "ExtendedMethod");
            var actual = left.Equals(right);
            actual.Should().BeFalse();
        }

        [Test]
        public void Equals_WithDifferentScheduleRefsGivesFalse()
        {
            var left = new CodeTaskElement {new ScheduleReferenceElement("first")};
            var right = new CodeTaskElement { new ScheduleReferenceElement("last") };
            var actual = left.Equals(right);
            actual.Should().BeFalse();
        }

        [Test]
        public void Equals_WithEqualScheduleRefsGivesTrue()
        {
            var left = new CodeTaskElement {new ScheduleReferenceElement("first")};
            var right = new CodeTaskElement {new ScheduleReferenceElement("first")};
            var actual = left.Equals(right);
            actual.Should().BeTrue();
        }
    }


}
