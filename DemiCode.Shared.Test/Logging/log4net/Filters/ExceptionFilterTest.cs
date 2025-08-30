#if INCLUDE_LOGGING_EXTENSIONS

using System;
using log4net;
using log4net.Config;
using DemiCode.Shared.Test;
using NUnit.Framework;

namespace DemiCode.Logging.log4net.Filters.Test
{
    [TestFixture]
    public class ExceptionFilterTest
    {
        private const string InclusiveFilterTestAppender = "InclusiveFilterTestAppender";
        private const string ExclusiveFilterTestAppender = "ExclusiveFilterTestAppender";
        private const string ExceptionMessageAppender = "ExceptionMessageAppender";

        [SetUp]
        public void TestInitialize()
        {
            FileHelper.ReplaceConfigFile(".\\TestData\\ExceptionFilterTest.config");
            System.Configuration.ConfigurationManager.RefreshSection("log4net");
            XmlConfigurator.Configure();
            AssertLog.ClearLog(InclusiveFilterTestAppender);
            AssertLog.ClearLog(ExclusiveFilterTestAppender);
            AssertLog.ClearLog(ExceptionMessageAppender);
        }

        [TearDown]
        public void TestCleanup()
        {
            FileHelper.RestoreConfigFile();
        }

        [Test]
        public void ConstructorTest()
        {
            var target = new ExceptionFilter();
            Assert.IsNotNull(target);
        }

        [Test]
        public void LogExceptionTest()
        {
            var log = LogManager.GetLogger(typeof(ExceptionFilterTest));
            var exception = GenerateExceptionWithSomeStackTrace("MessageInTheException", 10);

            log.Error("Unhandled exception occured", exception);

            AssertLog.ErrorCountIsEqual(InclusiveFilterTestAppender, 1);
            AssertLog.ErrorCountIsEqual(ExclusiveFilterTestAppender, 0);
            AssertLog.ErrorCountIsEqual(ExceptionMessageAppender, 1);
        }

        private static Exception GenerateExceptionWithSomeStackTrace(string message, int levels)
        {
            if (levels == 0)
            {
                try
                {
                    throw new ApplicationException(message);
                }
                catch(Exception ex)
                {
                    return ex;
                }
            }

            return GenerateExceptionWithSomeStackTrace(message, --levels);
        }

    }
}

#endif