using System;
using System.Collections.Generic;
using System.IO;
using DemiCode.Scheduling.Schedules;
using NUnit.Framework;

namespace DemiCode.Scheduling.Test
{
    /// <summary>
    ///This is a test class for DemiCode.Scheduling.SchedulerFactory and is intended
    ///to contain all DemiCode.Scheduling.SchedulerFactory Unit Tests
    ///</summary>
    [TestFixture]
    public class SchedulerFactoryTest
    {

        /// <summary>
        ///Create Scheduler from config. Uses the defaultScheduler attribute.
        ///</summary>
        [Test]
        public void CreateDefaultSchedulerFromConfigTest()
        {
            using (var scheduler = SchedulerFactory.CreateFromConfig("scheduling"))
            {
                Assert.IsNotNull(scheduler, "CreateFromConfig did not return the expected value.");
                Assert.AreEqual(2, scheduler.Count);
            }
        }

        /// <summary>
        ///Create Scheduler from config. Uses a named scheduler overriding default scheduler.
        ///</summary>
        [Test]
        public void CreateFromConfig_CreateNamedScheduler()
        {
            var factory = SchedulerFactory.Create();
            using (var scheduler = factory.CreateFromConfig("scheduling", "lunch"))
            {
                Assert.IsNotNull(scheduler, "CreateFromConfig did not return the expected value.");
                Assert.AreEqual(1, scheduler.Count);
            }
        }


        [Test]
        public void CreateDefaultSchedulerFromConfigFileTest()
        {
            using (var scheduler = SchedulerFactory.CreateFromConfigFile(".\\DemiCode.Scheduling.test.dll.config", "scheduling"))
            {
                Assert.IsNotNull(scheduler, "CreateFromConfigFile did not return the expected value.");
                Assert.AreEqual(2, scheduler.Count);
            }
        }

        [Test]
        public void CreateNamedSchedulerFromConfigFileTest()
        {
            using (var scheduler = SchedulerFactory.CreateFromConfigFile(".\\DemiCode.Scheduling.test.dll.config", "scheduling", "lunch"))
            {
                Assert.IsNotNull(scheduler, "CreateFromConfigFile did not return the expected value.");
                Assert.AreEqual(1, scheduler.Count);
            }
        }

        /// <summary>
        ///Create Scheduler from config. Uses a named scheduler.
        ///</summary>
        [Test]
        public void CreateNamedSchedulerFromConfigTest()
        {
            using (var scheduler = SchedulerFactory.CreateFromConfig("scheduling", "test"))
            {
                Assert.IsNotNull(scheduler, "CreateFromConfig did not return the expected value.");
                Assert.AreEqual(0, scheduler.Count);
            }
        }

        [Test]
        public void CreateSchedulerTest()
        {
            using (var scheduler = SchedulerFactory.Create())
            {
                Assert.IsNotNull(scheduler, "Create did not return the expected value.");
                Assert.AreEqual(0, scheduler.Count);
            }
        }

        [Test]
        public void CreateSchedulerWithSchedulesTest()
        {
            var schedules = new List<ISchedule> {new OneShotSchedule(DateTime.Now.AddDays(1))};
            using (var scheduler = SchedulerFactory.Create(schedules))
            {
                Assert.IsNotNull(scheduler, "Create did not return the expected value.");
                Assert.AreEqual(1, scheduler.Count);
            }
        }

        [Test]
        [ExpectedException(typeof (ArgumentNullException))]
        public void CreateWithNullSchedulerNameTest()
        {
            using (SchedulerFactory.CreateFromConfig("scheduling", (string)null))
            {
            }
        }

        [Test]
        [ExpectedException(typeof (ArgumentNullException))]
        public void CreateWithNullSectionNameTest()
        {
            using (SchedulerFactory.CreateFromConfig(null, "test"))
            {
            }
        }

        
        [Test]
        public void CreateFromXmlTest()
        {
            using (
                SchedulerFactory.CreateFromXml(@"TestData\ScheduleConfiguration.xml",
                                               "TestData/SchedulerFactoryCreateFromXmlTest", null))
            {
            }
        }

        
        [Test]
        public void CreateFromXmlSampleSchedulesTest()
        {
            using (SchedulerFactory.CreateFromXml(@"TestData\ScheduleSamples.xml", "schedulers/OneShots", null))
            {
            }
            using (SchedulerFactory.CreateFromXml(@"TestData\ScheduleSamples.xml", "schedulers/Intervals", null))
            {
            }
            using (SchedulerFactory.CreateFromXml(@"TestData\ScheduleSamples.xml", "schedulers/VariousWeekly", null))
            {
            }
            using (SchedulerFactory.CreateFromXml(@"TestData\ScheduleSamples.xml", "schedulers/MyFirstScheduler", null))
            {
            }
        }

        
        [Test, ExpectedException(typeof (ArgumentNullException))]
        public void CreateFromXmlWithNullXpathTest()
        {
            using (SchedulerFactory.CreateFromXml(@"TestData\ScheduleConfiguration.xml", null, null))
            {
            }
        }

        [Test, ExpectedException(typeof (ArgumentNullException))]
        public void CreateFromXmlWithNullFilenameTest()
        {
            using (SchedulerFactory.CreateFromXml(null, "TestData/SchedulerFactoryCreateFromXmlTest"))
            {
            }
        }

        [Test, ExpectedException(typeof (ArgumentException))]
        public void CreateFromXmlWithBogusXpathTest()
        {
            using (
                SchedulerFactory.CreateFromXml(@"TestData\ScheduleConfiguration.xml",
                                               "TestData/Bugus/SchedulerFactoryCreateFromXmlTest", null))
            {
            }
        }

        [Test, ExpectedException(typeof (FileNotFoundException))]
        public void CreateFromXmlWithBogusFileTest()
        {
            using (
                SchedulerFactory.CreateFromXml(@"TestData\BogusScheduleConfiguration.xml",
                                               "TestData/SchedulerFactoryCreateFromXmlTest", null))
            {
            }
        }
    }
}