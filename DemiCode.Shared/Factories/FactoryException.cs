using System;
using System.Runtime.Serialization;

namespace DemiCode.Shared.Factories
{
    /// <summary>
    /// An Exception that is thrown when the Factory class can't create an object.
    /// </summary>
    [Serializable]
    public class FactoryException : Exception
    {
        #region ctors
        /// <summary>
        /// Initializes a new instance of the <see cref="FactoryException"/> class.
        /// </summary>
        public FactoryException()
            : base()
        { }

        /// <summary>
        /// Initializes an instance of the <see cref="FactoryException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public FactoryException(string message)
            : base(message)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="FactoryException"/> class with a
        /// specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception,
        /// or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public FactoryException(string message, Exception innerException)
            : base(message, innerException)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="FactoryException"/> class with serialized data.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown. </param>
        /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
        protected FactoryException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
        #endregion
    }

    /// <summary>
    /// An Exception that is thrown when the configuration provided the Factory class is incorrect.
    /// </summary>
    [Serializable]
    public class FactoryConfigurationException : FactoryException
    {
        #region ctors
        /// <summary>
        /// Initializes a new instance of the <see cref="FactoryConfigurationException"/> class.
        /// </summary>
        public FactoryConfigurationException()
            : base()
        { }

        /// <summary>
        /// Initializes an instance of the <see cref="FactoryConfigurationException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public FactoryConfigurationException(string message)
            : base(message)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="FactoryConfigurationException"/> class with a
        /// specified error message and a reference to the inner exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception,
        /// or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public FactoryConfigurationException(string message, Exception innerException)
            : base(message, innerException)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="FactoryConfigurationException"/> class with serialized data.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown. </param>
        /// <param name="context">The <see cref="StreamingContext"/> that contains contextual information about the source or destination.</param>
        protected FactoryConfigurationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
        #endregion
    }
}
