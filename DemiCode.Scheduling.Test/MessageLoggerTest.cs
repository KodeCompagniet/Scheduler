using NUnit.Framework;

namespace DemiCode.Scheduling.Test
{
    /// <summary>
    ///This is a test class for DemiCode.Scheduling.MessageLogger and is intended
    ///to contain all DemiCode.Scheduling.MessageLogger Unit Tests
    ///</summary>
    [TestFixture]
    public class MessageLoggerTest
    {

        /// <summary>
        ///A test for LogComment (string, params object[])
        ///</summary>
        [Test]
        public void LogCommentTest()
        {
            const string format = "";
            var p = new object[0];

            MessageLogger.LogComment(format, p);
        }

        /// <summary>
        ///A test for LogError (string, params object[])
        ///</summary>
        [Test]
        public void LogErrorTest()
        {
            const string format = "";
            var p = new object[0];

            MessageLogger.LogError(format, p);
        }

        /// <summary>
        ///A test for LogInfo (string, params object[])
        ///</summary>
        [Test]
        public void LogInfoTest()
        {
            string format = "";
            object[] p = new object[0];

            MessageLogger.LogInfo(format, p);
        }

        /// <summary>
        ///A test for LogWarning (string, params object[])
        ///</summary>
        [Test]
        public void LogWarningTest()
        {
            string format = "";
            object[] p = new object[0];

            MessageLogger.LogWarning(format, p);
        }

    }


}
