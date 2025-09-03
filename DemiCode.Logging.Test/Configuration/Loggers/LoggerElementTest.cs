using System;
using DemiCode.Logging.Loggers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DemiCode.Logging.Configuration.Loggers.Test
{
    [TestClass]
    public class LoggerElementTest
    {
        [TestMethod]
        public void ConstructorTest()
        {
            new LoggerElement();
        }

        [TestMethod]
        public void Constructor2Test()
        {
            var element = new LoggerElement(typeof(NilLoggerFactory));
            element.LoggerType.Should().Be(typeof(NilLoggerFactory));
        }

        [TestMethod]
        public void GetLoggerTest()
        {
            var factory = new LoggerElement(typeof(NilLoggerFactory)).GetLoggerInstance();
            factory.Should().NotBeNull();
            factory.Should().BeOfType<NilLoggerFactory>();
        }

        [ExpectedException(typeof(InvalidOperationException))]
        [TestMethod]
        public void GetLogger2Test()
        {
            new LoggerElement().GetLoggerInstance();
        }

        [TestMethod]
        public void EqualsTest()
        {
            var left = new LoggerElement();
            var right = new LoggerElement();
            right.Should().Be(left);
        }

        [TestMethod]
        public void Equals2Test()
        {
            var left = new LoggerElement(typeof(NilLoggerFactory));
            var right = new LoggerElement(typeof(NilLoggerFactory));
            right.Should().Be(left);
        }

        [TestMethod]
        public void Equals3Test()
        {
            var left = new LoggerElement(typeof(NilLoggerFactory));
            var right = new LoggerElement(typeof(ConsoleLoggerFactory));
            right.Should().NotBe(left);
        }

    }
}
