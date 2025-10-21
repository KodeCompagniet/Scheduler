using System;
using System.Collections.Generic;
using System.Configuration;
using DemiCode.Logging.Configuration;
using DemiCode.Scheduling.Configuration.Schedules;
using DemiCode.Scheduling.Configuration.Tasks;
using DemiCode.Scheduling.Schedules;
using DemiCode.Configuration;

namespace DemiCode.Scheduling.Configuration
{
    /// <summary>
    /// Strong typed class for containing a scheduler element read from a configuration file.
    /// All <c>schedule</c> elements within the <c>scheduler</c> element will
    /// be loaded and exposed as a collection of <see cref="ISchedule"/> instances.
    /// </summary>
    /// <remarks>This class is intended to be used by the <see cref="SchedulerElementCollection"/> class</remarks>
    public class SchedulerElement : ScheduleElementCollection, IEquatable<SchedulerElement>
    {
        internal const string _ElementName = "scheduler";

        private readonly List<ISchedule> _schedules = new List<ISchedule>();

        /// <summary>
        /// Construct an empty <see cref="SchedulerElement"/> instance.
        /// </summary>
        public SchedulerElement()
        {
        }

        /// <summary>
        /// Construct a <see cref="SchedulerElement"/> instance.
        /// </summary>
        /// <param name="name">The name of the scheduler.</param>
        public SchedulerElement(string name)
        {
            Name = name;
        }

        /// <summary>
        /// A collection of <see cref="ISchedule"/> instances that can be used
        /// when constructing a <see cref="Scheduler"/> instance.
        /// </summary>
        [Obsolete("Use GetScheduleInstances", true)]
        public ISchedule[] Schedules
        {
            get { return _schedules.ToArray(); }
        }

        /// <summary>
        /// The name of our scheduler element. This attribute must be unique 
        /// within the list of schedulers.
        /// </summary>
        [ConfigurationProperty("name", IsRequired = true, IsKey = true)]
        public string Name
        {
            get { return ((string)base["name"]); }
            set { base["name"] = value; }
        }

        /// <summary>
        /// Gets the collection of <see cref="TaskElementBase"/> objects.
        /// </summary>
        /// <value>
        /// The collection of <see cref="TaskElementBase"/> objects.
        /// </value>
        [ConfigurationProperty("tasks", IsDefaultCollection = false, IsRequired = false)]
        [ConfigurationCollection(typeof(TaskElementCollection))]
        public TaskElementCollection Tasks
        {
            get { return (TaskElementCollection)base["tasks"]; }
        }

        /// <summary>
        /// Gets the collection of defined <see cref="LoggerElement"/> objects.
        /// </summary>
        /// <value>
        /// The collection of defined <see cref="LoggerElement"/> objects.
        /// </value>
        [ConfigurationProperty("loggers", IsDefaultCollection = false, IsRequired = false)]
        [ConfigurationCollection(typeof(LoggerElementCollection))]
        public LoggerElementCollection Loggers
        {
            get { return (LoggerElementCollection)base["loggers"]; }
        }

        /// <summary>
        /// Return a list of schedule instances based on our list of schedule configuration elements.
        /// </summary>
        /// <returns>A list of <see cref="ISchedule"/> instances.</returns>
        public List<ISchedule> GetScheduleInstances()
        {
            var list = new List<ISchedule>(Count);

            foreach (ScheduleElementBase element in this)
            {
                list.Add(element.GetScheduleInstance());
            }

            return list;
        }

        /// <summary>
        /// Called after deserialization.
        /// </summary>
        protected override void PostDeserialize()
        {
            base.PostDeserialize();

            Validate();
        }

        private void Validate()
        {
            //TODO: validate Loggers ?

            // Validate that tasks referencing schedules have valid reference names
            foreach (TaskElementBase taskElement in Tasks)
            {
                foreach (ScheduleReferenceElement scheduleReferenceElement in taskElement)
                {
                    if (GetScheduleElementByName(scheduleReferenceElement.ScheduleName) == null)
                    {
                        throw new InvalidConfigurationException(
                            String.Format("A schedule reference with name '{0}' points to an invalid schedule ",
                                          scheduleReferenceElement.ScheduleName));
                    }
                }
            }
        }

        ///<summary>
        ///Indicates whether the current object is equal to another object of the same type.
        ///</summary>
        ///
        ///<returns>
        ///true if the current object is equal to the other parameter; otherwise, false.
        ///</returns>
        ///
        ///<param name="schedulerElement">An object to compare with this object.</param>
        public bool Equals(SchedulerElement schedulerElement)
        {
            if (schedulerElement == null) return false;
            return base.Equals(schedulerElement);
        }

        ///<summary>
        ///Compares the <see cref="T:System.Configuration.ConfigurationElementCollection"></see> to the specified object.
        ///</summary>
        ///
        ///<returns>
        ///true if the object to compare with is equal to the current <see cref="T:System.Configuration.ConfigurationElementCollection"></see> instance; otherwise, false. The default is false.
        ///</returns>
        ///
        ///<param name="compareTo">The object to compare. </param>
        public override bool Equals(object compareTo)
        {
            if (ReferenceEquals(this, compareTo)) return true;
            return Equals(compareTo as SchedulerElement);
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