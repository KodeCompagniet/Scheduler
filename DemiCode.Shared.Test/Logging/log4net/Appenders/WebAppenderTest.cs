#if INCLUDE_LOGGING_EXTENSIONS

using System;
using System.IO;
using log4net;
using log4net.Config;
using DemiCode.Shared.Test;
using NUnit.Framework;

namespace DemiCode.Logging.log4net.Appenders.Test
{
    [TestFixture]
    public class WebAppenderTest
    {
        private const string MemoryAppenderName = "WebResultAppender";

        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            XmlConfigurator.Configure(new Uri(Path.GetFullPath(".\\TestData\\WebAppenderTest.config")));
            AssertLog.ClearLog(MemoryAppenderName);
        }

        [TearDown]
        public void TearDown()
        {
            LogManager.ResetConfiguration();
        }

        #endregion


        [Test]
        public void ConstructorTest()
        {
            var target = new WebAppender();
            target.Should().NotBeNull();
        }

        [Test]
        public void Info()
        {
            var log = LogManager.GetLogger(typeof (WebAppenderTest));
            log.Info("This information should get through to FogBugz");
            
            AssertLog.InfoCountIsEqual(MemoryAppenderName, 1);
            AssertFogBugzSuccess();
        }

        [Test]
        public void Warn()
        {
            var log = LogManager.GetLogger(typeof(WebAppenderTest));
            log.Warn("This message should get through to FogBugz");

            AssertLog.WarnCountIsEqual(MemoryAppenderName, 1);
            AssertFogBugzSuccess();
        }

        [Test]
        public void Error()
        {
            var log = LogManager.GetLogger(typeof(WebAppenderTest));
            Exception exception = null;
            try
            {
                GenerateExceptionWithSomeStackTrace("This is the exception object", 10);
                Assert.Fail("Failed to generate exception");
            }
            catch(Exception ex)
            {
                exception = ex;
            }

            log.Error("Unhandled exception occured", exception);

            AssertLog.ErrorCountIsEqual(MemoryAppenderName, 1);
            AssertFogBugzSuccess();
        }

        private static void GenerateExceptionWithSomeStackTrace(string message, int levels)
        {
            if (levels == 0)
                throw new ApplicationException(message);

            GenerateExceptionWithSomeStackTrace(message, --levels);
        }

        /// <summary>
        /// Assert that the latest response to FogBugz BugScout reported success
        /// </summary>
        private static void AssertFogBugzSuccess()
        {
            var events = AssertLog.GetMemoryAppender(MemoryAppenderName).GetEvents();
            var loggingEvent = events[0];
            var result = loggingEvent.Properties["webappender:Result"] as string;
            result.Should().NotBeNull();
            result.Should().Be("<?xml version=\"1.0\" ?><Success>Thanks for your submission</Success>");
        }
    }
}

#endif