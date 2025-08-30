using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Xml;
using DemiCode.Scheduling.Tasks;
using DemiCode.Configuration;

namespace DemiCode.Scheduling.Configuration.Tasks
{
    /// <summary>
    /// The base class for all task element configuration classes.
    /// </summary>
    public abstract class TaskElementBase : ScheduleReferenceElementCollection, IKeyedConfigurationElement
    {
        private const string _additionalPropertiesPropertyName = "__additionalProperties";

        private Dictionary<string, string> _additionalPropertiesBackup = null;
        private Dictionary<string, string> _additionalProperties
        {
            get
            {
                if (AdditionalProperties == null && !IsReadOnly())
                    AdditionalProperties = new Dictionary<string, string>(4);

                return AdditionalProperties;
            }
            set
            {
                AdditionalProperties = value;
            }
        }

        /// <summary>
        /// Additional parameters passed to the schedule. Use the <see cref="AddAdditionalProperty"/> method to append properties to this collection.
        /// </summary>
        [ConfigurationProperty(_additionalPropertiesPropertyName, IsRequired = false, IsKey = false)]
        [TypeConverter(typeof(StringDictionaryConverter))]
        public Dictionary<string, string> AdditionalProperties
        {
            get
            {
                if (_additionalPropertiesBackup != null)
                    return _additionalPropertiesBackup;

                 return (Dictionary<string, string>)base[_additionalPropertiesPropertyName];
            }
            set { base[_additionalPropertiesPropertyName] = value; }
        }

        /// <summary>
        /// The element name for this task element. This name will be used when serializing and deserializing configuration XML.
        /// </summary>
        public string TaskElementName { get; private set; }

        /// <summary>
        /// A unique key assigned to this task element. 
        /// </summary>
        public string TaskElementKey { get; protected set; }

        /// <summary>
        /// A collection of name/value pairs for this task loaded from configuration. This includes both element properties 
        /// and additional properties not defined on the element.
        /// </summary>
        public Dictionary<string, object> Parameters
        {
            get
            {
                var props = new Dictionary<String, Object>(ElementInformation.Properties.Count + _additionalProperties.Count);
                foreach (PropertyInformation property in ElementInformation.Properties)
                {
                    if (property.Name == _additionalPropertiesPropertyName) continue;
                    props.Add(property.Name, property.Value);
                }

                foreach (var prop in _additionalProperties)
                {
                    props.Add(prop.Key, prop.Value);
                }

                return props;
            }
        }

        ///// <summary>
        ///// A collection of name/value pairs for this task loaded from configuration. This includes both element properties 
        ///// and additional attributes not defined on the element.
        ///// </summary>
        //public Dictionary<string, object> AdditionalParameters
        //{
        //    get
        //    {
        //        var props = new Dictionary<String, Object>(_additionalProperties.Count);

        //        foreach (var prop in _additionalProperties)
        //        {
        //            props.Add(prop.Key, prop.Value);
        //        }

        //        return props;
        //    }
        //}

        /// <summary>
        /// Add an additional property. These properties can be used as arbitrary parameters in a task configuration. 
        /// E.g. <see cref="CodeTaskElement"/> uses additional properties in order to configure parameters for the method to execute.
        /// </summary>
        /// <param name="name">Name of the property.</param>
        /// <param name="value">Value of the property.</param>
        protected void AddAdditionalProperty(string name, string value)
        {
            _additionalProperties.Add(name, value);
        }

        /// <summary>
        /// Return the <see cref="ITask"/> implementation for this configuration element.
        /// </summary>
        /// <returns>An <see cref="ITask"/> instance.</returns>
        public abstract ITask GetTaskInstance();

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
            var retVal = false;

            if (!base.SerializeElement(null, false))
                return false;

            if (writer != null)
            {
                writer.WriteStartElement(TaskElementName);

                _additionalPropertiesBackup = _additionalProperties;
                var backup = Properties[_additionalPropertiesPropertyName];
                Properties.Remove(_additionalPropertiesPropertyName);

                retVal |= base.SerializeElement(writer, false);

                Properties.Add(backup);
                _additionalProperties = _additionalPropertiesBackup;
                _additionalPropertiesBackup = null;

                // Write out our additional attributes
                foreach (var property in _additionalProperties)
                {
                    writer.WriteAttributeString(property.Key, property.Value);
                }

                writer.WriteEndElement();

                return retVal;

            }

            return base.SerializeElement(writer, false);
        }

        #endregion

        #region IKeyedConfigurationElement Members

        string IKeyedConfigurationElement.ElementName
        {
            get { return TaskElementName; }
        }

        string IKeyedConfigurationElement.ElementKey
        {
            get { return TaskElementKey; }
        }

        #endregion

        /// <summary>
        /// Construct an empty <see cref="TaskElementBase"/> instance.
        /// </summary>
        /// <remarks>Note to inheritors: use this contstructor to assign the element representing your task type.</remarks>
        /// <param name="elementName">The element name representing this task.</param>
        protected TaskElementBase(string elementName)
        {
            TaskElementName = elementName;
            TaskElementKey = Guid.NewGuid().ToString();

            AdditionalProperties = new Dictionary<string, string>();
        }
    }
}
