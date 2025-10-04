using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using DemiCode.Logging;

namespace DemiCode.Scheduling.Tasks
{
    /// <summary>
    /// The ApplicationTask executes an application. 
    /// </summary>
    public class ApplicationTask : ITask
    {
        ///<summary>
        /// The path can be the name of any document (which is any registered file type associated with an executable that has a default open action) 
        /// or the path to an executable file.
        /// </summary>
        public string Path { get; private set; }

        /// <summary>
        /// The strings in <see cref="Arguments"/> will be passed to the application as command-line arguments.
        /// </summary>
        public string[] Arguments { get; private set; }

        /// <summary>
        /// Construct an <see cref="ApplicationTask"/> instance.
        /// </summary>
        /// <param name="path">The path can be the name of any document (which is any registered file type associated with an executable that has a default open action) 
        /// or the path to an executable file.</param>
        /// <param name="arguments">The strings in <paramref name="arguments"/> will be passed to the application as command-line arguments.</param>
        public ApplicationTask(string path, params string[] arguments)
        {
            if (String.IsNullOrEmpty(path))
                throw new ArgumentException("Path cannot be an empty string", "path");

            if (arguments == null)
                throw new ArgumentNullException("arguments");

            Path = path;
            Arguments = arguments;
        }

        /// <summary>
        /// Execute the application specified in <see cref="Path"/>.
        /// </summary>
        /// <remarks>
        /// The path can be the name of any document (which is any registered file type associated with an executable that has a default open action) 
        /// or the path to an executable file.
        /// The strings in <see cref="Arguments"/> will be passed to the application as command-line arguments.
        /// <br/>
        /// <see cref="Execute(ILoggerFactory)"/> will return immediately after the application have started and will not wait for the process to stop.
        /// </remarks>
        /// <param name="loggerFactory">A logger factory which the task can use to create loggers for logging information, warnings and errors</param>
        public void Execute(ILoggerFactory loggerFactory)
        {
            var logger = loggerFactory.CreateLogger(typeof(ApplicationTask));

            using (var process = new Process())
            {
                process.StartInfo.Arguments = ArgumentsToString(Arguments);
                process.StartInfo.UseShellExecute = true;
                process.StartInfo.FileName = Path;

                try
                {
                    process.Start();
                }
                catch (Exception e)
                {
                    logger.Error("Failed to start process {0}", e, Path);
                }
            }
        }

        internal static string ArgumentsToString(string[] arguments)
        {
            var args = String.Empty;
            foreach (var argument in arguments)
            {
                if (!(argument.StartsWith("\"") && argument.EndsWith("\"")))
                {
                    args += "\"" + argument + "\" ";
                }
                else
                {
                    args += argument + " ";
                }
            }
            return args;
        }
    }
}
