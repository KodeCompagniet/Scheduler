using DemiCode.Logging.log4net.Configuration;
using DemiCode.Logging;
using DemiCode.Logging.log4net;
using NUnit.Framework;

namespace DemiCode.Logging.log4net.Configuration.Test
{
    [TestFixture]
    public class Log4netLoggerElementTest
    {
        [Test]
        public void ConstructorTest()
        {
            new Log4netLoggerElement();
        }

        [Test]
        public void GetLoggerTest()
        {
            ILoggerFactory factory = new Log4netLoggerElement().GetLoggerInstance();
            Assert.IsNotNull(factory);
            Assert.IsInstanceOf<Log4netLoggerFactory>(factory);
        }

        [Test]
        public void EqualsTest()
        {
            var left = new Log4netLoggerElement();
            var right = new Log4netLoggerElement();
            Assert.AreEqual(left, right);
        }
    }
}
