using DemiCode.Configuration;
using NUnit.Framework;
using System;

namespace DemiCode.Scheduling.Test
{
    /// <summary>
    ///This is a test class for DemiCode.Scheduling.Configuration.InvalidConfigurationException and is intended
    ///to contain all DemiCode.Scheduling.Configuration.InvalidConfigurationException Unit Tests
    ///</summary>
    [TestFixture]
    public class InvalidConfigurationExceptionTest
    {

        /// <summary>
        ///A test for InvalidConfigurationException ()
        ///</summary>
        [Test]
        public void ConstructorTest()
        {
            InvalidConfigurationException target = new InvalidConfigurationException();
            target.Should().BeOfType<InvalidConfigurationException>();
        }

        /// <summary>
        ///A test for InvalidConfigurationException (string)
        ///</summary>
        [Test]
        public void ConstructorTest2()
        {
            string message = "Some message";

            InvalidConfigurationException target = new InvalidConfigurationException(message);
            target.Message.Should().Be(message);
        }

        /// <summary>
        ///A test for InvalidConfigurationException (string, Exception)
        ///</summary>
        [Test]
        public void ConstructorTest3()
        {
            string message = "Some message";
            Exception innerException = new InvalidOperationException();

            InvalidConfigurationException target = new InvalidConfigurationException(message, innerException);
            target.Message.Should().Be(message);
            target.InnerException.Should().BeSameAs(innerException);
        }

    }
}
