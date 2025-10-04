using DemiCode.Logging.Loggers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DemiCode.Scheduling.Loggers.Test
{
    [TestClass]
    public class NilLoggerFactoryTest
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
            ILoggerFactory factory = new NilLoggerFactory();
            ILogger logger = factory.CreateLogger("test");
            Assert.IsNotNull(logger);
        }

        [TestMethod]
        public void CreateByTypeTest()
        {
            ILoggerFactory factory = new NilLoggerFactory();
            ILogger logger = factory.CreateLogger(typeof(NilLoggerFactoryTest));
            Assert.IsNotNull(logger);
        }

        [TestMethod]
        public void CreateNDCTest()
        {
            ILoggerFactory factory = new NilLoggerFactory();

            using (factory.CreateNDC("context message"))
            {
            }
        }
    }
}