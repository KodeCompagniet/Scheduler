using NUnit.Framework;

namespace DemiCode.Logging.Test
{
    [TestFixture]
    public class NilLoggerFactoryTest
    {

        [Test]
        public void CreateTest()
        {
            ILoggerFactory factory = new NilLoggerFactory();
            ILogger logger = factory.CreateLogger("test");
            Assert.IsNotNull(logger);
        }

        [Test]
        public void CreateByTypeTest()
        {
            ILoggerFactory factory = new NilLoggerFactory();
            ILogger logger = factory.CreateLogger(typeof(NilLoggerFactoryTest));
            Assert.IsNotNull(logger);
        }

        [Test]
        public void CreateNDCTest()
        {
            ILoggerFactory factory = new NilLoggerFactory();
        
            using (factory.CreateNDC("context message"))
            {
            }
        }
    }
}