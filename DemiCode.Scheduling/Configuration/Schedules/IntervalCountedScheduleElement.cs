using System;
using System.Configuration;
using DemiCode.Scheduling.Schedules;
using DemiCode.Configuration;

namespace DemiCode.Scheduling.Configuration.Schedules
{
    /// <summary>
    /// Strong typed class containing a <see cref="DemiCode.Scheduling.Schedules.IntervalCountedSchedule"/> configuration element read from a configuration file.
    /// </summary>
    /// <remarks>This class is intended to be used by the <see cref="SchedulerElement"/> class</remarks>
    public class IntervalCountedScheduleElement : IntervalScheduleElement
    {
        private const string _elementName = "IntervalCountedSchedule";

        /// <summary>
        /// Construct a new <see cref="IntervalCountedScheduleElement"/> instance.
        /// </summary>
        public IntervalCountedScheduleElement()
            : base(_elementName)
        {
        }

        /// <summary>
        /// Construct a new <see cref="IntervalCountedScheduleElement"/> instance.
        /// </summary>
        /// <param name="name">The name of this Schedule.</param>
        /// <param name="firstOccurrence">First occurence</param>
        /// <param name="interval">Interval between occurences. Minimum 1 second resolution.</param>
        /// <param name="occurrences">Number of occurrences.</param>
        public IntervalCountedScheduleElement(string name, DateTime firstOccurrence, TimeSpan interval, long occurrences)
            : base(_elementName, name, firstOccurrence, interval)
        {
            Occurrences.Value = occurrences;
        }

        /// <summary>
        /// The number of occurences.
        /// </summary>
        [ConfigurationProperty("occurrences", IsRequired = true, IsKey = false)]
        public LongElement Occurrences
        {
            get { return ((LongElement)base["occurrences"]); }
            set { base["occurrences"] = value; }
        }

        /// <summary>
        /// Get an instance of the schedule type.
        /// </summary>
        /// <returns>A <see cref="IntervalCountedSchedule"/> instance.</returns>
        public override ISchedule GetScheduleInstance()
        {
            return new IntervalCountedSchedule(Name, FirstOccurrence.Value, Occurrences.Value, Interval.Value);
        }

    }
}
