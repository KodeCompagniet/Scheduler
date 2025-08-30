using System.Configuration;
using System.Xml;
using DemiCode.Scheduling.Schedules;
using DemiCode.Configuration;

namespace DemiCode.Scheduling.Configuration.Schedules
{
    /// <summary>
    /// Base class for schedule configuration element classes.
    /// </summary>
    /// 
    /// <remarks>This class is intended to be used by the <see cref="ScheduleElementCollection"/> class.
    /// Note to implementers: When inheriting <c>ScheduleElementBase</c> your class must provide a default constructor.
    /// </remarks>
    public abstract class ScheduleElementBase : ConfigurationElement, IKeyedConfigurationElement
    {
        private readonly string _elementName;

        #region IKeyedConfigurationElement Members

        string IKeyedConfigurationElement.ElementName
        {
            get { return _elementName; }
        }

        string IKeyedConfigurationElement.ElementKey
        {
            get { return Name; }
        }

        #endregion

        /// <summary>
        /// The name of our schedule element. This attribute must be unique within the list of schedules.
        /// </summary>
        [ConfigurationProperty("name", IsRequired = false, IsKey = false, DefaultValue="")]
        public string Name
        {
            get { return ((string)base["name"]); }
            set { base["name"] = value; }
        }

        /// <summary>
        /// Get an instance of the schedule type.
        /// </summary>
        /// <returns>An <see cref="ISchedule"/> instance.</returns>
        public abstract ISchedule GetScheduleInstance();

        /// <summary>
        /// Construct a <see cref="ScheduleElementBase"/> instance based on the specified element name.
        /// </summary>
        /// <param name="elementName">Name of the xml element loaded from the configuration file.</param>
        protected ScheduleElementBase(string elementName)
        {
            _elementName = elementName;
        }

        /// <summary>
        /// Construct a <see cref="ScheduleElementBase"/> instance based on the specified element name.
        /// </summary>
        /// <param name="elementName">Name of the xml element loaded from the configuration file.</param>
        /// <param name="scheduleName">Initial value of the <see cref="Name"/> property.</param>
        protected ScheduleElementBase(string elementName, string scheduleName) : this(elementName)
        {
            Name = scheduleName;
        }

        ///<summary>
        ///Writes the contents of this configuration element to the configuration file when implemented in a derived class.
        ///</summary>
        ///
        ///<returns>
        ///true if any data was actually serialized; otherwise, false.
        ///</returns>
        ///
        ///<param name="writer">The <see cref="T:System.Xml.XmlWriter"></see> that writes to the configuration file. </param>
        ///<param name="serializeCollectionKey">true to serialize only the collection key properties; otherwise, false. </param>
        protected override bool SerializeElement(XmlWriter writer, bool serializeCollectionKey)
        {
            var retVal = false;

            if (!base.SerializeElement(null, false))
                return false;

            if (writer != null)
            {
                writer.WriteStartElement(_elementName);
                retVal |= base.SerializeElement(writer, false);
                writer.WriteEndElement();

                return retVal;
            }

            retVal = base.SerializeElement(writer, false);
            return retVal;
        }
    }
}
