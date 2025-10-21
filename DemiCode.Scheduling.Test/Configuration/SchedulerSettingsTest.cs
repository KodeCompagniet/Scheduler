using DemiCode.Logging.Configuration;
using DemiCode.Logging;
using DemiCode.Logging.log4net;
using DemiCode.Scheduling.Configuration.Tasks;
using DemiCode.Scheduling.Schedules;
using DemiCode.Scheduling.Test.Mocks;
using DemiCode.Configuration;
using NUnit.Framework;
using System;
using System.Configuration;
using DemiCode.Scheduling.Configuration.Schedules;

namespace DemiCode.Scheduling.Configuration.Test
{
    [TestFixture]
    public class SchedulerSettingsTest
    {

        /// <summary>
        ///A test for DefaultScheduler
        ///</summary>
        [Test]
        public void DefaultSchedulerTest()
        {
            SchedulerSettings target = new SchedulerSettings();

            string val = "testvalue";

            target.DefaultScheduler = val;

            Assert.AreEqual(val, target.DefaultScheduler, "DemiCode.Scheduling.Configuration.SchedulerSettings.DefaultScheduler was not set correctly.");
        }

        /// <summary>
        /// Test that we're loading our schedules right.
        /// </summary>
        [Test]
        public void LoadSectionWithGenericSchedulesTest()
        {
            var ss = (SchedulerSettings)ConfigurationManager.GetSection("scheduling");
            var scheduler = ss.Schedulers[ss.DefaultScheduler];
            Assert.AreEqual(2, scheduler.Count);

            var element = scheduler.GetScheduleElementByName("WeeklySchedule") as ScheduleElement;
            Assert.IsNotNull(element);
            Assert.AreEqual(typeof(WeeklySchedule), element.ScheduleType);
            var weeklySchedule = element.GetScheduleInstance() as WeeklySchedule;
            Assert.IsNotNull(weeklySchedule);
            Assert.AreEqual("WeeklySchedule", weeklySchedule.Name);
            Assert.AreEqual(new DateTime(2006, 1, 1), weeklySchedule.FromDate);
            Assert.AreEqual(new DateTime(2006, 12, 31), weeklySchedule.ToDate);
            Assert.AreEqual(new TimeSpan(22, 0, 0), weeklySchedule.TimeOfDay);
            Assert.AreEqual(DayOfWeek.Monday, weeklySchedule.Days[0]);
            Assert.AreEqual(DayOfWeek.Tuesday, weeklySchedule.Days[1]);
            Assert.AreEqual(DayOfWeek.Wednesday, weeklySchedule.Days[2]);

            element = scheduler.GetScheduleElementByName("WeeklySchedule2") as ScheduleElement;
            Assert.IsNotNull(element);
            Assert.AreEqual(typeof(WeeklySchedule), element.ScheduleType);
            weeklySchedule = element.GetScheduleInstance() as WeeklySchedule;
            Assert.IsNotNull(weeklySchedule);
            Assert.AreEqual("WeeklySchedule2", weeklySchedule.Name);
            Assert.AreEqual(new DateTime(2006, 1, 1), weeklySchedule.FromDate);
            Assert.AreEqual(new DateTime(2006, 12, 31), weeklySchedule.ToDate);
            Assert.AreEqual(new TimeSpan(22, 0, 0), weeklySchedule.TimeOfDay);
            Assert.AreEqual(DayOfWeek.Friday, weeklySchedule.Days[0]);
            Assert.AreEqual(DayOfWeek.Tuesday, weeklySchedule.Days[1]);
        }

        /// <summary>
        /// Test that we're loading our schedules right.
        /// </summary>
        [Test]
        public void LoadSectionWithBuiltInSchedulesTest()
        {
            var ss = (SchedulerSettings)ConfigurationManager.GetSection("shortformatconfig");
            var scheduler = ss.Schedulers[ss.DefaultScheduler];
            Assert.AreEqual(4, scheduler.Count);

            var interval = scheduler.GetScheduleElementByName("interval") as IntervalScheduleElement;
            Assert.IsNotNull(interval);
            Assert.AreEqual(new DateTime(2006, 11, 30, 15, 50, 0), interval.FirstOccurrence.Value);

            var counted = scheduler.GetScheduleElementByName("counted") as IntervalCountedScheduleElement;
            Assert.IsNotNull(counted);
            Assert.AreEqual(31, counted.Occurrences.Value);

            var weekly = scheduler.GetScheduleElementByName("weekly") as WeeklyScheduleElement;
            Assert.IsNotNull(weekly);
            Assert.AreEqual(2, weekly.Days.Value.Length);
            Assert.AreEqual(DayOfWeek.Friday, weekly.Days.Value[0]);
            Assert.AreEqual(DayOfWeek.Tuesday, weekly.Days.Value[1]);

            var oneshot = scheduler.GetScheduleElementByName("oneshot") as OneShotScheduleElement;
            Assert.IsNotNull(oneshot);
            Assert.AreEqual(new DateTime(2007, 7, 1, 15, 20, 0), oneshot.Time.Value);
        }

