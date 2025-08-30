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
            Assert.IsNotNull(factory);
            Assert.IsInstanceOfType(factory, typeof(ConsoleLoggerFactory));
        }

        [TestMethod]
        public void EqualsTest()
        {
            var left = new ConsoleLoggerElement();
            var right = new ConsoleLoggerElement();
            Assert.AreEqual(left, right);
        }
    }
}
