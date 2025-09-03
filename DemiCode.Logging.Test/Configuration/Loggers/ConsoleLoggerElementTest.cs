using DemiCode.Logging.Loggers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DemiCode.Logging.Configuration.Loggers.Test
{
    [TestClass]
    public class ConsoleLoggerElementTest
    {
        [TestMethod]
        public void ConstructorTest()
        {
            new ConsoleLoggerElement();
        }

        [TestMethod]
        public void GetLoggerTest()
        {
            ILoggerFactory factory = new ConsoleLoggerElement().GetLoggerInstance();
            factory.Should().NotBeNull();
            factory.Should().BeOfType<ConsoleLoggerFactory>();
        }

        [TestMethod]
        public void EqualsTest()
        {
            var left = new ConsoleLoggerElement();
            var right = new ConsoleLoggerElement();
            right.Should().Be(left);
        }
    }
}
