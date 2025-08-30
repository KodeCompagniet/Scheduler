using System;
using System.Collections;
using DemiCode.Logging;
using DemiCode.Scheduling.Tasks;

namespace TestConsole
{

    public class DivByZeroTask : ITask
    {
        #region ITask Members

        /// <summary>
        /// Execute the task with the given parameters.
        /// </summary>
        /// <param name="loggerFactory">A logger factory which the task can use to create loggers for logging information, warnings and errors</param>
        public void Execute(ILoggerFactory loggerFactory)
        {
            if (loggerFactory == null)
                throw new ArgumentNullException("loggerFactory");
            
            var i = 0;

            i = 1/i;
            
        }
        #endregion
    }
}
