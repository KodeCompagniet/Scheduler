using NUnit.Framework;

namespace DemiCode.Logging.Test
{
    [TestFixture]
    public class ConsoleLoggerFactoryTest
    {

        [Test]
        public void CreateTest()
        {
            ILoggerFactory factory = new ConsoleLoggerFactory();
            ILogger logger = factory.CreateLogger("test");
            Assert.IsNotNull(logger);
        }

        [Test]
        public void CreateByTypeTest()
        {
            ILoggerFactory factory = new ConsoleLoggerFactory();
            ILogger logger = factory.CreateLogger(typeof(ConsoleLoggerFactoryTest));
            Assert.IsNotNull(logger);
        }

        [Test]
        public void CreateNDCTest()
        {
            ILoggerFactory factory = new ConsoleLoggerFactory();
        
            using (factory.CreateNDC("context message"))
            {
            }
        }
    }
}