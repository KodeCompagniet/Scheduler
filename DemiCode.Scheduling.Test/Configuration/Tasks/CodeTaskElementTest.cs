using DemiCode.Scheduling.Tasks;
using DemiCode.Scheduling.Test.Mocks;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

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
            Assert.That(task, Is.InstanceOfType(typeof(CodeTask)));
        }

        [Test]
        public void Equals_WithNoParamGivesTrue()
        {
            var left = new CodeTaskElement();
            var right = new CodeTaskElement();
            var actual = left.Equals(right);
            Assert.That(actual, Is.True);
        }

        [Test]
        public void Equals_WithSameTypeGivesTrue()
        {
            var left = new CodeTaskElement(typeof(SimpleClass), "SimpleMethod");
            var right = new CodeTaskElement(typeof(SimpleClass), "SimpleMethod");
            var actual = left.Equals(right);
            Assert.That(actual, Is.True);
        }

        [Test]
        public void Equals_WithDifferentTypeGivesFalse()
        {
            var left = new CodeTaskElement(typeof(SimpleClass), "SimpleMethod");
            var right = new CodeTaskElement(typeof(SimpleClass), "ExtendedMethod");
            var actual = left.Equals(right);
            Assert.That(actual, Is.False);
        }

        [Test]
        public void Equals_WithDifferentScheduleRefsGivesFalse()
        {
            var left = new CodeTaskElement {new ScheduleReferenceElement("first")};
            var right = new CodeTaskElement { new ScheduleReferenceElement("last") };
            var actual = left.Equals(right);
            Assert.That(actual, Is.False);
        }

        [Test]
        public void Equals_WithEqualScheduleRefsGivesTrue()
        {
            var left = new CodeTaskElement {new ScheduleReferenceElement("first")};
            var right = new CodeTaskElement {new ScheduleReferenceElement("first")};
            var actual = left.Equals(right);
            Assert.That(actual, Is.True);
        }
    }


}
