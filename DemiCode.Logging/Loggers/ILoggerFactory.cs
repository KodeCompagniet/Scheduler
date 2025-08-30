using System;

namespace DemiCode.Logging.Loggers
{
    /// <summary>
    /// A logger factory is used to create loggers and NDCs (Nested Diagnostics Context).
    /// </summary>
    public interface ILoggerFactory
    {
        /// <summary>
        /// Create a logger.
        /// </summary>
        /// <param name="name">Name of the logger</param>
        /// <returns>The logger instance.</returns>
        ILogger CreateLogger(string name);

        /// <summary>
        /// Create a logger.
        /// </summary>
        /// <param name="type">The full name of the type will be used as name for the logger</param>
        /// <returns>The logger instance.</returns>
        ILogger CreateLogger(Type type);

        /// <summary>
        /// Create a Nested Diagnostic Context.
        /// </summary>
        /// <param name="message">Message or context of the NDC</param>
        /// <returns>An object that serves as holder for the NDC. Must be disposed when the NDC goes out of scope. </returns>
        IDisposable CreateNDC(string message);
    }
}
