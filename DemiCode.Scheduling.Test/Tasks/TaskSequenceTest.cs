using DemiCode.Logging;
using DemiCode.Scheduling.Test;
using DemiCode.Scheduling.Test.Mocks;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace DemiCode.Scheduling.Tasks.Test
{
    /// <summary>
    ///This is a test class for DemiCode.Scheduling.Tasks.TaskSequence and is intended
    ///to contain all DemiCode.Scheduling.Tasks.TaskSequence Unit Tests
    ///</summary>
    [TestFixture]
    public class TaskSequenceTest
    {
        [Test]
        public void ConstructorTest()
        {
            var tasks = new[] { new TaskMock(), new TaskMock() };
            var taskSequence = new TaskSequence(tasks);
        }

        [Test]
        public void ExecuteTest()
        {
            ILoggerFactory loggerFactory = new LoggerFactoryMock();

            var tasks = new[] { new TaskMock(), new TaskMock() };
            var task = new TaskSequence(tasks);

            task.Execute(loggerFactory);

            Assert.IsTrue(tasks[0].WasExecuted);
            Assert.IsTrue(tasks[1].WasExecuted);
        }

        [Test]
        public void Execute2Test()
        {
            ILoggerFactory loggerFactory = new LoggerFactoryMock();

            var tasks = new ITask[] { new BadTaskMock(), new TaskMock() };
            var task = new TaskSequence(tasks);

            task.Execute(loggerFactory);

            Assert.IsTrue(((BadTaskMock)tasks[0]).WasExecuted);
            Assert.IsFalse(((TaskMock)tasks[1]).WasExecuted);
        }


        private static SortedList<int, string> paramsAddedByTask;
        public static void MethodCalledByTask(string param1)
        {
            paramsAddedByTask.Add(paramsAddedByTask.Count, param1);
        }

        [Test]
        public void ExecuteTaskSequenceTest()
        {
            paramsAddedByTask = new SortedList<int, string>(3);

            using (var scheduler = SchedulerFactory.CreateFromConfig("TaskSequenceConfigSection"))
            {
                var alarmListener = new ScheduledAlarmListener(scheduler);
                scheduler.Start();

                Assert.IsTrue(alarmListener.WaitForScheduledAlarm(2000));
            }

            Assert.AreEqual(3, paramsAddedByTask.Count);
            Assert.AreEqual("1", paramsAddedByTask[0]);
            Assert.AreEqual("0", paramsAddedByTask[1]);
            Assert.AreEqual("2", paramsAddedByTask[2]);
        }


        /// <summary>
        ///Does TaskSequence dispose instances?
        ///</summary>

        [Test]
        public void TaskSequenceDisposesCodeAfterExecuteTest()
        {
            DisposableClass.ResetTrackerFields();

            ILoggerFactory loggerFactory = new LoggerFactoryMock();

            ITask[] tasks = new[] { new CodeTask(typeof(DisposableClass), "SimpleMethod") };
            var task = new TaskSequence(tasks);

            task.Execute(loggerFactory);

            Assert.IsTrue(DisposableClass.DisposeWasCalled);
        }


        [Test]
        public void NullParamTest()
        {
            ILoggerFactory loggerFactory = new LoggerFactoryMock();

            TaskSequence task;

            try
            {
                task = new TaskSequence(null);
                task.Execute(loggerFactory);
                Assert.Fail("Did not receive expected exception");
            }
            catch (ArgumentNullException) { }
            catch { Assert.Fail("Did not receive expected exception"); }

            try
            {
                task = new TaskSequence(new ITask[] { new TaskMock() });
                task.Execute(null);
                Assert.Fail("Did not receive expected exception");
            }
            catch (ArgumentException) { }
            catch { Assert.Fail("Did not receive expected exception"); }

            try
            {
                new TaskSequence(new ITask[0]);
                Assert.Fail("Did not receive expected exception");
            }
            catch (ArgumentException) { }
            catch { Assert.Fail("Did not receive expected exception"); }

        }

    }


}

