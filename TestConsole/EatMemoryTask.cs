using System;
using System.Collections.Generic;
using DemiCode.Logging;
using DemiCode.Scheduling.Tasks;

namespace TestConsole
{

    /// <summary>
    /// Task that updates a file modified date causing file watchers to think the file is modified.
    /// </summary>
    public class EatMemoryTask : ITask
    {
        public long KbToEat { get; private set; }
        public EatMemoryTask(long kbToEat)
        {
            KbToEat = kbToEat;
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

            var list = new List<byte[]>();
            for (var i = 0; i < KbToEat; i++)
            {
                var memory = new byte[1024];    
                list.Add(memory);
            }
            
        }

        #endregion
    }
}
