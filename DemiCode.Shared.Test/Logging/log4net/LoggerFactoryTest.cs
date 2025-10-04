using DemiCode.Logging;
using DemiCode.Logging.log4net;
using NUnit.Framework;

namespace DemiCode.Scheduling.Loggers.log4net.Test
{
    [TestFixture]
    public class LoggerFactoryTest
    {

        [Test]
        public void CreateTest()
        {
            ILoggerFactory factory = new Log4netLoggerFactory();
            ILogger logger = factory.CreateLogger("test");
            Assert.IsNotNull(logger);
        }

        [Test]
        public void CreateByTypeTest()
        {
            ILoggerFactory factory = new Log4netLoggerFactory();
            ILogger logger = factory.CreateLogger(typeof(LoggerFactoryTest));
            Assert.IsNotNull(logger);
        }

        [Test]
        public void CreateNDCTest()
        {
            ILoggerFactory factory = new Log4netLoggerFactory();

            using (factory.CreateNDC("context message"))
            {
            }
        }
    }
}