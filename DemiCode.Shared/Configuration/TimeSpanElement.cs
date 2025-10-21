using System;
using System.Configuration;
using System.Xml;

namespace DemiCode.Configuration
{
    /// <summary>
    /// Configuration element for storing <see cref="TimeSpan"/> values.
    /// </summary>
    public class TimeSpanElement : ConfigurationElement, IEquatable<TimeSpanElement>
    {
        /// <summary>
        /// The time span.
        /// </summary>
        [ConfigurationProperty("value")]
        public TimeSpan Value
        {
            get { return (TimeSpan)base["value"]; }
            set { base["value"] = value; }
        }

        /// <summary>
        /// Initialize a new instance of <see cref="TimeSpanElement"/>.
        /// </summary>
        public TimeSpanElement()
        {
        }

        /// <summary>
        /// Initialize a new instance of <see cref="TimeSpanElement"/> with <see cref="Value"/> set to <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The value to use.</param>
        public TimeSpanElement(TimeSpan value)
        {
            Value = value;
        }

        /// <summary>
        /// Deserializing expects a simple text value in the current node of the reader. This value is then converted to a <see cref="TimeSpan"/>.
        /// </summary>
        ///
        ///<param name="reader">The <see cref="T:System.Xml.XmlReader"></see> that reads from the configuration file.</param>
        ///<param name="serializeCollectionKey">true to serialize only the collection key properties; otherwise, false.</param>
        protected override void DeserializeElement(XmlReader reader, bool serializeCollectionKey)
        {
            var value = reader.ReadElementString();
            Value = TimeSpan.Parse(value);
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
            if (writer == null)
                return true;

            writer.WriteString(Value.ToString());
            return true;
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
        public bool Equals(TimeSpanElement compareTo)
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
            return Equals(compareTo as TimeSpanElement);
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
