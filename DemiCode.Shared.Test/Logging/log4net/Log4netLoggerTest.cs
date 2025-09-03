using System;
using DemiCode.Logging;
using DemiCode.Logging.log4net;
using NUnit.Framework;

namespace DemiCode.Loggers.log4net.Test
{
    [TestFixture]
    public class Log4netLoggerTest
    {

        /// <summary>
        ///A test for Log4netLogger (string)
        ///</summary>
        [Test]
        public void ConstructorTest()
        {
            var name = "test"; 
            var target = new Log4netLogger(name);
            target.Should().NotBeNull();
        }

        /// <summary>
        ///A test for Log4netLogger (Type)
        ///</summary>
        [Test]
        public void ConstructorTest1()
        {
            var type = typeof (Log4netLoggerTest);
            var target = new Log4netLogger(type);
            target.Should().NotBeNull();
        }

        [Test]
        public void LogTest()
        {
            Exception e = new ApplicationException("Unit testing");
            ILogger logger = new Log4netLogger("testlogger");
            
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
