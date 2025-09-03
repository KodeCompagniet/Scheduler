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
            IDictionary parameters = new Dictionary<string, object>();
            ILoggerFactory loggerFactory = new LoggerFactoryMock();
            var target = new TaskHandler(task, parameters, loggerFactory);

            object sender = null;
            AlarmEventArgs e = null;

            target.ScheduledAlarmEventHandler(sender, e);

            TaskMock.WasExecutedByThread.Should().BeTrue();
        }

        /// <summary>
        ///A test for TaskHandler (ITask, NameValueCollection)
        ///</summary>
        [Test]
        public void ConstructorTest()
        {
            ITask task = new TaskMock();
            IDictionary parameters = new Dictionary<string, object>();
            ILoggerFactory factory = new LoggerFactoryMock();

            TaskHandler target = new TaskHandler(task, parameters, factory);
        
            target.Should().NotBeNull();
        }

        
        [Test]
        public void ConstructorWithNullParametersTest1()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                ITask task = new TaskMock();
                IDictionary parameters = new Dictionary<string, object>();
                ILoggerFactory loggerFactory = null;   

                TaskHandler target = new TaskHandler(task, parameters, loggerFactory);
            });
        }

        
        [Test]
        public void ConstructorWithNullParametersTest3()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                ITask task = null;
                IDictionary parameters = new Dictionary<string, object>();
                ILoggerFactory loggerFactory = new LoggerFactoryMock();

                TaskHandler target = new TaskHandler(task, parameters, loggerFactory);
            });
        }
    }
}