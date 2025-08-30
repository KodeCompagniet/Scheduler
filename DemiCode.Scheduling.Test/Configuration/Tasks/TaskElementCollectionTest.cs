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

        /// <summary>
        ///A test for CollectionType
        ///</summary>
        [Test]
        public void CollectionTypeTest()
        {
            var target = DemiCode_Scheduling_Configuration_TaskElementCollectionAccessor.CreatePrivate();
            var val = ConfigurationElementCollectionType.BasicMap;

            Assert.AreEqual(val, target.CollectionType, "DemiCode.Scheduling.Configuration.TaskElementCollection.CollectionType was not set correctly.");
        }

        /// <summary>
        ///A test for CreateNewElement ()
        ///</summary>
        [Test]
        public void CreateNewElementTest()
        {
            TaskElementCollection target = DemiCode_Scheduling_Configuration_TaskElementCollectionAccessor.CreatePrivate();

            DemiCode_Scheduling_Configuration_TaskElementCollectionAccessor accessor = new DemiCode_Scheduling_Configuration_TaskElementCollectionAccessor(target);

            ConfigurationElement actual = accessor.CreateNewElement();
            Assert.IsInstanceOfType(typeof(TaskElement), actual);
        }

        /// <summary>
        ///A test for CreateNewElement (string)
        ///</summary>
        [Test]
        public void CreateNewElementTest1()
        {
            TaskElementCollection target = DemiCode_Scheduling_Configuration_TaskElementCollectionAccessor.CreatePrivate();

            DemiCode_Scheduling_Configuration_TaskElementCollectionAccessor accessor = new DemiCode_Scheduling_Configuration_TaskElementCollectionAccessor(target);

            ConfigurationElement actual;

            string elementName = "task"; 
            actual = accessor.CreateNewElement(elementName);
            Assert.IsInstanceOfType(typeof(TaskElement), actual);

            elementName = "CodeTask";
            actual = accessor.CreateNewElement(elementName);
            Assert.IsInstanceOfType(typeof(CodeTaskElement), actual);

            elementName = "ApplicationTask";
            actual = accessor.CreateNewElement(elementName);
            Assert.IsInstanceOfType(typeof(ApplicationTaskElement), actual);
        }

        /// <summary>
        ///A test for CreateNewElement (string) with an unknown element name.
        ///</summary>
        
        [Test]
        [ExpectedException(typeof(InvalidConfigurationException))]
        public void CreateInvalidElementTest()
        {
            TaskElementCollection target = DemiCode_Scheduling_Configuration_TaskElementCollectionAccessor.CreatePrivate();
            DemiCode_Scheduling_Configuration_TaskElementCollectionAccessor accessor = new DemiCode_Scheduling_Configuration_TaskElementCollectionAccessor(target);

            string elementName = "SomeTask";

            ConfigurationElement actual = accessor.CreateNewElement(elementName);
        }


        /// <summary>
        ///A test for GetElementKey (ConfigurationElement)
        ///</summary>
        [Test]
        public void GetElementKeyTest()
        {
            TaskElementCollection target = DemiCode_Scheduling_Configuration_TaskElementCollectionAccessor.CreatePrivate();
            DemiCode_Scheduling_Configuration_TaskElementCollectionAccessor accessor = new DemiCode_Scheduling_Configuration_TaskElementCollectionAccessor(target);

            ConfigurationElement element = new TaskElement();

            object actual = accessor.GetElementKey(element);
            Assert.IsNotNull(actual);
        }

        /// <summary>
        ///A test for IsElementName (string)
        ///</summary>
        [Test]
        public void IsElementNameTest()
        {
            TaskElementCollection target = DemiCode_Scheduling_Configuration_TaskElementCollectionAccessor.CreatePrivate();
            DemiCode_Scheduling_Configuration_TaskElementCollectionAccessor accessor = new DemiCode_Scheduling_Configuration_TaskElementCollectionAccessor(target);

            Assert.IsFalse(accessor.IsElementName(null));
            Assert.IsFalse(accessor.IsElementName(""));

            Assert.IsTrue(accessor.IsElementName("task"));
            Assert.IsTrue(accessor.IsElementName("CodeTask"));
            Assert.IsTrue(accessor.IsElementName("ApplicationTask"));
        }

        /// <summary>
        ///A test for this[int index]
        ///</summary>
        [Test]
        public void ItemTest()
        {
            TaskElementCollection target = DemiCode_Scheduling_Configuration_TaskElementCollectionAccessor.CreatePrivate();

            TaskElement val = new TaskElement(); 
            int index = 0; 

            target[index] = val;
        }

    }


}
