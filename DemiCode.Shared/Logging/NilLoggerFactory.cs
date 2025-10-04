using System;

namespace DemiCode.Logging
{
    /// <summary>
    /// Logger implementation that ignores logging data.
    /// </summary>
    public class NilLoggerFactory : ILoggerFactory
    {
        public NilLoggerFactory() { }

        #region ILoggerFactory Members

        /// <summary>
        /// Create a logger.
        /// </summary>
        /// <param name="name">Name of the logger</param>
        ILogger ILoggerFactory.CreateLogger(string name)
        {
            return new NilLogger();
        }

        /// <summary>
        /// Create a logger.
        /// </summary>
        /// <param name="type">The full name of the type will be used as name for the logger</param>
        ILogger ILoggerFactory.CreateLogger(Type type)
        {
            return new NilLogger();
        }

        /// <summary>
        /// Create a Nested Diagnostic Context.
        /// </summary>
        /// <param name="message">Message or context of the NDC</param>
        /// <returns>An object that serves as holder for the NDC. Must be disposed when the NDC goes out of scope. </returns>
        public IDisposable CreateNDC(string message)
        {
            return new SimpleNDC(message);
        }

        #endregion
    }
}
