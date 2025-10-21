using System.Collections;
using DemiCode.Logging;
using DemiCode.Scheduling.Tasks;

namespace DemiCode.Scheduling.Test.Dummy
{
    /// <summary>
    /// Dummy class used in testing runtime config changes in Scheduler
    /// </summary>
    public class TestDummy : ITask
    {
        public static void SimpleMethod()
        {
        }

        #region ITask Members

        /// <summary>
        /// Execute the task with the given parameters.
        /// </summary>
        /// <param name="loggerFactory">A logger factory which the task can use to create loggers for logging information, warnings and errors</param>
        void ITask.Execute(ILoggerFactory loggerFactory)
        {
            loggerFactory.CreateLogger(typeof(TestDummy)).Debug("TestDummy called!");
        }

        #endregion
    }
}
