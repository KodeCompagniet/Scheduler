using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Xml;
using DemiCode.Scheduling.Schedules;
using DemiCode.Configuration;
using DemiCode.Shared.Factories;

namespace DemiCode.Scheduling.Configuration.Schedules
{
    /// <summary>
    /// Strong typed class for containing a task element read from a configuration file.
    /// All <c>schedule</c> elements within the <c>scheduler</c> element will
    /// be loaded and exposed as a collection of <see cref="ISchedule"/> instances.
    /// </summary>
    /// <remarks>This class is intended to be used by the <see cref="ScheduleElementCollection"/> class</remarks>
    public class ScheduleElement : ScheduleElementBase, IEquatable<ScheduleElement>
    {
        private const string _elementName = "schedule";
        private const string _additionalPropertiesPropertyName = "__additionalProperties";
        private const string _typePropertyName = "type";

        private readonly XmlDocument _additionalElements = new XmlDocument();

        private Dictionary<string, string> _additionalProperties
        {
            get
            {
                if (AdditionalProperties == null && !IsReadOnly())
                    AdditionalProperties = new Dictionary<string, string>(4);

                return AdditionalProperties;
            }
        }

        /// <summary>
        /// A collection of name/value pairs for this schedule loaded from configuration.
        /// </summary>
        /// <remarks>This collection contains name/value pairs for all parameters related to this configuration element. This includes 
        /// predefined properties (like <see cref="ScheduleElementBase.Name"/> and <see cref="ScheduleType"/>) 
        /// and additional name/value pairs.
        /// 
        /// Note: to add additional name/value pairs use the <see cref="AddAdditionalProperty"/> method.</remarks>
        public Dictionary<string, object> Parameters
        {
            get
            {
                var props = new Dictionary<string, object>(ElementInformation.Properties.Count + _additionalProperties.Count);
                foreach (PropertyInformation property in ElementInformation.Properties)
                {
                    if (property.Name != _typePropertyName && property.Name != _additionalPropertiesPropertyName)
                        props.Add(property.Name, property.Value);
                }

                foreach (var prop in _additionalProperties)
                {
                    props.Add(prop.Key, prop.Value);
                }

                return props;
            }
        }

        /// <summary>
        /// Add additional name/value property to this schedule.
        /// </summary>
        /// <param name="name">Name of the property. If a property with this name already exist, the value will be overwritten.</param>
        /// <param name="value">Value of the property.</param>
        public void AddAdditionalProperty(string name, string value)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            if (value == null)
                throw new ArgumentNullException("value");

            _additionalProperties[name] = value;
        }

        /// <summary>
        /// Get an instance of the task type.
        /// </summary>
        [Obsolete("Use GetScheduleInstance", true)]
        public virtual ISchedule Instance
        {
            get
            {
                return Factory.CreateInstance<ISchedule>(ScheduleType);
            }
        }

        /// <summary>
        /// Used by <see cref="ScheduleElementCollection"/> to determine if a schedule element is valid or not.
        /// </summary>
        internal static bool IsValidElementName(string elementName)
        {
            if (elementName == null)
                throw new ArgumentNullException("elementName");

            return elementName == _elementName;
        }

        /// <summary>
        /// Construct an empty <see cref="ScheduleElement"/> instance.
        /// </summary>
        public ScheduleElement()
            : base(_elementName)
        {
            _additionalElements.AppendChild(_additionalElements.CreateElement(_elementName));
        }

        /// <summary>
        /// Construct an empty <see cref="ScheduleElement"/> instance.
        /// </summary>
        /// <param name="name">Name of the schedule element.</param>
        /// <param name="scheduleType">The schedule type. This type must implement the <see cref="ISchedule"/> interface.</param>
        public ScheduleElement(string name, Type scheduleType)
            : base(_elementName, name)
        {
            if (scheduleType == null)
                throw new ArgumentNullException("scheduleType");

            ValidateType(scheduleType);

            _additionalElements.AppendChild(_additionalElements.CreateElement(_elementName));

            ScheduleType = scheduleType;
        }

        private static void ValidateType(Type scheduleType)
        {
            if (scheduleType.GetInterface(typeof(ISchedule).Name) == null)
            {
                throw new ArgumentException("Schedule type must implement the DemiCode.Scheduling.Schedules.ISchedule interface", "scheduleType");
            }
        }

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

        ///<summary>
        ///Gets a value indicating whether an unknown element is encountered during deserialization.
        ///</summary>
        ///
        ///<returns>
        ///true when an unknown element is encountered while deserializing.
        ///</returns>
        ///
        ///<param name="reader">The <see cref="T:System.Xml.XmlReader"></see> object being used for deserialization.</param>
        ///<param name="elementName">The name of the unknown subelement.</param>
        protected override bool OnDeserializeUnrecognizedElement(string elementName, XmlReader reader)
        {
            var document = new XmlDocument();
            using (var schedulereader = reader.ReadSubtree())
            {
                document.Load(schedulereader);
            }

            _additionalElements.DocumentElement.AppendChild(_additionalElements.ImportNode(document.DocumentElement, true));

            return true;
        }


