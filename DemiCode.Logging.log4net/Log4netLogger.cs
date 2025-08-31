using System;
using log4net;

namespace DemiCode.Logging.log4net
{
    /// <summary>
    /// Logger supporting the log4net logging framework.
    /// </summary>
    public class Log4netLogger : ILogger
    {
        private ILog _log;

        internal Log4netLogger(string name)
        {
            _log = LogManager.GetLogger(name);
        }

        internal Log4netLogger(Type type)
        {
            _log = LogManager.GetLogger(type);
        }

        #region ILogger Members

        void ILogger.Fatal(string message, params object[] args)
        {
            _log.FatalFormat(message, args);
        }

        /// <summary>
        /// Log a fatal message with exception information.
        /// </summary>
        /// <param name="message">Message with optional format parameters</param>
        /// <param name="exception">Exception</param>
        /// <param name="args">Optional format arguments</param>
        void ILogger.Fatal(string message, Exception exception, params object[] args)
        {
            _log.Fatal(String.Format(message, args), exception);
        }

        void ILogger.Error(string message, params object[] args)
        {
            _log.ErrorFormat(message, args);
        }

        /// <summary>
        /// Log an error message with exception information.
        /// </summary>
        /// <param name="message">Message with optional format parameters</param>
        /// <param name="exception">Exception</param>
        /// <param name="args">Optional format arguments</param>
        void ILogger.Error(string message, Exception exception, params object[] args)
        {
            _log.Error(String.Format(message, args), exception);
        }

        void ILogger.Warn(string message, params object[] args)
        {
            _log.WarnFormat(message, args);
        }

        /// <summary>
        /// Log a warning with exception information.
        /// </summary>
        /// <param name="message">Message with optional format parameters</param>
        /// <param name="exception">Exception</param>
        /// <param name="args">Optional format arguments</param>
        void ILogger.Warn(string message, Exception exception, params object[] args)
        {
            _log.Warn(String.Format(message, args), exception);
        }

        void ILogger.Info(string message, params object[] args)
        {
            _log.InfoFormat(message, args);
        }

        /// <summary>
        /// Log information with exception information.
        /// </summary>
        /// <param name="message">Message with optional format parameters</param>
        /// <param name="exception">Exception</param>
        /// <param name="args">Optional format arguments</param>
        void ILogger.Info(string message, Exception exception, params object[] args)
        {
            _log.Info(String.Format(message, args), exception);
        }

        void ILogger.Debug(string message, params object[] args)
        {
            _log.DebugFormat(message, args);
        }

        /// <summary>
        /// Log debug information with exception information.
        /// </summary>
        /// <param name="message">Message with optional format parameters</param>
        /// <param name="exception">Exception</param>
        /// <param name="args">Optional format arguments</param>
        void ILogger.Debug(string message, Exception exception, params object[] args)
        {
            _log.Debug(String.Format(message, args), exception);
        }

        #endregion
    }
}
