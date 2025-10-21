using System;
using System.Collections.Generic;
using System.Configuration;
using System.Xml;

namespace DemiCode.Configuration
{
    /// <summary>
    /// Represent a list of values. 
    /// </summary>
    /// <remarks>This class is intended for inheritance. Subclasses must provide <b>collection</b> and <b>item</b> element names.
    /// These names will be used when loading and serializing values to configuration XML. See the <see cref="DaysElement"/> class 
    /// for an example implementation using <see cref="DayOfWeek"/> values.
    /// </remarks>
    /// <typeparam name="T">An enum type that represents the valid names this element can contain</typeparam>
    public abstract class EnumElement<T> : ConfigurationElement, IEquatable<EnumElement<T>>
    {
        private readonly string _collectionElementName;
        private readonly string _itemElementName;

        /// <summary>
        /// The list of values.
        /// </summary>
        [ConfigurationProperty("value")]
        public T[] Value
        {
            get { return (T[])base["value"]; }
            set { base["value"] = value; }
        }

        /// <summary>
        /// Construct a new <see cref="EnumElement{T}"/>.
        /// </summary>
        /// <param name="collectionElementName">The element name used to group values in configuration, e.g. "days".</param>
        /// <param name="itemElementName">The element name used for each value, e.g. "day".</param>
        protected EnumElement(string collectionElementName, string itemElementName)
        {
            _collectionElementName = collectionElementName;
            _itemElementName = itemElementName;
        }

        /// <summary>
        /// Construct a new <see cref="EnumElement{T}"/>.
        /// </summary>
        /// <param name="value">Initial list of values.</param>
        /// <param name="collectionElementName">The element name used to group values in configuration, e.g. "days".</param>
        /// <param name="itemElementName">The element name used for each value, e.g. "day".</param>
        protected EnumElement(T[] value, string collectionElementName, string itemElementName)
            : this(collectionElementName, itemElementName)
        {
            Value = value;
        }

        /// <summary>
        /// Deserializing expects a single top node with a list of children as the current node of the reader. This value is then converted to a collection of enumerated values.
        /// </summary>
        ///
        ///<param name="reader">The <see cref="T:System.Xml.XmlReader"></see> that reads from the configuration file.</param>
        ///<param name="serializeCollectionKey">true to serialize only the collection key properties; otherwise, false.</param>
        protected override void DeserializeElement(XmlReader reader, bool serializeCollectionKey)
        {
            var values = new List<T>();

            using (var subtree = reader.ReadSubtree())
            {
                // First, skip the container element
                subtree.Read();
                if (subtree.Name != _collectionElementName)
                {
                    throw new ConfigurationErrorsException("Unrecognized collection element " + subtree.Name + "'");
                }

                subtree.ReadStartElement();

                while (true)
                {
                    // Skip any whitespace
                    while (subtree.NodeType == XmlNodeType.Whitespace)
                    {
                        if (!subtree.Read())
                            break;
                    }

                    // Stop when we reach the end element of our collection
                    if (subtree.NodeType == XmlNodeType.EndElement)
                        break;

                    if (subtree.Name != _itemElementName)
                    {
                        throw new ConfigurationErrorsException("Unrecognized item element " + subtree.Name + "'");
                    }

                    var value = subtree.ReadElementString();
                    values.Add((T)Enum.Parse(typeof(T), value, true));
                }
            }

            Value = values.ToArray();
        }

        ///<summary>
        ///Writes the contents of this configuration element to the configuration file.
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

            foreach (var element in Value)
            {
                writer.WriteStartElement(_itemElementName);
                writer.WriteString(element.ToString());
                writer.WriteEndElement();
            }

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
        public bool Equals(EnumElement<T> compareTo)
        {
            if (compareTo == null) return false;
            if (_collectionElementName != compareTo._collectionElementName) return false;
            if (_itemElementName != compareTo._itemElementName) return false;

            // Cannot rely on base equals since our array needs special treatment
            //if (!base.Equals(enumElement)) return false;
            return EqualsArray(Value, compareTo.Value);
        }

        private static bool EqualsArray(T[] left, T[] right)
        {
            if (ReferenceEquals(left, right)) return true;
            if (left == null || right == null) return false;
            if (left.LongLength != right.LongLength) return false;
            for (long i = 0; i < left.LongLength; i++)
            {
                if (!left[i].Equals(right[i])) return false;
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
            return Equals(compareTo as EnumElement<T>);
        }

        ///<summary>
        ///Serves as a hash function for this type.
        ///</summary>
        ///<returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = _collectionElementName?.GetHashCode() ?? 0;
                hash = (hash * 397) ^ (_itemElementName?.GetHashCode() ?? 0);
                if (Value != null)
                {
                    foreach (var item in Value)
                    {
                        hash = (hash * 397) ^ (item?.GetHashCode() ?? 0);
                    }
                }
                return hash;
            }
        }
    }
}
