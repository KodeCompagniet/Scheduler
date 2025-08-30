using System;
using System.Collections.Generic;
using System.Configuration;
using System.Xml;
using DemiCode.Logging.Loggers;
using DemiCode.Shared.Configuration;

namespace DemiCode.Logging.Configuration.Loggers
{
    /// <summary>
    /// The base class for all logger element configuration classes.
    /// </summary>
    public abstract class LoggerElementBase : ConfigurationElement, IKeyedConfigurationElement
    {
        private readonly Dictionary<string, string> _additionalProperties = new Dictionary<string, string>();

        /// <summary>
        /// The element name for this logger element. This name will be used when serializing and deserializing configuration XML.
        /// </summary>
        public string LoggerElementName { get; private set; }

        /// <summary>
        /// A unique key assigned to this logger element. 
        /// </summary>
        public string LoggerElementKey { get; protected set; }


        /// <summary>
        /// A collection of name/value pairs for this task loaded from configuration.
        /// </summary>
        public Dictionary<string, object> Parameters
        {
            get
            {
                var props = new Dictionary<String, Object>();
                foreach (PropertyInformation property in ElementInformation.Properties)
                {
                    props.Add(property.Name, property.Value);
                }

                foreach (var prop in _additionalProperties)
                {
                    props.Add(prop.Key, prop.Value);
                }

                return props;
            }
        }

        ///<summary>
        /// Return an <see cref="ILoggerFactory"/> instance.
        ///</summary>
        public abstract ILoggerFactory GetLoggerInstance();

        #region Deserialization

        /// <summary>
        /// Unrecongnized attributes will be collected in the <see cref="Parameters"/> collection.
        /// </summary>
        /// <param name="name">The name of the unrecognized attribute.</param>
        /// <param name="value">The value of the unrecognized attribute.</param>
        /// <returns>true when an unknown attribute is encountered while deserializing.</returns>
        protected override bool OnDeserializeUnrecognizedAttribute(string name, string value)
        {
            if (value != null)
                value = value.Trim();

            _additionalProperties.Add(name, value);
            return true;
        }

        #endregion

        #region Serialization

        ///<summary>
        ///Writes the configuration data to an XML element in the configuration file when overridden in a derived class.
        ///</summary>
        ///
        ///<returns>
        ///true if the <see cref="T:System.Configuration.ConfigurationElementCollection"></see> was written to the configuration file successfully.
        ///</returns>
        ///
        ///<param name="writer">Output stream that writes XML to the configuration file. </param>
        ///<param name="serializeCollectionKey">true to serialize the collection key; otherwise, false. </param>
        protected override bool SerializeElement(XmlWriter writer, bool serializeCollectionKey)
        {

            if (writer == null)
            {
                if ((Properties.Count > 0) && !base.SerializeElement(null, false))
                    return false;
                return true;
            }

            writer.WriteStartElement(LoggerElementName);

            // Write out our additional attributes
            foreach (var property in _additionalProperties)
            {
                writer.WriteAttributeString(property.Key, property.Value);
            }

            var retVal = base.SerializeElement(writer, false);
            writer.WriteEndElement();

            return retVal;

        }

        #endregion

        #region IKeyedConfigurationElement Members

        string IKeyedConfigurationElement.ElementName
        {
            get { return LoggerElementName; }
        }

        string IKeyedConfigurationElement.ElementKey
        {
            get { return LoggerElementKey; }
        }

        #endregion

        /// <summary>
        /// Construct an empty <see cref="LoggerElementBase"/> instance.
        /// </summary>
        /// <remarks>Note to inheritors: use this contstructor to assign the element representing your logger type.</remarks>
        /// <param name="elementName">The element name representing this logger.</param>
        protected LoggerElementBase(string elementName)
        {
            LoggerElementName = elementName;
            LoggerElementKey = Guid.NewGuid().ToString();
        }
    }
}
