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
            factory.Should().NotBeNull();
            factory.Should().BeOfType<Log4netLoggerFactory>();
        }

        [Test]
        public void EqualsTest()
        {
            var left = new Log4netLoggerElement();
            var right = new Log4netLoggerElement();
            right.Should().Be(left);
        }
    }
}
