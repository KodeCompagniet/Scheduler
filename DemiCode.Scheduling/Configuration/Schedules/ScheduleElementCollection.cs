using System;
using System.Collections.Generic;
using System.Configuration;
using DemiCode.Configuration;

namespace DemiCode.Scheduling.Configuration.Schedules
{
    /// <summary>
    /// A collection for <see cref="ScheduleElementBase"/> instances loaded from a scheduler configuration section.
    /// </summary>
    /// <remarks>This class is intended to be used by the <see cref="SchedulerSettings"/> class</remarks>
    public class ScheduleElementCollection : PolymorphicElementCollection<SchedulerSettingsManager.SerializationStateProvider>
    {
        private static readonly Dictionary<string, Type> _elementTypeMap;

        static ScheduleElementCollection()
        {
            // TODO: create a configurable ScheduleConfigTypeMap collection: SchedulerSettings.ScheduleElementTypeMap
            _elementTypeMap = new Dictionary<string, Type>(5)
                                  {
                                      {"schedule", typeof (ScheduleElement)},
                                      {"IntervalSchedule", typeof (IntervalScheduleElement)},
                                      {"IntervalCountedSchedule", typeof (IntervalCountedScheduleElement)},
                                      {"WeeklySchedule", typeof (WeeklyScheduleElement)},
                                      {"OneShotSchedule", typeof (OneShotScheduleElement)}
                                  };
        }

        /// <summary>
        /// Return a <see cref="ScheduleElementBase"/> class that implements the specified element name.
        /// </summary>
        /// <remarks>The type is used in <see cref="ConfigurationElementCollection.CreateNewElement(string)"/>.</remarks>
        /// <param name="elementName">Name of the element to get the type for.</param>
        /// <returns>The <see cref="ConfigurationElement"/> type to use for <paramref name="elementName"/>.</returns>
        protected override Type GetElementType(string elementName)
        {
            Type type;
            if (_elementTypeMap.TryGetValue(elementName, out type))
                return type;
            return null;
        }

        /// <summary>
        /// Return <see cref="ScheduleElementBase"/> by its index in the collection.
        /// </summary>
        /// <param name="index">The index of the element to retrieve</param>
        public ScheduleElementBase this[int index]
        {
            get
            {
                return (ScheduleElementBase)BaseGet(index);
            }
            set
            {
                BaseAdd(index, value);
            }
        }

        /// <summary>
        /// Return <see cref="ScheduleElementBase"/> by its name.
        /// </summary>
        /// <param name="name">The name of the element to retrieve</param>
        /// <returns>The configuration element with the specified name; otherwise <c>null</c></returns>
        public ScheduleElementBase GetScheduleElementByName(string name)
        {
            return (ScheduleElementBase)BaseGet(name);
        }

        /// <summary>
        /// Returns true if the specified element name is a valid name for this collection type.
        /// </summary>
        /// <param name="elementName">Name of the element as read from the configuration file.</param>
        /// <returns>true if the name is valid; otherwise, false.</returns>
        protected override bool IsElementName(string elementName)
        {
            if (base.IsElementName(elementName))
                return true;

            if (String.IsNullOrEmpty(elementName))
                return false;

            return _elementTypeMap.ContainsKey(elementName);
        }

        /// <summary>
        /// Collection type is <see cref="ConfigurationElementCollectionType.BasicMapAlternate"/>.
        /// </summary>
        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.BasicMap; }
        }

        /// <summary>
        /// Add the specified <see cref="ScheduleElementBase"/> to this collection. If an element with the same name 
        /// already exist the old element will be replaced.
        /// </summary>
        /// <param name="schedule">The schedule element to add.</param>
        public void Add(ScheduleElementBase schedule)
        {
            BaseAdd(schedule, true);
        }

        /// <summary>
        /// Removes the <see cref="ScheduleElementBase"/> at the specified index location.
        /// </summary>
        /// <param name="index">The index location of the <see cref="ScheduleElementBase"/> to remove.</param>
        public void Remove(int index)
        {
            BaseRemoveAt(index);
        }

        /// <summary>
        /// Removes the <see cref="ScheduleElementBase"/> with the specified name.
        /// </summary>
        /// <param name="name">The name of the <see cref="ScheduleElementBase"/> to remove.</param>
        public void Remove(string name)
        {
            if (!BaseIsRemoved(name))
                BaseRemove(name);
        }

        /// <summary>
        /// Return the default <see cref="ScheduleElementBase"/> class.
        /// </summary>
        /// <remarks>This type is used in <see cref="ConfigurationElementCollection.CreateNewElement()"/>.</remarks>
        /// <returns>The <see cref="SchedulerElement"/> type.</returns>
        protected override Type GetDefaultElementType()
        {
            return typeof(ScheduleElement);
        }
    }
}