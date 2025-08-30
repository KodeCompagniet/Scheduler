using System;
using System.Collections.Generic;
using System.Text;

namespace DemiCode.Logging
{
    /// <summary>
    /// Logger implementation that ignores logging data.
    /// </summary>
    public class NilLogger : ILogger
    {
        #region ILogger Members

        /// <summary>
        /// Log a fatal message.
        /// </summary>
        /// <param name="message">Message with optional format parameters</param>
        /// <param name="args">Optional format arguments</param>
        void ILogger.Fatal(string message, params object[] args)
        {
        }

        /// <summary>
        /// Log a fatal message with exception information.
        /// </summary>
        /// <param name="message">Message with optional format parameters</param>
        /// <param name="exception">Exception</param>
        /// <param name="args">Optional format arguments</param>
        void ILogger.Fatal(string message, Exception exception, params object[] args)
        {
        }

        /// <summary>
        /// Log an error message.
        /// </summary>
        /// <param name="message">Message with optional format parameters</param>
        /// <param name="args">Optional format arguments</param>
        void ILogger.Error(string message, params object[] args)
        {
        }

        /// <summary>
        /// Log an error message with exception information.
        /// </summary>
        /// <param name="message">Message with optional format parameters</param>
        /// <param name="exception">Exception</param>
        /// <param name="args">Optional format arguments</param>
        void ILogger.Error(string message, Exception exception, params object[] args)
        {
        }

        /// <summary>
        /// Log a warning.
        /// </summary>
        /// <param name="message">Message with optional format parameters</param>
        /// <param name="args">Optional format arguments</param>
        void ILogger.Warn(string message, params object[] args)
        {
        }

        /// <summary>
        /// Log a warning with exception information.
        /// </summary>
        /// <param name="message">Message with optional format parameters</param>
        /// <param name="exception">Exception</param>
        /// <param name="args">Optional format arguments</param>
        void ILogger.Warn(string message, Exception exception, params object[] args)
        {
        }

        /// <summary>
        /// Log information.
        /// </summary>
        /// <param name="message">Message with optional format parameters</param>
        /// <param name="args">Optional format arguments</param>
        void ILogger.Info(string message, params object[] args)
        {
        }

        /// <summary>
        /// Log information with exception information.
        /// </summary>
        /// <param name="message">Message with optional format parameters</param>
        /// <param name="exception">Exception</param>
        /// <param name="args">Optional format arguments</param>
        void ILogger.Info(string message, Exception exception, params object[] args)
        {
        }

        /// <summary>
        /// Log debug information.
        /// </summary>
        /// <param name="message">Message with optional format parameters</param>
        /// <param name="args">Optional format arguments</param>
        void ILogger.Debug(string message, params object[] args)
        {
        }

        /// <summary>
        /// Log debug information with exception information.
        /// </summary>
        /// <param name="message">Message with optional format parameters</param>
        /// <param name="exception">Exception</param>
        /// <param name="args">Optional format arguments</param>
        void ILogger.Debug(string message, Exception exception, params object[] args)
        {
        }

        #endregion
    }
}