        /// <summary>
        /// A test for PostDeserialize (). 
        /// We'll provoke some exceptions by loading an invalid section
        ///</summary>

        [Test]
        public void DeserializeInvalidSectionTest()
        {
            try
            {
                var ss = ConfigurationManager.GetSection("invalidschedulingsection") as SchedulerSettings;
                Assert.Fail("SchedulerSettings did not throw the expected exception");
            }
            catch (ConfigurationErrorsException e)
            {
                Assert.IsInstanceOf<InvalidConfigurationException>(e.InnerException);
            }
        }

        /// <summary>
        /// Test loading a scheduler section with task config.
        ///</summary>

        [Test]
        public void LoadSectionWithJobConfigTest()
        {
            var ss = (SchedulerSettings)ConfigurationManager.GetSection("TaskAndLoggingSection");
            var scheduler = ss.Schedulers[ss.DefaultScheduler];

            var tasks = scheduler.Tasks;
            Assert.IsNotNull(tasks);
            Assert.AreNotEqual(0, tasks.Count);

            // Validate tasks and properties are not null
            foreach (TaskElementBase task in tasks)
            {
                Assert.IsNotNull(task);
                Assert.IsNotNull(task.GetTaskInstance());
                Assert.IsNotNull(task.Parameters);
            }

            // Validate generic task with schedulerefs
            var genericTask = tasks[2];
            Assert.IsNotNull(genericTask);
            Assert.AreEqual(2, genericTask.Count);
            var scheduleref = genericTask[0];
            Assert.IsNotNull(scheduleref);
            Assert.IsNotNull("interval", scheduleref.ScheduleName);

            // Validate the app task
            var codeTask = tasks[3] as CodeTaskElement;
            Assert.IsNotNull(codeTask);
            Assert.AreEqual(typeof(SimpleClass), codeTask.Class);
            Assert.AreEqual("SimpleMethod", codeTask.Method);

            // Validate the app task
            var appTask = tasks[5] as ApplicationTaskElement;
            Assert.IsNotNull(appTask);
            Assert.AreEqual(@"c:\mytestapplication.exe", appTask.Path);
            Assert.AreEqual(@"c:\mytestapplication.exe", appTask.Parameters["path"]);
            Assert.AreEqual(@"test1", appTask.Parameters["someArgument"]);
            Assert.AreEqual(2, appTask.Parameters.Count);

            appTask = tasks[6] as ApplicationTaskElement;
            Assert.IsNotNull(appTask);
            Assert.AreEqual(@"c:\mytestapplication.exe", appTask.Path);
            Assert.AreEqual(@"c:\mytestapplication.exe", appTask.Parameters["path"]);
            Assert.AreEqual(@"test2", appTask.Parameters["someArgument"]);
            Assert.AreEqual(3, appTask.Parameters.Count);

        }

        /// <summary>
        /// Test loading a scheduler section with logger config.
        ///</summary>

        [Test]
        public void LoadSectionWithLoggerConfigTest()
        {
            var ss = (SchedulerSettings)ConfigurationManager.GetSection("TaskAndLoggingSection");
            var scheduler = ss.Schedulers["config_with_loggers_used_in_unittests"];
            Assert.IsNotNull(scheduler);

            var loggerElementCollection = scheduler.Loggers;
            Assert.IsNotNull(loggerElementCollection);
            Assert.AreNotEqual(0, loggerElementCollection.Count);

            // Validate loggers and properties are not null
            foreach (LoggerElementBase loggerElement in loggerElementCollection)
            {
                Assert.IsNotNull(loggerElement);
                Assert.IsNotNull(loggerElement.GetLoggerInstance());
                Assert.IsNotNull(loggerElement.Parameters);
            }

            // Validate generic logger
            var genericLogger = loggerElementCollection[0];
            Assert.IsNotNull(genericLogger);
            Assert.IsInstanceOf<LoggerFactoryMock>(genericLogger.GetLoggerInstance());
        }

        /// <summary>
        /// Test loading a scheduler section with invalid logger config.
        ///</summary>

        [Test]
        public void LoadSectionWithInvalidLoggerConfigTest()
        {
            Assert.Throws<ConfigurationErrorsException>(() =>
            {
                SchedulerSettings ss = (SchedulerSettings)ConfigurationManager.GetSection("LoggerInvalidConfigSection");
            });
        }

        /// <summary>
        /// Test loading a scheduler section with log4net logger config.
        ///</summary>

        [Test]
        public void LoadSectionWithLog4netLoggerConfigTest()
        {
            SchedulerSettings ss = (SchedulerSettings)ConfigurationManager.GetSection("TaskAndLoggingSection");
            SchedulerElement scheduler = ss.Schedulers["config_with_log4netlogger_used_in_unittests"];
            LoggerElementCollection loggerElementCollection = scheduler.Loggers;

            // Validate log4net logger
            LoggerElementBase log4netLogger = loggerElementCollection[0];
            Assert.IsNotNull(log4netLogger);

            ILoggerFactory factory = log4netLogger.GetLoggerInstance();
            Assert.IsNotNull(factory);
            Assert.IsInstanceOf<Log4netLoggerFactory>(factory);
        }

