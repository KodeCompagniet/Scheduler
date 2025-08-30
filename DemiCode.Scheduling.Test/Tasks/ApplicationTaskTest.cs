using DemiCode.Logging;
using DemiCode.Scheduling.Test.Mocks;
using NUnit.Framework;

namespace DemiCode.Scheduling.Tasks.Test
{
    /// <summary>
    ///This is a test class for DemiCode.Scheduling.Tasks.ApplicationTask and is intended
    ///to contain all DemiCode.Scheduling.Tasks.ApplicationTask Unit Tests
    ///</summary>
    [TestFixture]
    public class ApplicationTaskTest
    {

        /// <summary>
        ///A test for Execute (IDictionary)
        ///</summary>
        [Test]
        public void ExecuteTest()
        {
            var task = new ApplicationTask("cmd", new[]{"/C \"echo fisk\""});
            ILoggerFactory loggerFactory = new LoggerFactoryMock();

            task.Execute(loggerFactory);
        }

    }


}
