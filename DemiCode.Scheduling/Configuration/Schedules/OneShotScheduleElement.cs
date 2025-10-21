using System;
using System.Configuration;
using DemiCode.Scheduling.Schedules;
using DemiCode.Configuration;

namespace DemiCode.Scheduling.Configuration.Schedules
{
    /// <summary>
    /// Strong typed class containing a <see cref="DemiCode.Scheduling.Schedules.OneShotSchedule"/> configuration element read from a configuration file.
    /// </summary>
    /// <remarks>This class is intended to be used by the <see cref="SchedulerElement"/> class</remarks>
    public class OneShotScheduleElement : ScheduleElementBase
    {
        private const string _elementName = "OneShotSchedule";

        /// <summary>
        /// Construct a new <see cref="OneShotScheduleElement"/> instance.
        /// </summary>
        public OneShotScheduleElement()
            : base(_elementName)
        {
        }

        /// <summary>
        /// Construct a new <see cref="OneShotScheduleElement"/> instance.
        /// </summary>
        /// <param name="name">The name of this Schedule.</param>
        /// <param name="time">First occurrence.</param>
        public OneShotScheduleElement(string name, DateTime time)
            : base(_elementName, name)
        {
            Time.Value = time;
        }

        /// <summary>
        /// The date/time.
        /// </summary>
        [ConfigurationProperty("time", IsRequired = true, IsKey = false)]
        public DateTimeElement Time
        {
            get { return ((DateTimeElement)base["time"]); }
            set { base["time"] = value; }
        }

        /// <summary>
        /// Get an instance of the schedule type.
        /// </summary>
        /// <returns>A <see cref="OneShotSchedule"/> instance.</returns>
        public override ISchedule GetScheduleInstance()
        {
            return new OneShotSchedule(Name, Time.Value);
        }
    }
}
