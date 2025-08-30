using System;
using System.Configuration;
using DemiCode.Scheduling.Schedules;
using DemiCode.Configuration;

namespace DemiCode.Scheduling.Configuration.Schedules
{
    /// <summary>
    /// Strong typed class containing a <see cref="DemiCode.Scheduling.Schedules.IntervalSchedule"/> configuration element read from a configuration file.
    /// </summary>
    /// <remarks>This class is intended to be used by the <see cref="SchedulerElement"/> class</remarks>
    public class IntervalScheduleElement : ScheduleElementBase
    {
        private const string _elementName = "IntervalSchedule";

        /// <summary>
        /// Construct a new <see cref="IntervalScheduleElement"/> instance.
        /// </summary>
        public IntervalScheduleElement()
            : base(_elementName)
        {
        }

        /// <summary>
        /// Construct a new <see cref="IntervalScheduleElement"/> instance.
        /// </summary>
        /// <param name="name">The name of this Schedule.</param>
        /// <param name="firstOccurrence">First occurence</param>
        /// <param name="interval">Interval between occurences. Minimum 1 second resolution.</param>
        public IntervalScheduleElement(string name, DateTime firstOccurrence, TimeSpan interval)
            : this(_elementName, name, firstOccurrence, interval)
        {
        }

        /// <summary>
        /// Construct a new <see cref="IntervalScheduleElement"/> instance.
        /// </summary>
        /// <param name="elementName">The name of the configuration element.</param>
        protected IntervalScheduleElement(string elementName)
            : base(elementName)
        {
        }

        /// <summary>
        /// Construct a new <see cref="IntervalScheduleElement"/> instance.
        /// </summary>
        /// <param name="elementName">The name of the configuration element.</param>
        /// <param name="name">The name of this Schedule.</param>
        /// <param name="firstOccurence">First occurence</param>
        /// <param name="interval">Interval between occurences. Minimum 1 second resolution.</param>
        protected IntervalScheduleElement(string elementName, string name, DateTime firstOccurence, TimeSpan interval)
            : base(elementName, name)
        {
            FirstOccurrence.Value = firstOccurence;
            Interval.Value = interval;
        }

        /// <summary>
        /// The start date/time.
        /// </summary>
        [ConfigurationProperty("start", IsRequired = true, IsKey = false)]
        public DateTimeElement FirstOccurrence
        {
            get { return ((DateTimeElement)base["start"]); }
            set { base["start"] = value; }
        }

        /// <summary>
        /// The interval timespan.
        /// </summary>
        [ConfigurationProperty("interval", IsRequired = true, IsKey = false)]
        public TimeSpanElement Interval
        {
            get { return ((TimeSpanElement)base["interval"]); }
            set { base["interval"] = value; }
        }

        /// <summary>
        /// Get an instance of the schedule type.
        /// </summary>
        /// <returns>A <see cref="IntervalSchedule"/> instance.</returns>
        public override ISchedule GetScheduleInstance()
        {
            return new IntervalSchedule(Name, FirstOccurrence.Value, Interval.Value);
        }
    }
}
