using System;
using DemiCode.Logging;

namespace DemiCode.Logging.Configuration
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
        /// <returns>An <see cref="ConsoleLoggerFactory"/> instance.</returns>
        public override ILoggerFactory GetLoggerInstance()
        {
            return new ConsoleLoggerFactory();
        }

        ///<summary>
        ///Indicates whether the current object is equal to another object of the same type.
        ///</summary>
        ///
        ///<returns>
        ///true if the current object is equal to the other parameter; otherwise, false.
        ///</returns>
        ///
        ///<param name="compareTo">An object to compare with this object.</param>
        public bool Equals(ConsoleLoggerElement compareTo)
        {
            if (compareTo == null) return false;
            return base.Equals(compareTo);
        }

        ///<summary>
        ///Compares the current <see cref="T:System.Configuration.ConfigurationElement"></see> instance to the specified object.
        ///</summary>
        ///
        ///<returns>
        ///true if the object to compare with is equal to the current <see cref="T:System.Configuration.ConfigurationElement"></see> instance; otherwise, false. The default is false. 
        ///</returns>
        ///
        ///<param name="compareTo">The object to compare with.</param>
        public override bool Equals(object compareTo)
        {
            if (ReferenceEquals(this, compareTo)) return true;
            return Equals(compareTo as ConsoleLoggerElement);
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
