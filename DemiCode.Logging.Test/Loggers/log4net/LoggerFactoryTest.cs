using DemiCode.Logging;  // Use ILogger and ILoggerFactory from DemiCode.Shared
using DemiCode.Logging.log4net;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DemiCode.Scheduling.Loggers.log4net.Test
{
    [TestClass]
    public class LoggerFactoryTest
    {
        public TestContext TestContext { get; set; }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //

        //
        //Use TestCleanup to run code after each test has run
        //
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        [TestMethod]
        public void CreateTest()
        {
            ILoggerFactory factory = (ILoggerFactory)new Log4netLoggerFactory();
            ILogger logger = factory.CreateLogger("test");
            Assert.IsNotNull(logger);
        }

        [TestMethod]
        public void CreateByTypeTest()
        {
            ILoggerFactory factory = (ILoggerFactory)new Log4netLoggerFactory();
            ILogger logger = factory.CreateLogger(typeof(LoggerFactoryTest));
            Assert.IsNotNull(logger);
        }

        [TestMethod]
        public void CreateNDCTest()
        {
            ILoggerFactory factory = (ILoggerFactory)new Log4netLoggerFactory();

            using (factory.CreateNDC("context message"))
            {
            }
        }
    }
}