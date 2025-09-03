using NUnit.Framework;
using System.Configuration;
using DemiCode.Scheduling.Test;

namespace DemiCode.Scheduling.Configuration.Test
{
    /// <summary>
    ///This is a test class for DemiCode.Scheduling.Configuration.SchedulerElementCollection and is intended
    ///to contain all DemiCode.Scheduling.Configuration.SchedulerElementCollection Unit Tests
    ///</summary>
    [TestFixture]
    public class SchedulerElementCollectionTest
    {
        // Test helper class to expose protected methods for testing
        private class TestableSchedulerElementCollection : SchedulerElementCollection
        {
            public new ConfigurationElement CreateNewElement()
            {
                return base.CreateNewElement();
            }
        }

        /// <summary>
        ///A test for CreateNewElement ()
        ///</summary>
        
        [Test]
        public void CreateNewElementTest()
        {
            var target = new TestableSchedulerElementCollection();

            ConfigurationElement actual = target.CreateNewElement();
            actual.Should().NotBeNull();
        }

        /// <summary>
        ///A test for this[int index]
        ///</summary>
        [Test]
        public void ItemTest()
        {
            SchedulerSettings ss = ConfigurationManager.GetSection("scheduling") as SchedulerSettings;
            ss.Should().NotBeNull();

            SchedulerElementCollection target = ss.Schedulers;
            target.Count.Should().NotBe(0);
            SchedulerElement val = target[0];
            val.Name.Should().Be("prod");
        }

        /// <summary>
        ///A test for setting this[int index]. Should give an exception since our collection is readonly.
        ///</summary>
        [Test]
        public void SetItemWithReadOnlyCollectionTest()
        {
            Assert.Throws<ConfigurationErrorsException>(() =>
            {
                SchedulerSettings ss = ConfigurationManager.GetSection("scheduling") as SchedulerSettings;
                ss.Should().NotBeNull();

                SchedulerElementCollection target = ss.Schedulers;

                SchedulerElement val = new SchedulerElement();
                val.Name = "newelement";

                int index = 0;

                target[index] = val;
            });
        }

        /// <summary>
        ///A test for setting this[int index]. 
        ///</summary>
        [Test]
        public void SetItemTest()
        {
            SchedulerSettings settings = new SchedulerSettings();
            SchedulerElementCollection schedulers = settings.Schedulers;

            SchedulerElement val = new SchedulerElement();
            val.Name = "defaultScheduler";

            int index = 0;

            schedulers[index] = val;

            schedulers[index].Name.Should().Be("defaultScheduler");
        }

        [Test]
        public void AddItemTest()
        {
            SchedulerSettings settings = new SchedulerSettings();
            SchedulerElementCollection schedulers = settings.Schedulers;

            SchedulerElement scheduler = new SchedulerElement();
            scheduler.Name = "defaultScheduler";
            schedulers.Add(scheduler);

            schedulers.Count.Should().Be(1);
        }
    }


}
