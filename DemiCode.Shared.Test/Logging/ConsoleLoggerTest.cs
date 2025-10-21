using System;
using NUnit.Framework;

namespace DemiCode.Logging.Test
{
    [TestFixture]
    public class ConsoleLoggerTest
    {

        /// <summary>
        ///A test for ConsoleLogger (string)
        ///</summary>
        [Test]
        public void ConstructorTest()
        {
            ConsoleLogger target = new ConsoleLogger();
            Assert.IsNotNull(target);
        }

        /// <summary>
        ///A test for ConsoleLogger (Type)
        ///</summary>
        [Test]
        public void ConstructorTest1()
        {
            ConsoleLogger target = new ConsoleLogger();
            Assert.IsNotNull(target);
        }

        [Test]
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
