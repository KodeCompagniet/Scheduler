using System;
using NUnit.Framework;

namespace DemiCode.Logging.Test
{
    [TestFixture]
    public class NilLoggerTest
    {

        /// <summary>
        ///A test for NilLogger (string)
        ///</summary>
        [Test]
        public void ConstructorTest()
        {
            NilLogger target = new NilLogger();
            Assert.IsNotNull(target);
        }

        /// <summary>
        ///A test for NilLogger (Type)
        ///</summary>
        [Test]
        public void ConstructorTest1()
        {
            NilLogger target = new NilLogger();
            Assert.IsNotNull(target);
        }

        [Test]
        public void LogTest()
        {
            Exception e = new ApplicationException("Unit testing");
            ILogger logger = new NilLogger();

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
