using System;
using System.Configuration;
using DemiCode.Configuration;

namespace DemiCode.Scheduling.Configuration.Tasks
{
    /// <summary>
    /// A collection of schedule reference elements.
    /// </summary>
    public class ScheduleReferenceElementCollection : ExtendedConfigurationElementCollection
    {
        /// <summary>
        /// Add the specified <see cref="ScheduleReferenceElement"/> to this collection. If an element with the same name 
        /// already exist the old element will be replaced.
        /// </summary>
        /// <param name="scheduleReferenceElement">The element to add.</param>
        public void Add(ScheduleReferenceElement scheduleReferenceElement)
        {
            base.BaseAdd(scheduleReferenceElement);
        }

        /// <summary>
        /// Removes the <see cref="ScheduleReferenceElement"/> at the specified index location.
        /// </summary>
        /// <param name="index">The index location of the <see cref="ScheduleReferenceElement"/> to remove.</param>
        public void Remove(int index)
        {
            BaseRemoveAt(index);
        }

        /// <summary>
        /// Removes the <see cref="ScheduleReferenceElement"/>.
        /// </summary>
        /// <param name="scheduleReferenceElement">The name of the <see cref="ScheduleReferenceElement"/> to remove.</param>
        public void Remove(ScheduleReferenceElement scheduleReferenceElement)
        {
            BaseRemove(BaseIndexOf(scheduleReferenceElement));
        }

        /// <summary>
        /// Return <see cref="ScheduleReferenceElement"/> by its index in the collection.
        /// </summary>
        /// <param name="index">The index of the element to retrieve</param>
        public ScheduleReferenceElement this[int index]
        {
            get
            {
                return (ScheduleReferenceElement)BaseGet(index);
            }
            set
            {
                BaseAdd(index, value);
            }
        }

        ///<summary>
        ///The name used to identify this collection of elements in the configuration file when overridden in a derived class.
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
                return "scheduleref";
            }
        }

        /// <summary>
        /// Returns true if the specified element name is a valid name for this collection type.
        /// </summary>
        /// <param name="elementName">Name of the element as read from the configuration file.</param>
        /// <returns>true if the name is valid; otherwise, false.</returns>
        protected override bool IsElementName(string elementName)
        {
            if (String.IsNullOrEmpty(elementName))
                return false;
            return elementName == "scheduleref";
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
            return ((ScheduleReferenceElement)element).ScheduleName;
        }

        /// <summary>
        /// Create a new <see cref="ScheduleReferenceElement"/>.
        /// </summary>
        /// <returns>A new, empty element.</returns>
        protected override ConfigurationElement CreateNewElement()
        {
            return new ScheduleReferenceElement();
        }

        /// <summary>
        /// Create a new <see cref="ScheduleReferenceElement"/> with the specified element name assigned.
        /// </summary>
        /// <param name="elementName">The name to assign to the configuration element.</param>
        /// <returns>A new, empty element.</returns>
        protected override ConfigurationElement CreateNewElement(string elementName)
        {
            return new ScheduleReferenceElement(elementName);
        }

    }
}
