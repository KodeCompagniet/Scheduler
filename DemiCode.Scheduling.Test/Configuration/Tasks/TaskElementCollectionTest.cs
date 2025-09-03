using System.Configuration;
using DemiCode.Scheduling.Configuration.Tasks;
using DemiCode.Scheduling.Test;
using DemiCode.Configuration;
using NUnit.Framework;

namespace DemiCode.Scheduling.Tasks.Test
{
    /// <summary>
    ///This is a test class for DemiCode.Scheduling.Configuration.TaskElementCollection and is intended
    ///to contain all DemiCode.Scheduling.Configuration.TaskElementCollection Unit Tests
    ///</summary>
    [TestFixture]
    public class TaskElementCollectionTest
    {
        // Test helper class to expose protected methods for testing
        private class TestableTaskElementCollection : TaskElementCollection
        {
            public new ConfigurationElementCollectionType CollectionType => base.CollectionType;
            
            public new ConfigurationElement CreateNewElement()
            {
                return base.CreateNewElement();
            }
            
            public new ConfigurationElement CreateNewElement(string elementName)
            {
                return base.CreateNewElement(elementName);
            }
            
            public new object GetElementKey(ConfigurationElement element)
            {
                return base.GetElementKey(element);
            }
            
            public new bool IsElementName(string elementName)
            {
                return base.IsElementName(elementName);
            }
        }

        /// <summary>
        ///A test for CollectionType
        ///</summary>
        [Test]
        public void CollectionTypeTest()
        {
            var target = new TestableTaskElementCollection();
            var val = ConfigurationElementCollectionType.BasicMap;

            target.CollectionType.Should().Be(val);
        }

        /// <summary>
        ///A test for CreateNewElement ()
        ///</summary>
        [Test]
        public void CreateNewElementTest()
        {
            var target = new TestableTaskElementCollection();

            ConfigurationElement actual = target.CreateNewElement();
            actual.Should().BeOfType<TaskElement>();
        }

        /// <summary>
        ///A test for CreateNewElement (string)
        ///</summary>
        [Test]
        public void CreateNewElementTest1()
        {
            var target = new TestableTaskElementCollection();

            ConfigurationElement actual;

            string elementName = "task"; 
            actual = target.CreateNewElement(elementName);
            actual.Should().BeOfType<TaskElement>();

            elementName = "CodeTask";
            actual = target.CreateNewElement(elementName);
            actual.Should().BeOfType<CodeTaskElement>();

            elementName = "ApplicationTask";
            actual = target.CreateNewElement(elementName);
            actual.Should().BeOfType<ApplicationTaskElement>();
        }

        /// <summary>
        ///A test for CreateNewElement (string) with an unknown element name.
        ///</summary>
        
        [Test]
        public void CreateInvalidElementTest()
        {
            Assert.Throws<InvalidConfigurationException>(() =>
            {
                var target = new TestableTaskElementCollection();

                string elementName = "SomeTask";

                ConfigurationElement actual = target.CreateNewElement(elementName);
            });
        }


        /// <summary>
        ///A test for GetElementKey (ConfigurationElement)
        ///</summary>
        [Test]
        public void GetElementKeyTest()
        {
            var target = new TestableTaskElementCollection();

            ConfigurationElement element = new TaskElement();

            object actual = target.GetElementKey(element);
            actual.Should().NotBeNull();
        }

        /// <summary>
        ///A test for IsElementName (string)
        ///</summary>
        [Test]
        public void IsElementNameTest()
        {
            var target = new TestableTaskElementCollection();

            target.IsElementName(null).Should().BeFalse();
            target.IsElementName("").Should().BeFalse();

            target.IsElementName("task").Should().BeTrue();
            target.IsElementName("CodeTask").Should().BeTrue();
            target.IsElementName("ApplicationTask").Should().BeTrue();
        }

        /// <summary>
        ///A test for this[int index]
        ///</summary>
        [Test]
        public void ItemTest()
        {
            TaskElementCollection target = new TaskElementCollection();

            TaskElement val = new TaskElement(); 
            int index = 0; 

            target[index] = val;
        }

    }


}
