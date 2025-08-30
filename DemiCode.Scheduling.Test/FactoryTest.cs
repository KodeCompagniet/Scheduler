using System;
using System.Reflection;
using System.Xml;
using DemiCode.Scheduling.Test.Mocks;
using NUnit.Framework;
using DemiCode.Shared.Factories;

namespace DemiCode.Scheduling.Test
{
    /// <summary>
    ///This is a test class for DemiCode.Scheduling.Factory and is intended
    ///to contain all DemiCode.Scheduling.Factory Unit Tests
    ///</summary>
    [TestFixture]
    public class FactoryTest
    {

        ///<summary>
        ///A test for creating instances from types in GAC'ed assemblies.
        ///</summary>
        [Test]
        [Ignore("Creating types from GAC is not tested yet")]
        public void CreateInstanceFromGACedAssemblyTest()
        {
        }

        /// <summary>
        ///A test for CreateInstance&lt;&gt; (string, XmlNode)
        ///</summary>
        [Test]
        public void CreateInstanceTest()
        {
            XmlNode configurationNode = LoadConfigurationNode();
            IConfigurableMock target = Factory.CreateInstance<IConfigurableMock>("DemiCode.Scheduling.Test.Mocks.ConfigurableMock, DemiCode.Scheduling.Test", configurationNode);

            Assert.IsNotNull(target);
        }

        /// <summary>
        ///A test for CreateInstance&lt;&gt; (Type, XmlNode) with type parameter == null
        ///</summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CreateInstanceWithNullTypeTest()
        {
            XmlNode configurationNode = LoadConfigurationNode();
            Type typeToCreate = null;
            IConfigurableMock target = Factory.CreateInstance<IConfigurableMock>(typeToCreate, configurationNode);
        }

        /// <summary>
        ///A test for CreateInstance&lt;&gt; (Type, XmlNode) with type parameter == null
        ///</summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CreateInstanceWithNullType2Test()
        {
            Type typeToCreate = null;
            IConfigurableMock target = Factory.CreateInstance<IConfigurableMock>(typeToCreate);
        }

        /// <summary>
        ///A test for CreateInstance&lt;&gt; (Type, XmlNode) with type config == null
        ///</summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CreateInstanceWithNullConfigTest()
        {
            XmlNode config = null;
            IConfigurableMock target = Factory.CreateInstance<IConfigurableMock>(typeof(ConfigurableMock), config);
        }

        [Test]
        public void CreateWithTypeAsStringTest()
        {
            SimpleClass instance = Factory.CreateInstance<SimpleClass>(typeof(SimpleClass).AssemblyQualifiedName);
            Assert.IsNotNull(instance);
        }

        [Test]
        [ExpectedException(typeof (ArgumentNullException))]
        public void CreateWithNullAsStringTest()
        {
            string type = null;
            Factory.CreateInstance<SimpleClass>(type);
        }

        /// <summary>
        ///A test for CreateInstance&lt;&gt; (Type, XmlNode)
        ///</summary>
        [Test]
        public void CreateInstanceTest1()
        {
            XmlNode configurationNode = LoadConfigurationNode();
            IConfigurableMock target = Factory.CreateInstance<IConfigurableMock>(typeof(ConfigurableMock), configurationNode);

            Assert.IsNotNull(target);
        }

        /// <summary>
        ///A test for CreateInstance&lt;&gt; (XmlNode)
        ///</summary>
        [Test]
        public void CreateInstanceTest2()
        {
            XmlNode configurationNode = LoadConfigurationNode();
            IConfigurableMock target = Factory.CreateInstance<IConfigurableMock>(configurationNode);

            Assert.IsNotNull(target);
        }

        [Test]
        [ExpectedException(typeof(FactoryException))]
        public void CreateTypeWithoutRequiredConstructorTest()
        {
            XmlNode configurationNode = LoadConfigurationNode("TestData/NoValidConstructorTest");
            IConfigurableMock target = Factory.CreateInstance<IConfigurableMock>(configurationNode);
        }

        [Test]
        
