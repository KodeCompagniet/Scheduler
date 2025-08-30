using System;

namespace DemiCode.Logging
{
    /// <summary>
    /// A simple logger that outputs messages to the console. This logger is usefull in console applications and debugging scenarios. 
    /// </summary>
    public class ConsoleLogger : ILogger
    {
        private readonly string _loggerName;
        private readonly string _loggerNameMsg;
        
        /// <summary>
        /// Initialize a new instance of <see cref="ConsoleLogger"/>.
        /// </summary>
        public ConsoleLogger() : this(null) {}

        /// <summary>
        /// Initialize a new instance of <see cref="ConsoleLogger"/>.
        /// </summary>
        /// <param name="loggerName">The name of this logger.</param>
        public ConsoleLogger(string loggerName)
        {
            _loggerName = loggerName;
            if (String.IsNullOrEmpty(_loggerName))
            {
                _loggerNameMsg = String.Empty;
            }
            else
            {
                _loggerNameMsg = _loggerName + ": ";
            }
        }

        #region ILogger Members

        void ILogger.Fatal(string message, params object[] args)
        {
            Console.BackgroundColor = ConsoleColor.Red;
            WriteMessage(message, args);
        }

        /// <summary>
        /// Log a fatal message with exception information.
        /// </summary>
        /// <param name="message">Message with optional format parameters</param>
        /// <param name="exception">Exception</param>
        /// <param name="args">Optional format arguments</param>
        void ILogger.Fatal(string message, Exception exception, params object[] args)
        {
            WriteMessage(message, args);
            WriteException(exception);
        }

        void ILogger.Error(string message, params object[] args)
        {
            Console.BackgroundColor = ConsoleColor.Red;
            WriteMessage(message, args);
        }

        /// <summary>
        /// Log an error message with exception information.
        /// </summary>
        /// <param name="message">Message with optional format parameters</param>
        /// <param name="exception">Exception</param>
        /// <param name="args">Optional format arguments</param>
        void ILogger.Error(string message, Exception exception, params object[] args)
        {
            WriteMessage(message, args);
            WriteException(exception);
        }

        void ILogger.Warn(string message, params object[] args)
        {
            WriteMessage(message, args);
        }

        /// <summary>
        /// Log a warning with exception information.
        /// </summary>
        /// <param name="message">Message with optional format parameters</param>
        /// <param name="exception">Exception</param>
        /// <param name="args">Optional format arguments</param>
        void ILogger.Warn(string message, Exception exception, params object[] args)
        {
            WriteMessage(message, args);
            WriteException(exception);
        }

        void ILogger.Info(string message, params object[] args)
        {
            WriteMessage(message, args);
        }

        /// <summary>
        /// Log information with exception information.
        /// </summary>
        /// <param name="message">Message with optional format parameters</param>
        /// <param name="exception">Exception</param>
        /// <param name="args">Optional format arguments</param>
        void ILogger.Info(string message, Exception exception, params object[] args)
        {
            WriteMessage(message, args);
            WriteException(exception);
        }

        void ILogger.Debug(string message, params object[] args)
        {
            WriteMessage(message, args);
        }

        /// <summary>
        /// Log debug information with exception information.
        /// </summary>
        /// <param name="message">Message with optional format parameters</param>
        /// <param name="exception">Exception</param>
        /// <param name="args">Optional format arguments</param>
        void ILogger.Debug(string message, Exception exception, params object[] args)
        {
            WriteMessage(message, args);
            WriteException(exception);
        }

        #endregion

        private void WriteMessage(string message, params object [] args)
        {
            string msg = String.Format(_loggerNameMsg + message, args);
            Console.WriteLine(msg);
        }

        private static void WriteException(Exception e)
        {
            if (e != null)
            {
                Console.WriteLine("{0}: {1}", e.GetType().Name, e.Message);
                Console.WriteLine(e.StackTrace);
            }
        }
    }
}
