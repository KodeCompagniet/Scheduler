using System;
using NUnit.Framework;
using log4net;
using System.IO;

namespace DemiCode.Logging.log4net.Test
{
    /// <summary>
    /// FeedAppenderTest
    /// </summary>
    [TestFixture]
    public class FeedAppenderTest
    {

        [SetUp]
        public void MyTestInitialize()
        {
            string dir = Directory.GetCurrentDirectory();
            foreach (var item in Directory.GetFiles(dir, "Statusfeed*.xml"))
            {
                File.Delete(item);
            }
        }

        [Test]
        public void LogToFeedAppenderTest()
        {
            ILog log = LogManager.GetLogger(typeof(FeedAppenderTest));
            log.Error("This message should go to our status feed appender", new ApplicationException("This is an exception"));

            Assert.AreEqual(1, Directory.GetFiles(Directory.GetCurrentDirectory(), "Statusfeed*.xml").Length);
        }
    }
}
