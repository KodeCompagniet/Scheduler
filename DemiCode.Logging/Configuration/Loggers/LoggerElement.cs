using System;
using System.ComponentModel;
using System.Configuration;
using DemiCode.Logging.Loggers;
using DemiCode.Shared.Factories;

namespace DemiCode.Logging.Configuration.Loggers
{
    /// <summary>
    /// Strong typed class for containing a task element read from a configuration file.
    /// All <c>logger</c> elements within the <c>scheduler/loggers</c> element will
    /// be loaded and exposed as a collection of <see cref="ILoggerFactory"/> instances.
    /// </summary>
    /// <remarks>This class is intended to be used by the <see cref="LoggerElementCollection{V}"/> class</remarks>
    public class LoggerElement : LoggerElementBase
    {
        ///<summary>
        /// Return an <see cref="ILoggerFactory"/> instance.
        ///</summary>
        public override ILoggerFactory GetLoggerInstance()
        {
            if (LoggerType == null)
                throw new InvalidOperationException("LoggerType is not set");

            return Factory.CreateInstance<ILoggerFactory>(LoggerType);
        }

        /// <summary>
        /// Construct an empty <see cref="LoggerElement"/> instance.
        /// </summary>
        public LoggerElement() : base("logger") { }

        /// <summary>
        /// Construct an empty <see cref="LoggerElement"/> instance.
        /// </summary>
        public LoggerElement(Type loggerType)
            : this()
        {
            if (loggerType == null)
                throw new ArgumentNullException(nameof(loggerType));

            ValidateLoggerType(loggerType);

            LoggerType = loggerType;
        }

        private static void ValidateLoggerType(Type loggerType)
        {
            if (!typeof(ILoggerFactory).IsAssignableFrom(loggerType))
            {
                throw new ArgumentException("Logger type must implement the DemiCode.Scheduling.Loggers.ILoggerFactory interface", nameof(loggerType));
            }
        }


        /// <summary>
        /// The logger factory type.
        /// </summary>
        [ConfigurationProperty("type", IsRequired = true, IsKey = false)]
        [TypeConverter(typeof(TypeNameConverter))]
        public Type LoggerType
        {
            get { return ((Type)base["type"]); }
            set
            {
                if (value != null)
                    ValidateLoggerType(value);

                 base["type"] = value;
            }
        }


        ///<summary>
        ///Indicates whether the current object is equal to another object of the same type.
        ///</summary>
        ///
        ///<returns>
        ///true if the current object is equal to the other parameter; otherwise, false.
        ///</returns>
        ///
        ///<param name="loggerElement">An object to compare with this object.</param>
        public bool Equals(LoggerElement loggerElement)
        {
            if (loggerElement == null) return false;
            if (!base.Equals(loggerElement)) return false;
            //if (LoggerType != loggerElement.LoggerType) return false;
            return true;
        }

        ///<summary>
        ///Compares the <see cref="T:System.Configuration.ConfigurationElementCollection"></see> to the specified object.
        ///</summary>
        ///
        ///<returns>
        ///true if the object to compare with is equal to the current <see cref="T:System.Configuration.ConfigurationElementCollection"></see> instance; otherwise, false. The default is false.
        ///</returns>
        ///
        ///<param name="compareTo">The object to compare. </param>
        public override bool Equals(object compareTo)
        {
            if (ReferenceEquals(this, compareTo)) return true;
            return Equals(compareTo as LoggerElement);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
