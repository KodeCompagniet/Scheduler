using NUnit.Framework;
using System;
using DemiCode.Shared.Factories;

namespace DemiCode.Scheduling.Test
{
    /// <summary>
    ///This is a test class for DemiCode.Scheduling.FactoryException and is intended
    ///to contain all DemiCode.Scheduling.FactoryException Unit Tests
    ///</summary>
    [TestFixture]
    public class FactoryExceptionTest
    {

        /// <summary>
        ///A test for FactoryException ()
        ///</summary>
        [Test]
        public void ConstructorTest()
        {
            Assert.Throws<FactoryException>(() =>
            {
                FactoryException target = new FactoryException();
                throw target;
            });
        }

        /// <summary>
        ///A test for FactoryException (string)
        ///</summary>
        [Test]
        public void ConstructorTest2()
        {
            string message = "message";
            FactoryException target = new FactoryException(message);

            target.Message.Should().Be(message);
        }

        /// <summary>
        ///A test for FactoryException (string, Exception)
        ///</summary>
        [Test]
        public void ConstructorTest3()
        {
            string message = "message";
            Exception innerException = new ApplicationException();
            FactoryException target = new FactoryException(message, innerException);

            target.Message.Should().Be(message);
            target.InnerException.Should().BeSameAs(innerException);
        }

    }


}
