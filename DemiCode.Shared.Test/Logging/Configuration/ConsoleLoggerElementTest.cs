using DemiCode.Logging;
using NUnit.Framework;

namespace DemiCode.Logging.Configuration.Test
{
    [TestFixture]
    public class ConsoleLoggerElementTest
    {
        [Test]
        public void ConstructorTest()
        {
            new ConsoleLoggerElement();
        }

        [Test]
        public void GetLoggerTest()
        {
            ILoggerFactory factory = new ConsoleLoggerElement().GetLoggerInstance();
            factory.Should().NotBeNull();
            factory.Should().BeOfType<ConsoleLoggerFactory>();
        }

        [Test]
        public void EqualsTest()
        {
            var left = new ConsoleLoggerElement();
            var right = new ConsoleLoggerElement();
            right.Should().Be(left);
        }
    }
}
