using DemiCode.Logging;
using DemiCode.Logging.Configuration;
using DemiCode.Logging.log4net;

namespace DemiCode.Logging.log4net.Configuration
{
    /// <summary>
    /// Strong typed class for containing a Log4netLogger configuration element read from a configuration file.
    /// </summary>
    /// <remarks>This class is intended to be used by the <see cref="LoggerElementCollection{V}"/> class</remarks>
    public class Log4netLoggerElement : LoggerElementBase
    {
        /// <summary>
        /// Construct a new <see cref="Log4netLoggerElement"/> object.
        /// </summary>
        public Log4netLoggerElement()
            : base("Log4netLogger")
        {
        }

        ///<summary>
        /// Return an <see cref="ILoggerFactory"/> instance.
        ///</summary>
        /// <returns>An instance of <see cref="Log4netLoggerFactory"/>.</returns>
        public override ILoggerFactory GetLoggerInstance()
        {
            return new Log4netLoggerFactory();
        }
    }
}
