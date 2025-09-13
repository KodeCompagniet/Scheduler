using System;
using DemiCode.Logging.Loggers;
using log4net;

namespace DemiCode.Logging.log4net
{
    /// <summary>
    /// Factory for constructing log4net compatible loggers and NDC instances (Nested Diagnostics Context).
    /// </summary>
    public class Log4netLoggerFactory : ILoggerFactory
    {
        #region ILoggerFactory Members

        ILogger ILoggerFactory.CreateLogger(string name)
        {
            return new Log4netLogger(name);
        }

        ILogger ILoggerFactory.CreateLogger(Type type)
        {
            return new Log4netLogger(type);
        }

        /// <summary>
        /// Create a Nested Diagnostic Context.
        /// </summary>
        /// <param name="message">Message or context of the NDC</param>
        /// <returns>An object that serves as holder for the NDC. Must be disposed when the NDC goes out of scope. </returns>
        public IDisposable CreateNDC(string message)
        {
            return NDC.Push(message);
        }

        #endregion
    }
}