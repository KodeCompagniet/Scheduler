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
        /// <br/>
        /// As of version 2.0, UseShellExecute is set to false for reliable cross-platform argument handling.
        /// Shell features (pipes, redirection, environment variable expansion) are no longer supported.
        /// To use shell features, invoke the shell explicitly (e.g., cmd.exe /c or /bin/sh -c).
        /// </remarks>
        /// <param name="loggerFactory">A logger factory which the task can use to create loggers for logging information, warnings and errors</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="loggerFactory"/> is null</exception>
        public void Execute(ILoggerFactory loggerFactory)
        {
            if (loggerFactory == null)
                throw new ArgumentNullException(nameof(loggerFactory));

            var logger = loggerFactory.CreateLogger(typeof(ApplicationTask));

            using (var process = new Process())
            {
                process.StartInfo.FileName = Path;
                process.StartInfo.UseShellExecute = false;
                // Don't redirect streams - let the process handle its own output
                // Redirection without consumption would cause blocking when buffers fill

                // ArgumentList is only available in .NET Core 2.1+ and .NET 5+
                // For .NET Standard 2.0 and .NET Framework, use QuoteArguments fallback
#if NETCOREAPP2_1_OR_GREATER || NET5_0_OR_GREATER
                // Use ArgumentList for reliable cross-platform argument handling
                foreach (var arg in Arguments)
                {
                    process.StartInfo.ArgumentList.Add(arg);
                }
#else
                // Fallback to Arguments string for .NET Standard 2.0 and .NET Framework
                process.StartInfo.Arguments = QuoteArguments(Arguments);
#endif

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

        /// <summary>
        /// Quotes and escapes command-line arguments for safe shell execution.
        /// </summary>
        /// <param name="arguments">Array of argument strings to quote and escape</param>
        /// <returns>A single string with properly quoted and escaped arguments</returns>
        /// <remarks>
        /// This method is used as a fallback for frameworks that don't support ArgumentList.
        /// It handles arguments containing spaces, quotes, and backslashes according to Windows command-line parsing rules.
        /// </remarks>
        internal static string QuoteArguments(string[] arguments)
        {
            if (arguments == null || arguments.Length == 0)
                return String.Empty;

            var quotedArgs = new List<string>();
            foreach (var argument in arguments)
            {
                if (String.IsNullOrEmpty(argument))
                {
                    quotedArgs.Add("\"\"");
                    continue;
                }

                // Check if the argument needs quoting (contains space, tab, or quote)
                bool needsQuoting = argument.IndexOfAny(new[] { ' ', '\t', '"' }) >= 0;

                if (!needsQuoting)
                {
                    quotedArgs.Add(argument);
                    continue;
                }

                // Quote the argument and escape embedded quotes and backslashes
                var quoted = "\"";
                for (int i = 0; i < argument.Length; i++)
                {
                    int numBackslashes = 0;

                    // Count consecutive backslashes
                    while (i < argument.Length && argument[i] == '\\')
                    {
                        numBackslashes++;
                        i++;
                    }

                    if (i == argument.Length)
                    {
                        // Backslashes at end of argument must be doubled when quoted
                        quoted += new string('\\', numBackslashes * 2);
                        break;
                    }
                    else if (argument[i] == '"')
                    {
                        // Backslashes before a quote must be doubled, and the quote must be escaped
                        quoted += new string('\\', numBackslashes * 2 + 1);
                        quoted += '"';
                    }
                    else
                    {
                        // Backslashes before normal characters don't need escaping
                        quoted += new string('\\', numBackslashes);
                        quoted += argument[i];
                    }
                }
                quoted += "\"";
                quotedArgs.Add(quoted);
            }

            return String.Join(" ", quotedArgs);
        }
    }
}
