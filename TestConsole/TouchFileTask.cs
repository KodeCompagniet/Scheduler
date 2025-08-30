using System;
using System.IO;
using DemiCode.Logging;
using DemiCode.Scheduling.Tasks;

namespace TestConsole
{

    /// <summary>
    /// Task that updates a file modified date causing file watchers to think the file is modified.
    /// </summary>
    public class TouchFileTask : ITask
    {
        public string Filename { get; private set; }

        public TouchFileTask(string filename)
        {
            Filename = filename;
        }

        #region ITask Members

        /// <summary>
        /// Execute the task with the given parameters.
        /// </summary>
        /// <param name="loggerFactory">A logger factory which the task can use to create loggers for logging information, warnings and errors</param>
        public void Execute(ILoggerFactory loggerFactory)
        {
            if (loggerFactory == null)
                throw new ArgumentNullException("loggerFactory");

            if (String.IsNullOrEmpty(Filename))
                throw new InvalidOperationException("Configuration must provide the 'filename' attribute");

            if (!File.Exists(Filename))
                throw new InvalidOperationException("The 'file' attribute specifies a non-existing file");
            
            File.SetLastWriteTime(Filename, DateTime.Now);
        }

        #endregion
    }
}
