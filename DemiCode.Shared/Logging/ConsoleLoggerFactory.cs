using System;

namespace DemiCode.Logging
{
    /// <summary>
    /// Factory for constructing Console loggers and NDC instances (Nested Diagnostics Context).
    /// </summary>
    public class ConsoleLoggerFactory : ILoggerFactory
    {
        #region ILoggerFactory Members

        /// <summary>
        /// Create a logger.
        /// </summary>
        /// <param name="name">Name of the logger</param>
        ILogger ILoggerFactory.CreateLogger(string name)
        {
            return new ConsoleLogger(name);
        }

        /// <summary>
        /// Create a logger.
        /// </summary>
        /// <param name="type">The full name of the type will be used as name for the logger</param>
        ILogger ILoggerFactory.CreateLogger(Type type)
        {
            return new ConsoleLogger(type.FullName);
        }

        /// <summary>
        /// Create an NDC.
        /// </summary>
        /// <param name="message">Message or context of the NDC</param>
        /// <returns>An object that serves as holder for the NDC. Must be disposed when the NDC goes out of scope. 
        /// </returns>
        IDisposable ILoggerFactory.CreateNDC(string message)
        {
            return new SimpleNDC(message);
        }

        #endregion
    }
}
