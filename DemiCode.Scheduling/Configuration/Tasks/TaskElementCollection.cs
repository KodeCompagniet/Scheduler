using System;
using System.Collections.Generic;
using System.Configuration;
using DemiCode.Configuration;

namespace DemiCode.Scheduling.Configuration.Tasks
{

    /// <summary>
    /// A collection for <see cref="TaskElementBase"/> instances loaded from a scheduler/tasks configuration section.
    /// </summary>
    /// <remarks>This class is intended to be used by the <see cref="SchedulerSettings"/> class</remarks>
    public class TaskElementCollection : PolymorphicElementCollection<SchedulerSettingsManager.SerializationStateProvider>
    {
        private static readonly Dictionary<string, Type> _elementTypeMap;

        static TaskElementCollection()
        {
            _elementTypeMap = new Dictionary<string, Type>(4)
                                  {
                                      {"task", typeof (TaskElement)},
                                      {"ApplicationTask", typeof (ApplicationTaskElement)},
                                      {"CodeTask", typeof (CodeTaskElement)},
                                      {"TaskSequence", typeof (TaskSequenceElement)}
                                  };
        }

        /// <summary>
        /// Return the <see cref="TaskElementBase"/> class that implements the specified element name.
        /// </summary>
        /// <remarks>The type is used in <see cref="ConfigurationElementCollection.CreateNewElement(string)"/>.</remarks>
        /// <param name="elementName">The element name to get the type for.</param>
        /// <returns>The <see cref="TaskElementBase"/> type to use for <paramref name="elementName"/>.</returns>
        protected override Type GetElementType(string elementName)
        {
            Type type;
            if (_elementTypeMap.TryGetValue(elementName, out type))
                return type;
            return null;
        }

        /// <summary>
        /// Return <see cref="TaskElementBase"/> by its index in the collection.
        /// </summary>
        /// <param name="index">The index of the element to retrieve</param>
        public TaskElementBase this[int index]
        {
            get
            {
                return (TaskElementBase)BaseGet(index);
            }
            set
            {
                BaseAdd(index, value);
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
        /// Add the specified <see cref="TaskElementBase"/> to this collection. If an element with the same name 
        /// already exist the old element will be replaced.
        /// </summary>
        /// <param name="task">The task element to add.</param>
        public void Add(TaskElementBase task)
        {
            base.BaseAdd(task);
        }

        /// <summary>
        /// Removes the <see cref="TaskElementBase"/> at the specified index location.
        /// </summary>
        /// <param name="index">The index location of the <see cref="TaskElementBase"/> to remove.</param>
        public void Remove(int index)
        {
            BaseRemoveAt(index);
        }

        /// <summary>
        /// Removes the <see cref="TaskElementBase"/> with the specified name.
        /// </summary>
        /// <param name="name">The name of the <see cref="TaskElementBase"/> to remove.</param>
        public void Remove(string name)
        {
            if (!BaseIsRemoved(name))
                BaseRemove(name);
        }

        /// <summary>
        /// Return the default <see cref="TaskElementBase"/> class.
        /// </summary>
        /// <remarks>This type is used in <see cref="ConfigurationElementCollection.CreateNewElement()"/>.</remarks>
        /// <returns>The default element type <see cref="TaskElement"/>.</returns>
        protected override Type GetDefaultElementType()
        {
            return typeof(TaskElement);
        }
    }
}
