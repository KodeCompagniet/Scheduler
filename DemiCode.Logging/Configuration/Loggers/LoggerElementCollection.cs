using System;
using System.Configuration;
using DemiCode.Shared.Configuration;

namespace DemiCode.Logging.Configuration.Loggers
{
    /// <summary>
    /// A collection for <see cref="LoggerElementBase"/> instances loaded from a scheduler/jobs configuration section.
    /// </summary>
    /// <typeparam name="V">A type that implements a class that will indicate wether configuration is serialized or not.</typeparam>
    public class LoggerElementCollection<V> : PolymorphicElementCollection<V> where V : ISerializationStateProvider, new()
    {
        //private ConfigurationElement _parent;

        /////<summary>
        /////Resets the <see cref="T:System.Configuration.ConfigurationElementCollection"></see> to its unmodified state when overridden in a derived class.
        /////</summary>
        /////
        /////<param name="parentElement">The <see cref="T:System.Configuration.ConfigurationElement"></see> representing the collection parent element, if any; otherwise, null. </param>
        //protected override void Reset(ConfigurationElement parentElement)
        //{
        //    base.Reset(parentElement);
        //    _parent = parentElement;
        //}

        /// <summary>
        /// Return <see cref="LoggerElementBase"/> by its index in the collection.
        /// </summary>
        /// <param name="index">The index of the element to retrieve</param>
        public LoggerElementBase this[int index]
        {
            get
            {
                return (LoggerElementBase)BaseGet(index);
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

            return (GetElementType(elementName) != null);
        }

        /// <summary>
        /// Collection type is <see cref="ConfigurationElementCollectionType.BasicMapAlternate"/>.
        /// </summary>
        public override ConfigurationElementCollectionType CollectionType
        {
            get { return ConfigurationElementCollectionType.BasicMap; }
        }

        /// <summary>
        /// Add the specified <see cref="LoggerElementBase"/> to this collection. If an element with the same name 
        /// already exist the old element will be replaced.
        /// </summary>
        public void Add(LoggerElementBase logger)
        {
            base.BaseAdd(logger);
        }

        /// <summary>
        /// Removes the <see cref="LoggerElementBase"/> at the specified index location.
        /// </summary>
        /// <param name="index">The index location of the <see cref="LoggerElementBase"/> to remove.</param>
        public void Remove(int index)
        {
            BaseRemoveAt(index);
        }

        /// <summary>
        /// Removes the <see cref="LoggerElementBase"/> with the specified name.
        /// </summary>
        /// <param name="name">The name of the <see cref="LoggerElementBase"/> to remove.</param>
        public void Remove(string name)
        {
            if (!BaseIsRemoved(name))
                BaseRemove(name);
        }

        /// <summary>
        /// Override in subclass to return the <see cref="ConfigurationElement"/> class that implements the specified element name.
        /// </summary>
        protected override Type GetElementType(string elementName)
        {
            switch (elementName)
            {
                case "logger":
                    return typeof (LoggerElement);

                case "ConsoleLogger":
                    return typeof (ConsoleLoggerElement);

                case "Log4netLogger":
                    return
                        Type.GetType(
                            "DemiCode.Logging.Configuration.Loggers.log4net.Log4netLoggerElement, DemiCode.Logging.log4net", true);
            }

            return null;
        }

        /// <summary>
        /// Override in subclass to return the default <see cref="ConfigurationElement"/> class.
        /// </summary>
        /// <remarks>This type is used in <see cref="ConfigurationElementCollection.CreateNewElement()"/>.</remarks>
        protected override Type GetDefaultElementType()
        {
            return typeof (LoggerElement);
        }
    }
}
