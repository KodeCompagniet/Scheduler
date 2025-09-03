using System;
using DemiCode.Shared.Factories;
using NUnit.Framework;

namespace DemiCode.Scheduling.Test
{
    /// <summary>
    ///This is a test class for DemiCode.Scheduling.FactoryConfigurationException and is intended
    ///to contain all DemiCode.Scheduling.FactoryConfigurationException Unit Tests
    ///</summary>
    [TestFixture]
    public class FactoryConfigurationExceptionTest
    {
        
        /// <summary>
        ///A test for FactoryConfigurationException ()
        ///</summary>
        [Test]
        public void ConstructorTest()
        {
            Action act = () => throw new FactoryConfigurationException();
            act.Should().Throw<FactoryConfigurationException>();
        }


        /// <summary>
        ///A test for FactoryConfigurationException (string)
        ///</summary>
        [Test]
        public void ConstructorTest2()
        {
            string message = "Hello world";
            FactoryConfigurationException target = new FactoryConfigurationException(message);

            target.Message.Should().Be(message);
        }

        /// <summary>
        ///A test for FactoryConfigurationException (string, Exception)
        ///</summary>
        [Test]
        public void ConstructorTest3()
        {
            string message = "message";
            Exception innerException = new ApplicationException();
            FactoryConfigurationException target = new FactoryConfigurationException(message, innerException);

            target.Message.Should().Be(message);
            target.InnerException.Should().BeSameAs(innerException);
        }

    }
}
