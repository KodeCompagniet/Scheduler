using System;
using DemiCode.Logging.Loggers;

namespace DemiCode.Logging.Test.Mocks
{
    public class LoggerMock : ILogger
    {
        #region ILogger Members

        public IDisposable SetNDC(string message)
        {
            return new SimpleNDC(message);
        }

        public void Fatal(string message, params object[] args)
        {
        }

        /// <summary>
        /// Log a fatal message with exception information.
        /// </summary>
        /// <param name="message">Message with optional format parameters</param>
        /// <param name="exception">Exception</param>
        /// <param name="args">Optional format arguments</param>
        public void Fatal(string message, Exception exception, params object[] args)
        {
        }

        public void Error(string message, params object[] args)
        {
        }

        /// <summary>
        /// Log an error message with exception information.
        /// </summary>
        /// <param name="message">Message with optional format parameters</param>
        /// <param name="exception">Exception</param>
        /// <param name="args">Optional format arguments</param>
        public void Error(string message, Exception exception, params object[] args)
        {
        }

        public void Warn(string message, params object[] args)
        {
        }

        /// <summary>
        /// Log a warning with exception information.
        /// </summary>
        /// <param name="message">Message with optional format parameters</param>
        /// <param name="exception">Exception</param>
        /// <param name="args">Optional format arguments</param>
        public void Warn(string message, Exception exception, params object[] args)
        {
        }

        public void Info(string message, params object[] args)
        {
        }

        /// <summary>
        /// Log information with exception information.
        /// </summary>
        /// <param name="message">Message with optional format parameters</param>
        /// <param name="exception">Exception</param>
        /// <param name="args">Optional format arguments</param>
        public void Info(string message, Exception exception, params object[] args)
        {
        }

        public void Debug(string message, params object[] args)
        {
        }

        /// <summary>
        /// Log debug information with exception information.
        /// </summary>
        /// <param name="message">Message with optional format parameters</param>
        /// <param name="exception">Exception</param>
        /// <param name="args">Optional format arguments</param>
        public void Debug(string message, Exception exception, params object[] args)
        {
        }

        #endregion
    }
}