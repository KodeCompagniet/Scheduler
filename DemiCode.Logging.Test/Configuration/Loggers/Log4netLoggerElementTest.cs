using DemiCode.Logging.log4net.Configuration;
using DemiCode.Logging.Loggers;
using DemiCode.Logging.log4net;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DemiCode.Logging.Test.Configuration.Loggers
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
            factory.Should().NotBeNull();
            factory.Should().BeOfType<Log4netLoggerFactory>();
        }

        [TestMethod]
        public void EqualsTest()
        {
            var left = new Log4netLoggerElement();
            var right = new Log4netLoggerElement();
            right.Should().Be(left);
        }
    }
}
