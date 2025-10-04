using System.Configuration;
using DemiCode.Scheduling.Configuration.Tasks;
using DemiCode.Scheduling.Tasks;
using DemiCode.Scheduling.Test.Mocks;
using NUnit.Framework;

namespace DemiCode.Scheduling.Configuration.Test
{
    /// <summary>
    ///This is a test class for DemiCode.Scheduling.Configuration.TaskSequenceElement and is intended
    ///to contain all DemiCode.Scheduling.Configuration.TaskSequenceElement Unit Tests
    ///</summary>
    [TestFixture]
    public class TaskSequenceElementTest
    {

        /// <summary>
        ///A test for Instance
        ///</summary>
        [Test]
        public void InstanceTest()
        {
            var target = new TaskSequenceElement();
            target.Tasks.Add(new CodeTaskElement(typeof(SimpleClass), "SimpleMethod"));
            var val = target.GetTaskInstance();
            Assert.IsNotNull(val);
            Assert.IsInstanceOf<TaskSequence>(val);
        }

        /// <summary>
        ///A test for TaskSequenceElement ()
        ///</summary>
        [Test]
        public void ConstructorTest()
        {
            new TaskSequenceElement();
        }

        /// <summary>
        /// Test loading a scheduler section with TaskSequence.
        ///</summary>
        [Test]
        public void LoadSectionWithTaskSequenceTest()
        {
            var ss = (SchedulerSettings)ConfigurationManager.GetSection("TaskSequenceConfigSection");
            var taskSequenceElement = (TaskSequenceElement)ss.Schedulers[ss.DefaultScheduler].Tasks[0];
            Assert.AreEqual(3, taskSequenceElement.Tasks.Count);

            Assert.AreEqual(typeof(CodeTaskElement), taskSequenceElement.Tasks[0].GetType());
            Assert.IsTrue(taskSequenceElement.Tasks[0].Parameters.ContainsKey("param1"));
            Assert.AreEqual("1", taskSequenceElement.Tasks[0].Parameters["param1"]);

            Assert.IsTrue(taskSequenceElement.Tasks[1].Parameters.ContainsKey("param1"));
            Assert.AreEqual("0", taskSequenceElement.Tasks[1].Parameters["param1"]);

            Assert.IsTrue(taskSequenceElement.Tasks[2].Parameters.ContainsKey("param1"));
            Assert.AreEqual("2", taskSequenceElement.Tasks[2].Parameters["param1"]);

        }

        /// <summary>
        /// Test loading a scheduler section with TaskSequence and tasks with schedulerefs.
        ///</summary>
        [Test]
        public void LoadSectionWithTaskSequenceScheduleRefTest()
        {
            Assert.Throws<ConfigurationErrorsException>(() =>
            {
                SchedulerSettings ss = (SchedulerSettings)ConfigurationManager.GetSection("TaskSequenceScheduleRefConfigSection");
            });
        }

        /// <summary>
        /// Test loading a scheduler section with TaskSequence without tasks.
        ///</summary>
        [Test]
        public void LoadSectionWithEmptyTaskSequenceTest()
        {
            Assert.Throws<ConfigurationErrorsException>(() =>
            {
                SchedulerSettings ss = (SchedulerSettings)ConfigurationManager.GetSection("TaskSequenceEmptyConfigSection");
            });
        }

        [Test]
        public void Equals_WithNoParamGivesTrue()
        {
            var left = new TaskSequenceElement();
            var right = new TaskSequenceElement();
            var actual = left.Equals(right);
            Assert.That(actual, Is.True);
        }

        [Test]
        public void Equals_WithSameTypeGivesTrue()
        {
            var left = new TaskSequenceElement();
            left.Tasks.Add(new CodeTaskElement(typeof(SimpleClass), "SimpleMethod"));
            var right = new TaskSequenceElement();
            right.Tasks.Add(new CodeTaskElement(typeof(SimpleClass), "SimpleMethod"));
            var actual = left.Equals(right);
            Assert.That(actual, Is.True);
        }

        [Test]
        public void Equals_WithDifferentTypeGivesFalse()
        {
            var left = new TaskSequenceElement();
            left.Tasks.Add(new CodeTaskElement(typeof(SimpleClass), "SimpleMethod"));
            var right = new TaskSequenceElement();
            right.Tasks.Add(new ApplicationTaskElement());

            var actual = left.Equals(right);

            Assert.That(actual, Is.False);
        }

        [Test]
        public void Equals_WithDifferentScheduleRefsGivesFalse()
        {
            var left = new TaskSequenceElement { new ScheduleReferenceElement("first") };
            var right = new TaskSequenceElement { new ScheduleReferenceElement("last") };
            var actual = left.Equals(right);
            Assert.That(actual, Is.False);
        }

        [Test]
        public void Equals_WithEqualScheduleRefsGivesTrue()
        {
            var left = new TaskSequenceElement { new ScheduleReferenceElement("first") };
            var right = new TaskSequenceElement { new ScheduleReferenceElement("first") };
            var actual = left.Equals(right);
            Assert.That(actual, Is.True);
        }

    }


}
