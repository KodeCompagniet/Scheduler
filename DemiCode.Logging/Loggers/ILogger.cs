using System;

namespace DemiCode.Logging.Loggers
{
    /// <summary>
    /// Interface for a logger.
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Log a fatal message.
        /// </summary>
        /// <param name="message">Message with optional format parameters</param>
        /// <param name="args">Optional format arguments</param>
        void Fatal(string message, params object[] args);

        /// <summary>
        /// Log a fatal message with exception information.
        /// </summary>
        /// <param name="message">Message with optional format parameters</param>
        /// <param name="exception">Exception</param>
        /// <param name="args">Optional format arguments</param>
        void Fatal(string message, Exception exception, params object[] args);

        /// <summary>
        /// Log an error message.
        /// </summary>
        /// <param name="message">Message with optional format parameters</param>
        /// <param name="args">Optional format arguments</param>
        void Error(string message, params object[] args);

        /// <summary>
        /// Log an error message with exception information.
        /// </summary>
        /// <param name="message">Message with optional format parameters</param>
        /// <param name="exception">Exception</param>
        /// <param name="args">Optional format arguments</param>
        void Error(string message, Exception exception, params object[] args);

        /// <summary>
        /// Log a warning.
        /// </summary>
        /// <param name="message">Message with optional format parameters</param>
        /// <param name="args">Optional format arguments</param>
        void Warn(string message, params object[] args);

        /// <summary>
        /// Log a warning with exception information.
        /// </summary>
        /// <param name="message">Message with optional format parameters</param>
        /// <param name="exception">Exception</param>
        /// <param name="args">Optional format arguments</param>
        void Warn(string message, Exception exception, params object[] args);

        /// <summary>
        /// Log information.
        /// </summary>
        /// <param name="message">Message with optional format parameters</param>
        /// <param name="args">Optional format arguments</param>
        void Info(string message, params object[] args);

        /// <summary>
        /// Log information with exception information.
        /// </summary>
        /// <param name="message">Message with optional format parameters</param>
        /// <param name="exception">Exception</param>
        /// <param name="args">Optional format arguments</param>
        void Info(string message, Exception exception, params object[] args);

        /// <summary>
        /// Log debug information.
        /// </summary>
        /// <param name="message">Message with optional format parameters</param>
        /// <param name="args">Optional format arguments</param>
        void Debug(string message, params object[] args);

        /// <summary>
        /// Log debug information with exception information.
        /// </summary>
        /// <param name="message">Message with optional format parameters</param>
        /// <param name="exception">Exception</param>
        /// <param name="args">Optional format arguments</param>
        void Debug(string message, Exception exception, params object[] args);
    }
}
