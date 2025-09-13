using System.Configuration;
using DemiCode.Scheduling.Tasks;
using DemiCode.Scheduling.Test;
using DemiCode.Scheduling.Test.Mocks;
using NUnit.Framework;

namespace DemiCode.Scheduling.Configuration.Tasks.Test
{
    /// <summary>
    ///This is a test class for DemiCode.Scheduling.Configuration.TaskElement and is intended
    ///to contain all DemiCode.Scheduling.Configuration.TaskElement Unit Tests
    ///</summary>
    [TestFixture]
    public class TaskElementTest
    {

        /// <summary>
        ///A test for Instance
        ///</summary>
        [Test]
        public void InstanceTest()
        {
            var target = new TaskElement();
            target.TaskType = typeof(TaskMock);
            ITask val = target.GetTaskInstance();
            Assert.IsNotNull(val);
            Assert.IsInstanceOf<TaskMock>(val);
        }

        [Test]
        public void InstanceTest2()
        {
            var target = new TaskElement(typeof(TaskMock));
            ITask val = target.GetTaskInstance();
            Assert.IsNotNull(val);
            Assert.IsInstanceOf<TaskMock>(val);
        }

        [Test]
        public void InstanceTest3()
        {
            var target = new TaskElement(typeof(TaskMock), "param0Value");

            var val = target.GetTaskInstance();
            Assert.IsNotNull(val);
            Assert.IsInstanceOf<TaskMock>(val);
            var task = (TaskMock)val;
            Assert.AreEqual("param0Value", task.Param0);
        }

        /// <summary>
        ///A test for OnDeserializeUnrecognizedAttribute (string, string)
        ///</summary>
        
        // Test helper class to expose protected methods for testing
        private class TestableTaskElement : TaskElement
        {
            public bool TestOnDeserializeUnrecognizedAttribute(string name, string value)
            {
                return OnDeserializeUnrecognizedAttribute(name, value);
            }
            
            public object TestOnRequiredPropertyNotFound(string name)
            {
                return OnRequiredPropertyNotFound(name);
            }
        }
        
        [Test]
        public void OnDeserializeUnrecognizedAttributeTest()
        {
            var target = new TestableTaskElement();

            Assert.IsTrue(target.TestOnDeserializeUnrecognizedAttribute("attributeName", "attributeValue"));
            Assert.IsTrue(target.Parameters.ContainsKey("attributeName"));
            Assert.AreEqual("attributeValue", target.Parameters["attributeName"]);
        }

        /// <summary>
        ///A test for OnRequiredPropertyNotFound (string)
        ///</summary>
        
        [Test]
        public void OnRequiredPropertyNotFoundTest()
        {
            Assert.Throws<ConfigurationErrorsException>(() =>
            {
                var target = new TestableTaskElement();
                var actual = target.TestOnRequiredPropertyNotFound("someotherprop");
            });
        }

        /// <summary>
        ///A test for Parameters
        ///</summary>
        [Test]
        public void ParametersTest()
        {
            var target = new TestableTaskElement();

            Assert.IsNotNull(target.Parameters);

            target.TestOnDeserializeUnrecognizedAttribute("attrib", "value");

            // The default Task parameter and our new 'attrib/value' parameter
            Assert.AreEqual(2, target.Parameters.Count);
        }

        /// <summary>
        ///A test for TaskElement ()
        ///</summary>
        [Test]
        public void ConstructorTest()
        {
            var target = new TaskElement();
        }

        /// <summary>
        ///A test for TaskElement (string)
        ///</summary>
        [Test]
        public void ConstructorTest3()
        {
            var target = new CodeTaskElement(GetType(), "ConstructorTest3");
            var task = target.GetTaskInstance();
            Assert.AreEqual(typeof(CodeTask), task.GetType());
        }

        /// <summary>
        ///A test for ScheduleType
        ///</summary>
        [Test]
        public void TaskTypeTest()
        {
            var target = new TaskElement();

            var val = typeof(CodeTask);
            target.TaskType = val;

            Assert.AreEqual(val, target.TaskType, "DemiCode.Scheduling.Configuration.TaskElement.ScheduleType was not set correctly.");
        }

        [Test]
        public void Equals_WithNoParamGivesTrue()
        {
            var left = new TaskElement();
            var right = new TaskElement();
            var actual = left.Equals(right);
            Assert.That(actual, Is.True);
        }

        [Test]
        public void Equals_WithSameTypeGivesTrue()
        {
            var left = new TaskElement(typeof(CodeTask));
            var right = new TaskElement(typeof(CodeTask));
            var actual = left.Equals(right);
            Assert.That(actual, Is.True);
        }

        [Test]
        public void Equals_WithDifferentTypeGivesFalse()
        {
            var left = new TaskElement(typeof(CodeTask));
            var right = new TaskElement(typeof(ApplicationTask));
            var actual = left.Equals(right);
            Assert.That(actual, Is.False);
        }

        [Test]
        public void Equals_WithDifferentScheduleRefsGivesFalse()
        {
            var left = new TaskElement(typeof(CodeTask)) { new ScheduleReferenceElement("first") };
            var right = new TaskElement(typeof(CodeTask)) { new ScheduleReferenceElement("last") };
            var actual = left.Equals(right);
            Assert.That(actual, Is.False);
        }

        [Test]
        public void Equals_WithEqualScheduleRefsGivesTrue()
        {
            var left = new TaskElement { new ScheduleReferenceElement("first") };
            var right = new TaskElement { new ScheduleReferenceElement("first") };
            var actual = left.Equals(right);
            Assert.That(actual, Is.True);
        }

        [Test]
        public void Equals_WithDifferentAmountScheduleRefsGivesFalse()
        {
            var left = new TaskElement { new ScheduleReferenceElement("first") };
            var right = new TaskElement { new ScheduleReferenceElement("last"), new ScheduleReferenceElement("first") };
            var actual = left.Equals(right);
            Assert.That(actual, Is.False);
        }
    }


}
