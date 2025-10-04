using System;
using System.Configuration;
using DemiCode.Logging.Loggers;

namespace DemiCode.Logging.Configuration.Loggers
{
    /// <summary>
    /// Strong typed class for containing a ConsoleLogger configuration element read from a configuration file.
    /// </summary>
    /// <remarks>This class is intended to be used by the <see cref="LoggerElementCollection{V}"/> class</remarks>
    public class ConsoleLoggerElement : LoggerElementBase, IEquatable<ConsoleLoggerElement>
    {
        /// <summary>
        /// Construct a new <see cref="ConsoleLoggerElement"/> object.
        /// </summary>
        public ConsoleLoggerElement()
            : base("ConsoleLogger")
        {
        }

        ///<summary>
        /// Return an <see cref="ILoggerFactory"/> instance.
        ///</summary>
        public override ILoggerFactory GetLoggerInstance()
        {
            return new ConsoleLoggerFactory();
        }

        public bool Equals(ConsoleLoggerElement consoleLoggerElement)
        {
            if (consoleLoggerElement == null) return false;
            return base.Equals(consoleLoggerElement);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(this, obj)) return true;
            return Equals(obj as ConsoleLoggerElement);
        }

        ///<summary>
        ///Serves as a hash function for this type.
        ///</summary>
        ///<returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
