using DemiCode.Logging.Configuration.Loggers.log4net;
using DemiCode.Logging.Loggers;
using DemiCode.Logging.Loggers.log4net;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DemiCode.Logging.Configuration.Loggers.log4net.Test
{
    [TestClass]
    public class Log4netLoggerElementTest
    {
        [TestMethod]
        public void ConstructorTest()
        {
            new Log4netLoggerElement();
        }

        [TestMethod]
        public void GetLoggerTest()
        {
            ILoggerFactory factory = new Log4netLoggerElement().GetLoggerInstance();
            Assert.IsNotNull(factory);
            Assert.IsInstanceOfType(factory, typeof(Log4netLoggerFactory));
        }

        [TestMethod]
        public void EqualsTest()
        {
            var left = new Log4netLoggerElement();
            var right = new Log4netLoggerElement();
            Assert.AreEqual(left, right);
        }
    }
}
