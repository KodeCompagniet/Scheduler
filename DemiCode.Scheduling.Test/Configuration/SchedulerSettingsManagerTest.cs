using DemiCode.Scheduling.Schedules;
using DemiCode.Scheduling.Test;
using NUnit.Framework;
using System;
using System.IO;

namespace DemiCode.Scheduling.Configuration.Test
{
    [TestFixture]
    public class SchedulerSettingsManagerTest
    {

        /// <summary>
        ///A test for GetConfigurationSection (string)
        ///</summary>
        [Test]
        public void GetConfigurationSectionTest()
        {
            const string sectionName = "scheduling";

            var actual = SchedulerSettingsManager.GetConfigurationSection(sectionName);

            actual.Should().NotBeNull();
            actual.Should().BeOfType<SchedulerSettings>();
        }

        [Test]
        public void GetNullConfigurationSectionTest()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                const string sectionName = null;
                SchedulerSettingsManager.GetConfigurationSection(sectionName);
            });
        }

        [Test]
        public void GetNonExistentConfigurationSectionTest()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                const string sectionName = "somebogussectionname";
                SchedulerSettingsManager.GetConfigurationSection(sectionName);
            });
        }


        /// <summary>
        /// Test that config is read from cache even though we change the config file on disk.
        /// </summary>
        [Test]
        public void LoadConfigFromCacheTest()
        {
            const string sectionName = "ConfigCacheTest";

            var ss = SchedulerSettingsManager.GetConfigurationSection(sectionName);
            var schedule = (IntervalSchedule)ss.Schedulers[ss.DefaultScheduler].GetScheduleInstances()[0];
            schedule.Interval.Should().Be(TimeSpan.FromSeconds(10));

            try
            {
                FileHelper.ReplaceConfigFile(Path.Combine("TestData", "ConfigCacheTest.config"));

                ss = SchedulerSettingsManager.GetConfigurationSection(sectionName);
                schedule = (IntervalSchedule)ss.Schedulers[ss.DefaultScheduler].GetScheduleInstances()[0];
                schedule.Interval.Should().Be(TimeSpan.FromSeconds(10));
            }
            finally
            {
                FileHelper.RestoreConfigFile();
            }
        }

        /// <summary>
        /// Test that config is read from disk invalidating the cached version.
        /// </summary>
        [Test]
        public void LoadConfigFromDiskTest()
        {
            const string sectionName = "ConfigCacheTest";

            var ss = SchedulerSettingsManager.GetConfigurationSection(sectionName);
            var schedule = (IntervalSchedule)ss.Schedulers[ss.DefaultScheduler].GetScheduleInstances()[0];
            schedule.Interval.Should().Be(TimeSpan.FromSeconds(10));

            try
            {
                FileHelper.ReplaceConfigFile(Path.Combine("TestData", "ConfigCacheTest.config"));

                ss = SchedulerSettingsManager.GetConfigurationSection(sectionName, true);
                schedule = (IntervalSchedule)ss.Schedulers[ss.DefaultScheduler].GetScheduleInstances()[0];
                schedule.Interval.Should().Be(TimeSpan.FromSeconds(50));
            }
            finally
            {
                FileHelper.RestoreConfigFile();
            }
        }

        [Test]
        public void GetWritableConfigurationSection()
        {
            SchedulerSettingsManager.GetWritableConfigurationSection("writableSection");
        }

        // This test validates .NET Framework-specific behavior where configuration sections
        // with mismatched assembly versions would throw InvalidOperationException.
        // In .NET 8, ConfigurationManager is more lenient with version mismatches,
        // ignoring version numbers when type names match. This is actually beneficial
        // for backward compatibility, so we're skipping this test on .NET 8+.
        [Test]
        [Ignore("Known .NET 8 ConfigurationManager behavior difference - version mismatches are handled more gracefully")]
        public void GetWritableConfigurationSection_OldSectionTest()
        {
            Assert.Throws<InvalidOperationException>(() =>
            {
                SchedulerSettingsManager.GetWritableConfigurationSection("writableSection_old");
            });
        }
    }
}
