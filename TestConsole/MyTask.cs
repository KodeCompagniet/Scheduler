using System;
using DemiCode.Logging;
using DemiCode.Scheduling.Tasks;

namespace TestConsole
{
    public class MyTask : ITask
    {
        public string Message { get; private set; }

        public MyTask(string message)
        {
            Message = message;
        }

        #region ITask Members

        /// <summary>
        /// Execute the task with the given parameters.
        /// </summary>
        /// <param name="loggerFactory">A logger factory which the task can use to create loggers for logging information, warnings and errors</param>
        public void Execute(ILoggerFactory loggerFactory)
        {
            if (loggerFactory == null)
                throw new ArgumentNullException(nameof(loggerFactory));

            ILogger logger = loggerFactory.CreateLogger(GetType());
            if (String.IsNullOrEmpty(Message))
            {
                logger.Info("Event called at {0:yyyy-MM-dd HH:mm:ss:ff}", DateTime.Now);
            }
            else
            {
                logger.Info("Event called at {0:yyyy-MM-dd HH:mm:ss:ff} {1}", DateTime.Now, Message);
            }
        }

        #endregion
    }
}
