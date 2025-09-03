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

            target.DefaultScheduler.Should().Be(val);
        }

        /// <summary>
        /// Test that we're loading our schedules right.
        /// </summary>
        [Test]
        public void LoadSectionWithGenericSchedulesTest()
        {
            var ss = (SchedulerSettings)ConfigurationManager.GetSection("scheduling");
            var scheduler = ss.Schedulers[ss.DefaultScheduler];
            scheduler.Count.Should().Be(2);

            var element = scheduler.GetScheduleElementByName("WeeklySchedule") as ScheduleElement;
            element.Should().NotBeNull();
            element.ScheduleType.Should().Be(typeof(WeeklySchedule));
            var weeklySchedule = element.GetScheduleInstance() as WeeklySchedule;
            weeklySchedule.Should().NotBeNull();
            weeklySchedule.Name.Should().Be("WeeklySchedule");
            weeklySchedule.FromDate.Should().Be(new DateTime(2006, 1, 1));
            weeklySchedule.ToDate.Should().Be(new DateTime(2006, 12, 31));
            weeklySchedule.TimeOfDay.Should().Be(new TimeSpan(22, 0, 0));
            weeklySchedule.Days[0].Should().Be(DayOfWeek.Monday);
            weeklySchedule.Days[1].Should().Be(DayOfWeek.Tuesday);
            weeklySchedule.Days[2].Should().Be(DayOfWeek.Wednesday);

            element = scheduler.GetScheduleElementByName("WeeklySchedule2") as ScheduleElement;
            element.Should().NotBeNull();
            element.ScheduleType.Should().Be(typeof(WeeklySchedule));
            weeklySchedule = element.GetScheduleInstance() as WeeklySchedule;
            weeklySchedule.Should().NotBeNull();
            weeklySchedule.Name.Should().Be("WeeklySchedule2");
            weeklySchedule.FromDate.Should().Be(new DateTime(2006, 1, 1));
            weeklySchedule.ToDate.Should().Be(new DateTime(2006, 12, 31));
            weeklySchedule.TimeOfDay.Should().Be(new TimeSpan(22, 0, 0));
            weeklySchedule.Days[0].Should().Be(DayOfWeek.Friday);
            weeklySchedule.Days[1].Should().Be(DayOfWeek.Tuesday);
        }

        /// <summary>
        /// Test that we're loading our schedules right.
        /// </summary>
        [Test]
        public void LoadSectionWithBuiltInSchedulesTest()
        {
            var ss = (SchedulerSettings)ConfigurationManager.GetSection("shortformatconfig");
            var scheduler = ss.Schedulers[ss.DefaultScheduler];
            scheduler.Count.Should().Be(4);

            var interval = scheduler.GetScheduleElementByName("interval") as IntervalScheduleElement;
            interval.Should().NotBeNull();
            interval.FirstOccurrence.Value.Should().Be(new DateTime(2006, 11, 30, 15, 50, 0));
            interval.Interval.Value.Should().Be(new TimeSpan(0, 0, 5));

            var counted = scheduler.GetScheduleElementByName("counted") as IntervalCountedScheduleElement;
            counted.Should().NotBeNull();
            counted.Occurrences.Value.Should().Be(31);

            var weekly = scheduler.GetScheduleElementByName("weekly") as WeeklyScheduleElement;
            weekly.Should().NotBeNull();
            weekly.Days.Value.Length.Should().Be(2);
            weekly.Days.Value[0].Should().Be(DayOfWeek.Friday);
            weekly.Days.Value[1].Should().Be(DayOfWeek.Tuesday);

            var oneshot = scheduler.GetScheduleElementByName("oneshot") as OneShotScheduleElement;
            oneshot.Should().NotBeNull();
            oneshot.Time.Value.Should().Be(new DateTime(2007, 7, 1, 15, 20, 0));
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
                e.InnerException.Should().BeOfType<InvalidConfigurationException>();
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
            tasks.Should().NotBeNull();
            tasks.Count.Should().NotBe(0);

            // Validate tasks and properties are not null
            foreach (TaskElementBase task in tasks)
            {
                task.Should().NotBeNull();
                task.GetTaskInstance().Should().NotBeNull();
                task.Parameters.Should().NotBeNull();
            }

            // Validate generic task with schedulerefs
            var genericTask = tasks[2];
            genericTask.Should().NotBeNull();
            genericTask.Count.Should().Be(2);
            var scheduleref = genericTask[0];
            scheduleref.Should().NotBeNull();
            scheduleref.ScheduleName.Should().Be("interval");

            // Validate the app task
            var codeTask = tasks[3] as CodeTaskElement;
            codeTask.Should().NotBeNull();
            codeTask.Class.Should().Be(typeof(SimpleClass));
            codeTask.Method.Should().Be("SimpleMethod");

            // Validate the app task
            var appTask = tasks[5] as ApplicationTaskElement;
            appTask.Should().NotBeNull();
            appTask.Path.Should().Be(@"c:\mytestapplication.exe");
            appTask.Parameters["path"].Should().Be(@"c:\mytestapplication.exe");
            appTask.Parameters["someArgument"].Should().Be(@"test1");
            appTask.Parameters.Count.Should().Be(2);

            appTask = tasks[6] as ApplicationTaskElement;
            appTask.Should().NotBeNull();
            appTask.Path.Should().Be(@"c:\mytestapplication.exe");
            appTask.Parameters["path"].Should().Be(@"c:\mytestapplication.exe");
            appTask.Parameters["someArgument"].Should().Be(@"test2");
            appTask.Parameters.Count.Should().Be(3);

        }

        /// <summary>
        /// Test loading a scheduler section with logger config.
        ///</summary>
        
        [Test]
        public void LoadSectionWithLoggerConfigTest()
        {
            var ss = (SchedulerSettings)ConfigurationManager.GetSection("TaskAndLoggingSection");
            var scheduler = ss.Schedulers["config_with_loggers_used_in_unittests"];
            scheduler.Should().NotBeNull();

            var loggerElementCollection = scheduler.Loggers;
            loggerElementCollection.Should().NotBeNull();
            loggerElementCollection.Count.Should().NotBe(0);

            // Validate loggers and properties are not null
            foreach (LoggerElementBase loggerElement in loggerElementCollection)
            {
                loggerElement.Should().NotBeNull();
                loggerElement.GetLoggerInstance().Should().NotBeNull();
                loggerElement.Parameters.Should().NotBeNull();
            }

            // Validate generic logger
            var genericLogger = loggerElementCollection[0];
            genericLogger.Should().NotBeNull();
            genericLogger.GetLoggerInstance().Should().BeOfType<LoggerFactoryMock>();
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
            log4netLogger.Should().NotBeNull();
            
            ILoggerFactory factory = log4netLogger.GetLoggerInstance();
            factory.Should().NotBeNull();
            factory.Should().BeOfType<Log4netLoggerFactory>();
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
            loggerElement.Should().NotBeNull();

            ILoggerFactory factory = loggerElement.GetLoggerInstance();
            factory.Should().NotBeNull();
            factory.Should().BeOfType<ConsoleLoggerFactory>();
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
            loggerElement.Should().NotBeNull();
            loggerElement.Parameters.Count.Should().Be(2);

            var factory = loggerElement.GetLoggerInstance();
            factory.Should().NotBeNull();
            factory.Should().BeOfType<LoggerFactoryMock>();
        }

        /// <summary>
        /// Test loading a scheduler section with CodeTask config containing parameters.
        ///</summary>
        
        [Test]
        public void LoadSectionWithCodeTaskWithParametersConfigTest()
        {
            SchedulerSettings ss = (SchedulerSettings)ConfigurationManager.GetSection("CodeTaskWithParamsConfigSection");
            SchedulerElement schedulerElement = ss.Schedulers[ss.DefaultScheduler];
            schedulerElement.Tasks.Count.Should().Be(1);
            CodeTaskElement codeTaskElement = schedulerElement.Tasks[0] as CodeTaskElement;
            codeTaskElement.Should().NotBeNull();
            codeTaskElement.Class.Should().Be(typeof (SimpleClass));
            codeTaskElement.Method.Should().Be("OtherMethodWithParameters");
            codeTaskElement.Parameters.ContainsKey("param1").Should().BeTrue();
            codeTaskElement.Parameters.ContainsKey("param2").Should().BeTrue();
        }

        [Test]
        public void LoadSectionWithTaskWithParametersConfigTest()
        {
            SchedulerSettings ss = (SchedulerSettings)ConfigurationManager.GetSection("TaskAndLoggingSection");
            SchedulerElement schedulerElement = ss.Schedulers[ss.DefaultScheduler];
            schedulerElement.Tasks.Count.Should().Be(8);
            TaskElement taskElement = schedulerElement.Tasks[1] as TaskElement;
            taskElement.Should().NotBeNull();
            taskElement.TaskType.Should().Be(typeof(TaskMock));
            taskElement.AdditionalProperties["someParameter"].Should().Be("someValue");
            TaskMock task = taskElement.GetTaskInstance() as TaskMock;
            task.Should().NotBeNull();
            task.Param0.Should().Be("someValue");
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
            var left = new SchedulerSettings {DefaultScheduler = "left"};
            var right = new SchedulerSettings { DefaultScheduler = "right"};
            right.Should().NotBe(left);
        }

        [Test]
        public void Equals_EqualObjectsGivesTrue()
        {
            var left = new SchedulerSettings { DefaultScheduler = "same" };
            var right = new SchedulerSettings { DefaultScheduler = "same" };
            right.Should().Be(left);
        }

    }
}
