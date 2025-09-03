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
                scheduler.Should().NotBeNull("CreateFromConfig did not return the expected value.");
                scheduler.Count.Should().Be(2);
            }
        }

        /// <summary>
        ///Create Scheduler from config. Uses a named scheduler overriding default scheduler.
        ///</summary>
        [Test]
        public void CreateFromConfig_CreateNamedScheduler()
        {
            using (var scheduler = Scheduler.CreateFromConfig("scheduling", "lunch"))
            {
                scheduler.Should().NotBeNull("CreateFromConfig did not return the expected value.");
                scheduler.Count.Should().Be(1);
            }
        }


        [Test]
        public void CreateDefaultSchedulerFromConfigFileTest()
        {
            using (var scheduler = SchedulerFactory.CreateFromConfigFile("DemiCode.Scheduling.Test.dll.config", "scheduling"))
            {
                scheduler.Should().NotBeNull("CreateFromConfigFile did not return the expected value.");
                scheduler.Count.Should().Be(2);
            }
        }

        [Test]
        public void CreateNamedSchedulerFromConfigFileTest()
        {
            using (var scheduler = SchedulerFactory.CreateFromConfigFile("DemiCode.Scheduling.Test.dll.config", "scheduling", "lunch"))
            {
                scheduler.Should().NotBeNull("CreateFromConfigFile did not return the expected value.");
                scheduler.Count.Should().Be(1);
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
                scheduler.Should().NotBeNull("CreateFromConfig did not return the expected value.");
                scheduler.Count.Should().Be(0);
            }
        }

        [Test]
        public void CreateSchedulerTest()
        {
            using (var scheduler = SchedulerFactory.Create())
            {
                scheduler.Should().NotBeNull("Create did not return the expected value.");
                scheduler.Count.Should().Be(0);
            }
        }

        [Test]
        public void CreateSchedulerWithSchedulesTest()
        {
            var schedules = new List<ISchedule> {new OneShotSchedule(DateTime.Now.AddDays(1))};
            using (var scheduler = SchedulerFactory.Create(schedules))
            {
                scheduler.Should().NotBeNull("Create did not return the expected value.");
                scheduler.Count.Should().Be(1);
            }
        }

        [Test]
        public void CreateWithNullSchedulerNameTest()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                using (SchedulerFactory.CreateFromConfig("scheduling", (string)null))
                {
                }
            });
        }

        [Test]
        public void CreateWithNullSectionNameTest()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                using (SchedulerFactory.CreateFromConfig(null, "test"))
                {
                }
            });
        }

        
        [Test]
        [Ignore("SchedulerFactory.CreateFromXml has been removed. Use CreateFromConfigFile with .NET configuration files instead.")]
        public void CreateFromXmlTest()
        {
            // This test used the removed CreateFromXml method which loaded custom XML files.
            // The modern approach is to use CreateFromConfigFile with .NET configuration files.
            // using (
            //     SchedulerFactory.CreateFromXml(@"TestData\ScheduleConfiguration.xml",
            //                                    "TestData/SchedulerFactoryCreateFromXmlTest", null))
            // {
            // }
        }

        
        [Test]
        [Ignore("SchedulerFactory.CreateFromXml has been removed. Use CreateFromConfigFile with .NET configuration files instead.")]
        public void CreateFromXmlSampleSchedulesTest()
        {
            // This test used the removed CreateFromXml method which loaded custom XML files.
            // using (SchedulerFactory.CreateFromXml(@"TestData\ScheduleSamples.xml", "schedulers/OneShots", null))
            // {
            // }
            // using (SchedulerFactory.CreateFromXml(@"TestData\ScheduleSamples.xml", "schedulers/Intervals", null))
            // {
            // }
            // using (SchedulerFactory.CreateFromXml(@"TestData\ScheduleSamples.xml", "schedulers/VariousWeekly", null))
            // {
            // }
            // using (SchedulerFactory.CreateFromXml(@"TestData\ScheduleSamples.xml", "schedulers/MyFirstScheduler", null))
            // {
            // }
        }

        
        [Test]
        [Ignore("SchedulerFactory.CreateFromXml has been removed.")]
        public void CreateFromXmlWithNullXpathTest()
        {
            // Assert.Throws<ArgumentNullException>(() =>
            // {
            //     using (SchedulerFactory.CreateFromXml(@"TestData\ScheduleConfiguration.xml", null, null))
            //     {
            //     }
            // });
        }

        [Test]
        [Ignore("SchedulerFactory.CreateFromXml has been removed.")]
        public void CreateFromXmlWithNullFilenameTest()
        {
            // Assert.Throws<ArgumentNullException>(() =>
            // {
            //     using (SchedulerFactory.CreateFromXml(null, "TestData/SchedulerFactoryCreateFromXmlTest"))
            //     {
            //     }
            // });
        }

        [Test]
        [Ignore("SchedulerFactory.CreateFromXml has been removed.")]
        public void CreateFromXmlWithBogusXpathTest()
        {
            // Assert.Throws<ArgumentException>(() =>
            // {
            //     using (
            //         SchedulerFactory.CreateFromXml(@"TestData\ScheduleConfiguration.xml",
            //                                        "TestData/Bugus/SchedulerFactoryCreateFromXmlTest", null))
            //     {
            //     }
            // });
        }

        [Test]
        [Ignore("SchedulerFactory.CreateFromXml has been removed.")]
        public void CreateFromXmlWithBogusFileTest()
        {
            // Assert.Throws<FileNotFoundException>(() =>
            // {
            //     using (
            //         SchedulerFactory.CreateFromXml(@"TestData\BogusScheduleConfiguration.xml",
            //                                        "TestData/SchedulerFactoryCreateFromXmlTest", null))
            //     {
            //     }
            // });
        }
    }
}
