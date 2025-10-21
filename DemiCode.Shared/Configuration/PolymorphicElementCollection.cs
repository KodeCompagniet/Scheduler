using System;
using System.Collections.Generic;
using System.Configuration;
using DemiCode.Configuration;
using DemiCode.Shared.Factories;

namespace DemiCode.Configuration
{
    /// <summary>
    /// This interface is intended for use by DemiCode internally.
    /// </summary>
    /// <typeparam name="V">A type that implements a class that will indicate wether configuration is being serialized or not.</typeparam>
    public abstract class PolymorphicElementCollection<V> : ExtendedConfigurationElementCollection, IEquatable<PolymorphicElementCollection<V>> where V : ISerializationStateProvider, new()
    {
        private readonly V _serializationStateProvider = new V();

        /// <summary>
        /// Override in subclass to return the default <see cref="ConfigurationElement"/> class.
        /// </summary>
        /// <remarks>This type is used in <see cref="ConfigurationElementCollection.CreateNewElement()"/>.</remarks>
        /// <returns>The default element type.</returns>
        protected abstract Type GetDefaultElementType();

        /// <summary>
        /// Override in subclass to return the <see cref="ConfigurationElement"/> class that implements the specified element name.
        /// </summary>
        /// <remarks>The type is used in <see cref="ConfigurationElementCollection.CreateNewElement(string)"/>.</remarks>
        /// <param name="elementName">The element name to get the type for.</param>
        /// <returns>The <see cref="ConfigurationElement"/> type to use for <paramref name="elementName"/>.</returns>
        protected abstract Type GetElementType(string elementName);

        private Dictionary<object, string> _keyToElementType;

        /// <summary>
        /// Get the key of the specified element in this collection.
        /// </summary>
        /// <param name="element">An element from this collection.</param>
        /// <returns>The key of the element</returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            var el = element as IKeyedConfigurationElement;
            if (el == null)
                throw new ArgumentException("Unknown ConfigurationElement type", "element");

            object key = el.ElementKey;

            if (_serializationStateProvider.IsSerializing)
            {
                if (_keyToElementType == null)
                    _keyToElementType = new Dictionary<object, string>();

                _keyToElementType[key] = el.ElementName;
            }

            return key;
        }

        ///<summary>
        ///When overridden in a derived class, creates a new <see cref="T:System.Configuration.ConfigurationElement"></see>.
        ///</summary>
        ///
        ///<returns>
        ///A new <see cref="T:System.Configuration.ConfigurationElement"></see>.
        ///</returns>
        ///
        protected override ConfigurationElement CreateNewElement()
        {
            var type = GetDefaultElementType();
            if (type == null)
            {
                throw new InvalidConfigurationException("Could not create default element");
            }

            return Factory.CreateInstance<ConfigurationElement>(type);
        }

        ///<summary>
        ///Creates a new <see cref="T:System.Configuration.ConfigurationElement"></see> when overridden in a derived class.
        ///</summary>
        ///
        ///<returns>
        ///A new <see cref="T:System.Configuration.ConfigurationElement"></see>.
        ///</returns>
        ///
        ///<param name="elementName">The name of the <see cref="T:System.Configuration.ConfigurationElement"></see> to create. </param>
        protected override ConfigurationElement CreateNewElement(string elementName)
        {
            var elementType = elementName;

            if (_serializationStateProvider.IsSerializing)
            {
                if (_keyToElementType == null || !_keyToElementType.ContainsKey(elementName))
                    throw new InvalidConfigurationException(String.Format("Could not create element {0}", elementName));
                elementType = _keyToElementType[elementName];
            }

            var type = GetElementType(elementType);
            if (type == null)
            {
                throw new InvalidConfigurationException(String.Format("Could not create element {0} with key {1}", elementType, elementName));
            }

            return Factory.CreateInstance<ConfigurationElement>(type);
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
        public bool Equals(PolymorphicElementCollection<V> compareTo)
        {
            if (ReferenceEquals(this, compareTo)) return true;
            return base.Equals(compareTo);
        }

        ///<summary>
        ///Compares the <see cref="T:System.Configuration.ConfigurationElementCollection"></see> to the specified object.
        /// Elements in this collection will be compared to elements in the <paramref name="compareTo"/> collection.
        /// Additionally, any properties defined on this collection will be compared.
        ///</summary>
        ///
        ///<returns>
        ///true if the object to compare with is equal to the current <see cref="T:System.Configuration.ConfigurationElementCollection"></see> instance; otherwise, false. The default is false.
        ///</returns>
        ///
        ///<param name="compareTo">The object to compare. </param>
        public override bool Equals(object compareTo)
        {
            return Equals(compareTo as PolymorphicElementCollection<V>);
        }

        ///<summary>
        ///Gets a unique value representing the <see cref="T:System.Configuration.ConfigurationElementCollection"></see> instance.
        ///</summary>
        ///
        ///<returns>
        ///A unique value representing the <see cref="T:System.Configuration.ConfigurationElementCollection"></see> current instance.
        ///</returns>
        ///
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}