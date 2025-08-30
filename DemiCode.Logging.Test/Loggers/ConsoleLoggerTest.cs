using System;
using DemiCode.Logging.Loggers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DemiCode.Scheduling.Loggers.Test
{
    [TestClass]
    public class ConsoleLoggerTest
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
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for ConsoleLogger (string)
        ///</summary>
        [DeploymentItem("DemiCode.Scheduling.dll")]
        [TestMethod]
        public void ConstructorTest()
        {
            ConsoleLogger target = new ConsoleLogger();
            Assert.IsNotNull(target);
        }

        /// <summary>
        ///A test for ConsoleLogger (Type)
        ///</summary>
        [DeploymentItem("DemiCode.Scheduling.dll")]
        [TestMethod]
        public void ConstructorTest1()
        {
            ConsoleLogger target = new ConsoleLogger();
            Assert.IsNotNull(target);
        }

        [DeploymentItem("DemiCode.Scheduling.dll")]
        [TestMethod]
        public void LogTest()
        {
            Exception e = new ApplicationException("Unit testing");
            ILogger logger = new ConsoleLogger();
            
            logger.Debug("Debug {0}", 1);
            logger.Debug("Debug {0}", e, 2);

            logger.Info("Info {0}", 1);
            logger.Info("Info {0}", e, 2);

            logger.Warn("Warn {0}", 1);
            logger.Warn("Warn {0}", e, 2);

            logger.Error("Error {0}", 1);
            logger.Error("Error {0}", e, 2);

            logger.Fatal("Fatal {0}", 1);
            logger.Fatal("Fatal {0}", e, 2);

        }
    }


}