        [ExpectedException(typeof(FactoryConfigurationException))]
        public void CreateFromNodeWithoutTypeAttributeTest()
        {
            XmlNode configurationNode = LoadConfigurationNode("TestData/NoTypeAttribTest");
            IConfigurableMock target = Factory.CreateInstance<IConfigurableMock>(configurationNode);
        }

        [Test]
        [ExpectedException(typeof(FactoryException))]
        public void CreateFromNonexistingTypeTest()
        {
            string typeName = "SomeBogusType";
            XmlNode configurationNode = LoadConfigurationNode("TestData/ConfigurationTest");
            IConfigurableMock target = Factory.CreateInstance<IConfigurableMock>(typeName, configurationNode);
        }

        [Test]
        [ExpectedException(typeof(FactoryException))]
        public void CreateFromNonInterfaceTemplateTest()
        {
            XmlNode configurationNode = LoadConfigurationNode("TestData/ConfigurationTest");
            String target = Factory.CreateInstance<String>(configurationNode);
        }

        [Test]
        public void CreateFromNonInterfaceTypeTest()
        {
            SimpleClass target = Factory.CreateInstance<SimpleClass>(typeof(SimpleClass));
            Assert.IsNotNull(target);
        }
        /// <summary>
        /// The type configured in our xml does not implement ISecondConfigurableMock
        /// and should thus fail.
        /// </summary>
        [Test]
        [ExpectedException(typeof(FactoryException))]
        public void CreateTypeWithoutInterfaceTest()
        {
            XmlNode configurationNode = LoadConfigurationNode();
            ISecondConfigurableMock target = Factory.CreateInstance<ISecondConfigurableMock>(configurationNode);
        }

        /// <summary>
        /// The configured class throws a <see cref="TargetInvocationException"/> and thus provoces running
        /// the exception handler.
        /// </summary>
        [Test]
        [ExpectedException(typeof(FactoryException))]
        public void CreateTypeWithTargetInvocationExceptionConstructorTest()
        {
            XmlNode configurationNode = LoadConfigurationNode("TestData/TargetInvocationExceptionTest");
            IConfigurableMock target = Factory.CreateInstance<IConfigurableMock>(configurationNode);
        }

        /// <summary>
        /// The configured class throws a <see cref="InvalidOperationException"/> and thus provoces running
        /// the exception handler.
        /// </summary>
        [Test]
        [Ignore("Not able to provoke the last exception handler in Factory.CreateInstance(Type typeToCreate, XmlNode config)")]
        [ExpectedException(typeof(FactoryException))]
        public void CreateTypeWithBadConstructorTest()
        {
            XmlNode configurationNode = LoadConfigurationNode("TestData/BadConstructorTest");
            IConfigurableMock target = Factory.CreateInstance<IConfigurableMock>(configurationNode);
        }

        [Test]
        public void CreateTypeWithParametersInConstructorTest()
        {
            SimpleClass simpleClass = 
                Factory.CreateInstance<SimpleClass>(typeof (SimpleClass), "paramValue");
            Assert.IsNotNull(simpleClass);
            Assert.AreEqual("paramValue", simpleClass.ConstructorParameter1);
        }

        [Test]
        [ExpectedException(typeof (FactoryException))]
        public void CreateTypeWithBadConstructorTest2()
        {
            Factory.CreateInstance<SimpleClass>(typeof(SimpleClass), true);
        }

        [Test]
        public void CreateTypeWithZeroParametersInConstructorTest()
        {
            SimpleClass simpleClass =
                Factory.CreateInstance<SimpleClass>(typeof(SimpleClass));
            Assert.IsNotNull(simpleClass);
        }

        [Test]
        [ExpectedException(typeof(FactoryException))]
        public void CreateTypeWithMissingConstructorTest()
        {
            Factory.CreateInstance<SimpleClass>(typeof(SimpleClass), "paramValue", 10, 20, 30, 40);
        }

        private static XmlNode LoadConfigurationNode()
        {
            return LoadConfigurationNode("TestData/ConfigurationTest");
        }

        private static XmlNode LoadConfigurationNode(string xpath)
        {
            XmlDocument xd = new XmlDocument();
            xd.Load(@"TestData\FactoryConfiguration.xml");
            XmlNode configurationNode = xd.SelectSingleNode(xpath);
            return configurationNode;
        }
    }
}
