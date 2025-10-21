using System;
using System.Runtime.Serialization;

namespace DemiCode.Configuration
{
    /// <summary>
    /// An exception that is thrown when a configuration section encounter errors.
    /// </summary>
    [Serializable]
    public class InvalidConfigurationException : Exception
    {
        #region ctors
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidConfigurationException"/> class.
        /// </summary>
        public InvalidConfigurationException()
            : base()
        { }

        /// <summary>
        /// Initializes an instance of the <see cref="InvalidConfigurationException"/> class with a specified error message. 
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public InvalidConfigurationException(string message)
            : base(message)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidConfigurationException"/> class with a
        /// specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception,
        /// or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public InvalidConfigurationException(string message, Exception innerException)
            : base(message, innerException)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidConfigurationException"/> class with serialized data.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown. </param>
        /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
        protected InvalidConfigurationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
        #endregion
    }
}
