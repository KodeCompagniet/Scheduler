using System.IO;
using System.Threading;
using DemiCode.Scheduling.Configuration;
using DemiCode.Scheduling.Test;
using DemiCode.Scheduling.Test.Mocks;
using log4net.Core;
using NUnit.Framework;

namespace DemiCode.Scheduling.Service.Test
{
    [TestFixture]
    public class WatchConfigFileTest
    {

        /// <summary>
        /// This test asserts a file watcher event modifying a scheduler.
        /// </summary>
        [Test]
        public void InitTest()
        {
            // Create default scheduler from configuration
            using (var scheduler = SchedulerMock.CreateFromConfig("FileWatchConfigSection"))
            {
                // Watch configuration file for changes
                using (var watcher = new ConfigurationFileWatcher())
                {
                    // Start the engine
                    scheduler.Start();

                    using (var trigger = new AutoResetEvent(false))
                    {
                        watcher.ConfigurationFileChanged +=
                            delegate
                            {
                                scheduler.Stop();
                                trigger.Set();
                            };

                        Thread.Sleep(50);

                        FileHelper.TouchFile(watcher.ConfigFile.FullName);

                        trigger.WaitOne(2000, true).Should().BeTrue();
                        scheduler.IsRunning.Should().BeFalse();
                    }
                }
            }
        }

        [Test]
        public void AddTaskAtRuntimeTest()
        {
            var sectionName = "AddTaskAtRuntimeTest";
            Scheduler scheduler = null;

            FileHelper.BackupConfigFile();
            SimpleClass.ResetTrackerFields();

            // Watch configuration file for changes
            try
            {
                using (var watcher = new ConfigurationFileWatcher())
                {
                    scheduler = SchedulerMock.CreateFromConfig(sectionName);
                    scheduler.Start();

                    Thread.Sleep(1000);
                    SimpleClass.ExtendedMethodCalled.Should().BeTrue();
                    SimpleClass.ExtendedMethodWithOneParamCalled.Should().BeFalse();

                    using (var trigger = new AutoResetEvent(false))
                    {
                        ConfigurationFileChangedEventHandler x = 
                            delegate
                            {
                                scheduler.Should().NotBeNull();

                                scheduler.Stop();
                                scheduler.Dispose();

                                // NOTE: important to reset trackers here, lest the asserts later on will be faulty
                                SimpleClass.ResetTrackerFields();

                                scheduler = SchedulerFactory.CreateFromConfig(sectionName, true);
                                scheduler.Start();

                                trigger.Set();
                            };

                        watcher.ConfigurationFileChanged += x;

                        FileHelper.ReplaceConfigFile(Path.Combine("TestData", "AddTaskAtRuntimeTest.config"));

                        // Wait for the watcher to trigger the change and reconfigure the scheduler
                        trigger.WaitOne(2000, true).Should().BeTrue();
                        watcher.ConfigurationFileChanged -= x;

                        // Let the schedules run...
                        Thread.Sleep(1200);

                        SimpleClass.ExtendedMethodCalled.Should().BeFalse();
                        SimpleClass.ExtendedMethodWithOneParamCalled.Should().BeTrue();
                    }
                }
            }
            finally
            {
                if (scheduler != null)
                {
                    scheduler.Dispose();
                }

                FileHelper.RestoreConfigFile();
            }
        }

        [Test]
        public void AddNewAssemblyAtRuntimeTest()
        {
            AssertLog.Configure(Path.Combine("TestData", "AddNewAssemblyAtRuntimeTest.log4net.config"));

            const string sectionName = "AddNewAssemblyAtRuntimeTest";
            Scheduler scheduler = null;

            FileHelper.BackupConfigFile();

            try
            {
                using (var watcher = new ConfigurationFileWatcher())
                {
                    scheduler = SchedulerMock.CreateFromConfig(sectionName);
                    scheduler.Start();

                    Thread.Sleep(2000);

                    using (var trigger = new AutoResetEvent(false))
                    {
                        watcher.ConfigurationFileChanged +=
                            delegate
                            {
                                if (scheduler != null)
                                {
                                    if (!Monitor.TryEnter(scheduler))
                                        return;
                                    lock (scheduler)
                                    {
                                        scheduler.Stop();
                                        scheduler.Dispose();

                                        var newScheduler = SchedulerFactory.CreateFromConfig(sectionName, true);
                                        newScheduler.Start();

                                        scheduler = newScheduler;
                                    }
                                }

                                trigger.Set();
                            };

                        FileHelper.ReplaceConfigFile(Path.Combine("TestData", "AddNewAssemblyAtRuntimeTest.config"));
                        AssertLog.ClearLog();

                        // Wait for the watcher to trigger the change and reconfigure the scheduler
                        trigger.WaitOne(4000, true).Should().BeTrue();

                        // Let the schedules run...
                        Thread.Sleep(1500);

                        // Here, the external task should be executed. Using log4net memoryappender we can track messages 
                        // from the TaskHandler/ITask infrastructure.
                        AssertLog.EventCountIsNotEqual(0);

                        LoggingEvent testDummyEvent = null;
                        foreach (var loggingEvent in AssertLog.GetEvents())
                        {
                            if (loggingEvent.MessageObject.ToString() == "TestDummy called!")
                            {
                                testDummyEvent = loggingEvent;
                                break;
                            }
                        }
                        testDummyEvent.Should().NotBeNull();
                        testDummyEvent.Level.Should().Be(Level.Debug);               
                    }
                }
            }
            finally
            {
                if (scheduler != null)
                {
                    scheduler.Dispose();
                }

                FileHelper.RestoreConfigFile();
            }
        }
    }
}
