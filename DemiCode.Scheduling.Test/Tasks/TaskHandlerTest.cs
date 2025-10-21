using System;
using System.Collections;
using System.Collections.Generic;
using DemiCode.Logging;
using DemiCode.Scheduling.Alarms;
using DemiCode.Scheduling.Test.Mocks;
using NUnit.Framework;

namespace DemiCode.Scheduling.Tasks.Test
{
    /// <summary>
    ///This is a test class for DemiCode.Scheduling.Scheduler.TaskHandler.
    ///</summary>
    [TestFixture]
    public class TaskHandlerTest
    {

        /// <summary>
        ///A test for ScheduledAlarmEventHandler (object, AlarmEventArgs)
        ///</summary>
        [Test]
        public void ScheduledAlarmEventHandlerTest()
        {
            var task = new TaskMock();
            ILoggerFactory loggerFactory = new LoggerFactoryMock();
            var target = new TaskHandler(task, loggerFactory);

            object sender = null;
            AlarmEventArgs e = null;

            target.ScheduledAlarmEventHandler(sender, e);

            Assert.IsTrue(TaskMock.WasExecutedByThread);
        }

        /// <summary>
        ///A test for TaskHandler (ITask, NameValueCollection)
        ///</summary>
        [Test]
        public void ConstructorTest()
        {
            ITask task = new TaskMock();
            ILoggerFactory factory = new LoggerFactoryMock();

            TaskHandler target = new TaskHandler(task, factory);

            Assert.IsNotNull(target);
        }


        [Test]
        public void ConstructorWithNullParametersTest1()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                ITask task = new TaskMock();
                ILoggerFactory loggerFactory = null;

                TaskHandler target = new TaskHandler(task, loggerFactory);
            });
        }


        [Test]
        public void ConstructorWithNullParametersTest3()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                ITask task = null;
                ILoggerFactory loggerFactory = new LoggerFactoryMock();

                TaskHandler target = new TaskHandler(task, loggerFactory);
            });
        }
    }
}