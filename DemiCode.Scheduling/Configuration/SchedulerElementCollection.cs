using System;
using System.Configuration;
using DemiCode.Configuration;

namespace DemiCode.Scheduling.Configuration
{

    /// <summary>
    /// A collection for <see cref="SchedulerElement"/> instances loaded from a scheduler configuration section.
    /// </summary>
    /// <remarks>This class is intended to be used by the <see cref="SchedulerSettings"/> class</remarks>
    public class SchedulerElementCollection : ExtendedConfigurationElementCollection
    {
        ///<summary>
        ///Gets the name used to identify this collection of elements in the configuration file.
        ///</summary>
        ///
        ///<returns>
        ///The name of the collection; otherwise, an empty string. The default is an empty string.
        ///</returns>
        ///
        protected override string ElementName
        {
            get
            {
                return "scheduler";
            }
        }

        /// <summary>
        /// Return <see cref="SchedulerElement"/> by its index in the collection.
        /// </summary>
        /// <param name="index">The index of the element to retrieve</param>
        public SchedulerElement this[int index]
        {
            get
            {
                return (SchedulerElement)BaseGet(index);
            }
            set
            {
                BaseAdd(index, value);
            }
        }

        /// <summary>
        /// Return <see cref="SchedulerElement"/> by its <see cref="SchedulerElement.Name"/> value.
        /// </summary>
        /// <param name="name">The name of the element to retrieve</param>
        new public SchedulerElement this[string name]
        {
            get
            {
                return (SchedulerElement)BaseGet(name);
            }
            set
            {
                BaseAdd(value);
            }
        }

        /// <summary>
        /// Returns true if the specified element name is a valid name for this collection type.
        /// </summary>
        /// <param name="elementName">Name of the element as read from the configuration file.</param>
        /// <returns>true if the name is valid; otherwise, false.</returns>
        protected override bool IsElementName(string elementName)
        {
            return (!String.IsNullOrEmpty(elementName)) && (elementName == SchedulerElement._ElementName);
        }

        /// <summary>
        /// Collection type is <see cref="ConfigurationElementCollectionType.BasicMapAlternate"/>.
        /// </summary>
        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.BasicMapAlternate; }
        }

        /// <summary>
        /// Get the key of the specified element in this collection.
        /// </summary>
        /// <param name="element">An element from this collection.</param>
        /// <returns>The key of the element</returns>
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((SchedulerElement)element).Name;
        }

        /// <summary>
        /// Create a new <see cref="SchedulerElement"/>.
        /// </summary>
        /// <returns>A new, empty element.</returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new SchedulerElement();
        }

        /// <summary>
        /// Create a new <see cref="SchedulerElement"/> with the specified element name assigned.
        /// </summary>
        /// <param name="elementName">The name to assign to the configuration element.</param>
        /// <returns>A new, empty element.</returns>
        protected override ConfigurationElement CreateNewElement(string elementName)
        {
            return new SchedulerElement();
        }

        /// <summary>
        /// Get the number of schedulers in this collection.
        /// </summary>
        public new int Count
        {
            get { return base.Count; }
        }

        /// <summary>
        /// Add the specified <see cref="SchedulerElement"/> to this collection. If an element with the same name 
        /// already exist the old element will be replaced.
        /// </summary>
        /// <param name="scheduler">The scheduler element to add.</param>
        public void Add(SchedulerElement scheduler)
        {
            base.BaseAdd(scheduler);
        }

        /// <summary>
        /// Removes the <see cref="SchedulerElement"/> at the specified index location.
        /// </summary>
        /// <param name="index">The index location of the <see cref="SchedulerElement"/> to remove.</param>
        public void Remove(int index)
        {
            BaseRemoveAt(index);
        }

        /// <summary>
        /// Removes the <see cref="SchedulerElement"/> with the specified name.
        /// </summary>
        /// <param name="name">The name of the <see cref="SchedulerElement"/> to remove.</param>
        public void Remove(string name)
        {
            if (!BaseIsRemoved(name))
                BaseRemove(name);
        }
    }
}