        private XmlDocument _forSerialization;

        ///<summary>
        ///Called before serialization.
        ///</summary>
        ///<param name="writer">The <see cref="T:System.Xml.XmlWriter"></see> object that will be used to serialize the <see cref="T:System.Configuration.ConfigurationElement"></see> object.</param>
        protected override void PreSerialize(XmlWriter writer)
        {
            base.PreSerialize(writer);

            _forSerialization = GetXmlData();
        }

        /// <summary>
        /// Get a document with all attributes and child elements for this configuration element.
        /// </summary>
        private XmlDocument GetXmlData()
        {
            var doc = (XmlDocument)_additionalElements.CloneNode(true);
            XmlNode e = doc.DocumentElement;

            if (e == null)
                throw new InvalidOperationException();

            foreach (ConfigurationProperty property in Properties)
            {
                // Skip serializing additional properties
                if (property.Name == _additionalPropertiesPropertyName)
                    continue;

                // Skip props with null value
                var propValue = base[property];
                if (propValue == null)
                    continue;

                var attribute = doc.CreateAttribute(property.Name);

                if (propValue is Type)
                {
                    attribute.Value = ((Type)propValue).AssemblyQualifiedName;
                }
                else
                {
                    attribute.Value = propValue.ToString();
                }

                e.Attributes.SetNamedItem(attribute);
            }

            if (_additionalProperties != null)
            {
                foreach (var property in _additionalProperties)
                {
                    var attribute = doc.CreateAttribute(property.Key);
                    attribute.Value = property.Value;
                    e.Attributes.SetNamedItem(attribute);
                }
            }

            return doc;
        }

        /// <summary>
        /// Write the contents of this GenericScheduleElement to the configuration file. 
        /// </summary>
        /// <param name="writer">Configuration file writer</param>
        /// <param name="serializeCollectionKey">true to only serialize the collection key properties; otherwise, false.</param>
        /// <seealso cref="ConfigurationElement.SerializeElement"/>
        /// <returns>true if serialization succeeds; otherwise, false.</returns>
        protected override bool SerializeElement(XmlWriter writer, bool serializeCollectionKey)
        {
            PreSerialize(writer);

            if (_forSerialization == null || _forSerialization.DocumentElement == null)
                return false;

            if (writer == null)
                return true;

            _forSerialization.DocumentElement.WriteTo(writer);

            return true;
        }

        /// <summary>
        /// The type for this schedule element. The type assigned must implement the <see cref="ISchedule"/> interface.
        /// </summary>
        [ConfigurationProperty(_typePropertyName, IsRequired = true, IsKey = false)]
        [TypeConverter(typeof(TypeNameConverter))]
        public Type ScheduleType
        {
            get { return ((Type)base[_typePropertyName]); }
            set
            {
                if (value != null)
                {
                    ValidateType(value);
                }
                base[_typePropertyName] = value;
            }
        }

        /// <summary>
        /// Additional parameters passed to the schedule. Use the <see cref="AddAdditionalProperty"/> method to append properties to this collection.
        /// </summary>
        [ConfigurationProperty(_additionalPropertiesPropertyName, IsRequired = false, IsKey = false)]
        [TypeConverter(typeof(StringDictionaryConverter))]
        public Dictionary<string, string> AdditionalProperties
        {
            get { return (Dictionary<string, string>)base[_additionalPropertiesPropertyName]; }
            set { base[_additionalPropertiesPropertyName] = value; }
        }

        /// <summary>
        /// Get an instance of the schedule type.
        /// </summary>
        /// <returns>An instance of the type in <see cref="ScheduleType"/>.</returns>
        public override ISchedule GetScheduleInstance()
        {
            var doc = GetXmlData();

            return Factory.CreateInstance<ISchedule>(ScheduleType, doc.DocumentElement);
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
        public bool Equals(ScheduleElement compareTo)
        {
            if (compareTo == null) return false;
            if (Name != compareTo.Name) return false;
            if (ScheduleType != compareTo.ScheduleType) return false;
            if (_additionalElements.OuterXml != compareTo._additionalElements.OuterXml) return false;
            return CompareDictionary(_additionalProperties, compareTo._additionalProperties);
        }

        private static bool CompareDictionary(Dictionary<string, string> left, Dictionary<string, string> right)
        {
            if (ReferenceEquals(left, right)) return true;
            if (left == null || right == null) return false;
            if (left.Count != right.Count) return false;

            foreach (string key in left.Keys)
            {
                if (!right.ContainsKey(key)) return false;
                if (left[key] != right[key]) return false;
            }

            return true;
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
            return Equals(compareTo as ScheduleElement);
        }

        ///<summary>
        ///Gets a unique value representing the <see cref="T:System.Configuration.ConfigurationElement"></see> current instance.
        ///</summary>
        ///
        ///<returns>
        ///A unique value representing the <see cref="T:System.Configuration.ConfigurationElement"></see> current instance.
        ///</returns>
        ///
        public override int GetHashCode()
        {
            return _additionalElements.GetHashCode();
        }
    }
}

