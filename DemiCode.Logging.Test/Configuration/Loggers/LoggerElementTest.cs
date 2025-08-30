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
            Assert.AreEqual(typeof (NilLoggerFactory), element.LoggerType);
        }

        [TestMethod]
        public void GetLoggerTest()
        {
            var factory = new LoggerElement(typeof(NilLoggerFactory)).GetLoggerInstance();
            Assert.IsNotNull(factory);
            Assert.IsInstanceOfType(factory, typeof(NilLoggerFactory));
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
            Assert.AreEqual(left, right);
        }

        [TestMethod]
        public void Equals2Test()
        {
            var left = new LoggerElement(typeof(NilLoggerFactory));
            var right = new LoggerElement(typeof(NilLoggerFactory));
            Assert.AreEqual(left, right);
        }

        [TestMethod]
        public void Equals3Test()
        {
            var left = new LoggerElement(typeof(NilLoggerFactory));
            var right = new LoggerElement(typeof(ConsoleLoggerFactory));
            Assert.AreNotEqual(left, right);
        }

    }
}
