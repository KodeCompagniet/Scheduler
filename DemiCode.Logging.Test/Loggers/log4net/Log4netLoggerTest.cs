using System;
using DemiCode.Logging;  // Use ILogger from DemiCode.Shared
using DemiCode.Logging.log4net;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DemiCode.Scheduling.Loggers.log4net.Test
{
    /// <summary>
    ///This is a test class for DemiCode.Scheduling.Loggers.log4net.Log4netLogger and is intended
    ///to contain all DemiCode.Scheduling.Loggers.log4net.Log4netLogger Unit Tests
    ///</summary>
    [TestClass]
    public class Log4netLoggerTest
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
        ///A test for Log4netLogger (string)
        ///</summary>
        [DeploymentItem("DemiCode.Scheduling.Loggers.log4net.dll")]
        [TestMethod]
        public void ConstructorTest()
        {
            string name = "test"; 
            Log4netLogger target = new Log4netLogger(name);
            Assert.IsNotNull(target);
        }

        /// <summary>
        ///A test for Log4netLogger (Type)
        ///</summary>
        [DeploymentItem("DemiCode.Scheduling.Loggers.log4net.dll")]
        [TestMethod]
        public void ConstructorTest1()
        {
            Type type = typeof (Log4netLoggerTest);
            Log4netLogger target = new Log4netLogger(type);
            Assert.IsNotNull(target);
        }

        [TestMethod]
        public void LogTest()
        {
            Exception e = new ApplicationException("Unit testing");
            // NOTE: Cast required but fails at runtime - possible assembly version mismatch
            // See TESTDATA_INVESTIGATION_RESULTS.md for details
            ILogger logger = (ILogger)new Log4netLogger("testlogger");
            
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
