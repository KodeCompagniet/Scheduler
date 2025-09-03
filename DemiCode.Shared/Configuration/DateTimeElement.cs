using System;
using System.Configuration;
using System.Xml;
using System.Globalization;

namespace DemiCode.Configuration
{
    /// <summary>
    /// A configuration element for representing <see cref="DateTime"/> in XML elements.
    /// </summary>
    public class DateTimeElement : ConfigurationElement, IEquatable<DateTimeElement>
    {
        private readonly string _format = "yyyy-MM-dd HH:mm:ss";

        /// <summary>
        /// Return true if <see cref="Value"/> is equal <see cref="DateTime.MinValue"/>, false if not.
        /// </summary>
        public bool IsEmpty { get { return Value == DateTime.MinValue; } }

        /// <summary>
        /// The <see cref="DateTime"/> value.
        /// </summary>
        [ConfigurationProperty("value")]
        public DateTime Value
        {
            get { return (DateTime)base["value"]; }
            set { base["value"] = value; }
        }

        /// <summary>
        /// The format to use when converting <see cref="Value"/> to and from <see cref="String"/>.
        /// </summary>
        public string Format { get { return _format; } }

        /// <summary>
        /// Initialize a new instance of <see cref="DateTimeElement"/>.
        /// </summary>
        public DateTimeElement()
        {
            Value = DateTime.MinValue;
        }

        /// <summary>
        /// Initialize a new instance of <see cref="DateTimeElement"/> with <see cref="Value"/> set to <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The value to use.</param>
        public DateTimeElement(DateTime value)
        {
            Value = value;
        }

        /// <summary>
        /// Note to inheritors: override the format used to ParseExact the datetime structure.
        /// </summary>
        /// <param name="format">The format to use when parsing <see cref="Value"/> to and from string.</param>
        protected DateTimeElement(string format)
        {
            _format = format;
        }

        /// <summary>
        /// Deserializing expects a simple text value in the current node of the reader. This value is then converted to <see cref="DateTime"/>.
        /// </summary>
        ///
        ///<param name="reader">The <see cref="T:System.Xml.XmlReader"></see> that reads from the configuration file.</param>
        ///<param name="serializeCollectionKey">true to serialize only the collection key properties; otherwise, false.</param>
        protected override void DeserializeElement(XmlReader reader, bool serializeCollectionKey)
        {
            var value = reader.ReadElementString();
            Value = DateTime.ParseExact(value, _format, CultureInfo.CurrentCulture);
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
            if (Value == DateTime.MinValue)
                return false;

            if (writer == null)
                return true;

            writer.WriteString(ToString());
            return true;
        }

        ///<summary>
        ///Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        ///</summary>
        ///
        ///<returns>
        ///A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
        ///</returns>
        ///<filterpriority>2</filterpriority>
        public override string ToString()
        {
            return Value.ToString(_format, CultureInfo.CurrentCulture);
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
        public bool Equals(DateTimeElement compareTo)
        {
            if (compareTo == null) return false;
            if (!base.Equals(compareTo)) return false;
            return _format == compareTo._format;
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
            return Equals(compareTo as DateTimeElement);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hash = base.GetHashCode();
                hash = (hash * 397) ^ Value.GetHashCode();
                hash = (hash * 397) ^ (_format?.GetHashCode() ?? 0);
                return hash;
            }
        }

    }
}
