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
            Assert.IsNotNull(factory);
            Assert.IsInstanceOf<ConsoleLoggerFactory>(factory);
        }

        [Test]
        public void EqualsTest()
        {
            var left = new ConsoleLoggerElement();
            var right = new ConsoleLoggerElement();
            Assert.AreEqual(left, right);
        }
    }
}