        /// <summary>
        /// Test loading a scheduler section with ConsoleLogger config.
        ///</summary>

        [Test]
        public void LoadSectionWithConsoleLoggerConfigTest()
        {
            SchedulerSettings ss = (SchedulerSettings)ConfigurationManager.GetSection("TaskAndLoggingSection");
            SchedulerElement scheduler = ss.Schedulers["config_with_consolelogger_used_in_unittests"];
            LoggerElementCollection loggerElementCollection = scheduler.Loggers;

            LoggerElementBase loggerElement = loggerElementCollection[0];
            Assert.IsNotNull(loggerElement);

            ILoggerFactory factory = loggerElement.GetLoggerInstance();
            Assert.IsNotNull(factory);
            Assert.IsInstanceOf<ConsoleLoggerFactory>(factory);
        }

        /// <summary>
        /// Test loading a scheduler section with logger with parameters config.
        ///</summary>

        [Test]
        public void LoadSectionWithParameterizedLoggerConfigTest()
        {
            var ss = (SchedulerSettings)ConfigurationManager.GetSection("TaskAndLoggingSection");
            var scheduler = ss.Schedulers["config_with_loggerandparameters_used_in_unittests"];
            var loggerElementCollection = scheduler.Loggers;

            var loggerElement = loggerElementCollection[0];
            Assert.IsNotNull(loggerElement);
            Assert.AreEqual(2, loggerElement.Parameters.Count);

            var factory = loggerElement.GetLoggerInstance();
            Assert.IsNotNull(factory);
            Assert.IsInstanceOf<LoggerFactoryMock>(factory);
        }

        /// <summary>
        /// Test loading a scheduler section with CodeTask config containing parameters.
        ///</summary>

        [Test]
        public void LoadSectionWithCodeTaskWithParametersConfigTest()
        {
            SchedulerSettings ss = (SchedulerSettings)ConfigurationManager.GetSection("CodeTaskWithParamsConfigSection");
            SchedulerElement schedulerElement = ss.Schedulers[ss.DefaultScheduler];
            Assert.AreEqual(1, schedulerElement.Tasks.Count);
            CodeTaskElement codeTaskElement = schedulerElement.Tasks[0] as CodeTaskElement;
            Assert.IsNotNull(codeTaskElement);
            Assert.AreEqual(typeof(SimpleClass), codeTaskElement.Class);
            Assert.AreEqual("OtherMethodWithParameters", codeTaskElement.Method);
            Assert.IsTrue(codeTaskElement.Parameters.ContainsKey("param1"));
            Assert.IsTrue(codeTaskElement.Parameters.ContainsKey("param2"));
        }

        [Test]
        public void LoadSectionWithTaskWithParametersConfigTest()
        {
            SchedulerSettings ss = (SchedulerSettings)ConfigurationManager.GetSection("TaskAndLoggingSection");
            SchedulerElement schedulerElement = ss.Schedulers[ss.DefaultScheduler];
            Assert.AreEqual(8, schedulerElement.Tasks.Count);
            TaskElement taskElement = schedulerElement.Tasks[1] as TaskElement;
            Assert.IsNotNull(taskElement);
            Assert.AreEqual(typeof(TaskMock), taskElement.TaskType);
            Assert.AreEqual("someValue", taskElement.AdditionalProperties["someParameter"]);
            TaskMock task = taskElement.GetTaskInstance() as TaskMock;
            Assert.IsNotNull(task);
            Assert.AreEqual("someValue", task.Param0);
        }

        /// <summary>
        /// Test loading a section with an invalid task element config.
        /// </summary>

        [Test]
        public void TaskElementMissingTypeTest()
        {
            Assert.Throws<ConfigurationErrorsException>(() =>
            {
                ConfigurationManager.GetSection("TaskInvalidConfigSection");
            });
        }

        /// <summary>
        /// Test loading a scheduler section with invalid CodeTask config.
        ///</summary>

        [Test]
        public void LoadSectionWithInvalidCodeTaskConfigTest()
        {
            Assert.Throws<ConfigurationErrorsException>(() =>
            {
                ConfigurationManager.GetSection("CodeTaskInvalidConfigSection");
            });
        }

        [Test]
        public void Equals_DifferentObjectsGivesFalse()
        {
            var left = new SchedulerSettings { DefaultScheduler = "left" };
            var right = new SchedulerSettings { DefaultScheduler = "right" };
            Assert.AreNotEqual(left, right);
        }

        [Test]
        public void Equals_EqualObjectsGivesTrue()
        {
            var left = new SchedulerSettings { DefaultScheduler = "same" };
            var right = new SchedulerSettings { DefaultScheduler = "same" };
            Assert.AreEqual(left, right);
        }

    }
}
