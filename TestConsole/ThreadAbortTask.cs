using System;
using System.Collections;
using System.Threading;
using DemiCode.Logging;
using DemiCode.Scheduling.Tasks;

namespace TestConsole
{

    public class ThreadAbortTask : ITask
    {
        #region ITask Members

        /// <summary>
        /// Execute the task with the given parameters.
        /// </summary>
        /// <param name="loggerFactory">A logger factory which the task can use to create loggers for logging information, warnings and errors</param>
        public void Execute(ILoggerFactory loggerFactory)
        {
            if (loggerFactory == null)
                throw new ArgumentNullException(nameof(loggerFactory));

            
            ILogger logger = loggerFactory.CreateLogger(this.GetType());

            logger.Debug("Aborting thread {0}", Thread.CurrentThread.ManagedThreadId);

            Thread.CurrentThread.Abort();
        }

        #endregion
    }
}
