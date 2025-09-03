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
            var tasks = new[] {new TaskMock(), new TaskMock()};
            var taskSequence = new TaskSequence(tasks);
        }

        [Test]
        public void ExecuteTest()
        {
            ILoggerFactory loggerFactory = new LoggerFactoryMock();

            var tasks = new[] { new TaskMock(), new TaskMock() };
            var task = new TaskSequence(tasks);

            task.Execute(loggerFactory);

            tasks[0].WasExecuted.Should().BeTrue();
            tasks[1].WasExecuted.Should().BeTrue();
        }

        [Test]
        public void Execute2Test()
        {
            ILoggerFactory loggerFactory = new LoggerFactoryMock();

            var tasks = new ITask[] { new BadTaskMock(), new TaskMock() };
            var task = new TaskSequence(tasks);

            task.Execute(loggerFactory);

            ((BadTaskMock)tasks[0]).WasExecuted.Should().BeTrue();
            ((TaskMock)tasks[1]).WasExecuted.Should().BeFalse();
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

                alarmListener.WaitForScheduledAlarm(2000).Should().BeTrue();
            }

            paramsAddedByTask.Count.Should().Be(3);
            paramsAddedByTask[0].Should().Be("1");
            paramsAddedByTask[1].Should().Be("0");
            paramsAddedByTask[2].Should().Be("2");
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

            DisposableClass.DisposeWasCalled.Should().BeTrue();
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
            catch(ArgumentNullException) {}
            catch { Assert.Fail("Did not receive expected exception"); }

            try
            {
                task = new TaskSequence(new ITask[]{new TaskMock()});
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

