using System;
using System.Xml;
using System.Xml.XPath;
using System.Globalization;

namespace DemiCode.Scheduling.Schedules
{
    /// <summary>
    /// Implementation of a Schedule that will signal infinite times at a given interval from
    /// a specified starttime.
    /// </summary>
    /// <seealso cref="ISchedule"/>
    /// <seealso cref="ScheduleBase"/>
    /// <seealso cref="Scheduler"/>
    public class IntervalSchedule : ScheduleBase
    {
        private readonly TimeSpan _interval;

        /// <summary>
        /// Gets the interval between occurences
        /// </summary>
        public TimeSpan Interval
        {
            get { return _interval; }
        }

        #region ctors
        /// <summary>
        /// Creates an IntervalSchedule object.
        /// </summary>
        /// <param name="name">The name of this Schedule.</param>
        /// <param name="firstOccurence">First occurence</param>
        /// <param name="interval">Interval between occurences. Minimum 1 second resolution.</param>
        /// <param name="additionalData">User-defined data associated with the object.</param>
        public IntervalSchedule(string name, DateTime firstOccurence, TimeSpan interval, object additionalData)
            : base(name, firstOccurence, additionalData)
        {
            if (interval.Ticks <= 0)
                throw new ArgumentException("Argument 'interval' can't be an empty or negative timespan.", nameof(interval));

            if (interval.TotalSeconds < 1)
                throw new ArgumentException("Argument 'interval' cannot be a timespan less than 1 second.", nameof(interval));

            _interval = interval;
        }

        /// <summary>
        /// Creates an IntervalSchedule object.
        /// </summary>
        /// <param name="firstOccurence">First occurence</param>
        /// <param name="interval">Interval between occurences. Minimum 1 second resolution.</param>
        /// <param name="additionalData">User-defined data associated with the object.</param>
        public IntervalSchedule(DateTime firstOccurence, TimeSpan interval, object additionalData)
            : this(String.Empty, firstOccurence, interval, additionalData)
        {
        }

        /// <summary>
        /// Creates an IntervalSchedule object.
        /// </summary>
        /// <param name="firstOccurence">First occurence</param>
        /// <param name="interval">Interval between occurences. Minimum 1 second resolution.</param>
        public IntervalSchedule(DateTime firstOccurence, TimeSpan interval)
            : this(String.Empty, firstOccurence, interval, null)
        {
        }

        /// <summary>
        /// Creates an IntervalSchedule object.
        /// </summary>
        /// <param name="name">The name of this Schedule.</param>
        /// <param name="firstOccurence">First occurence</param>
        /// <param name="interval">Interval between occurences. Minimum 1 second resolution.</param>
        public IntervalSchedule(string name, DateTime firstOccurence, TimeSpan interval)
            : this(name, firstOccurence, interval, null)
        {
        }

        /// <summary>
        /// Creates a IntervalSchedule object from xml configuration.
        /// </summary>
        /// <param name="config">An XmlNode representing a IntervalSchedule</param>
        [Obsolete("Deprecated. Use configuration system instead.")]
        public IntervalSchedule(XmlNode config)
            : base(config)
        {
            FirstOccurrence = ParseElementValue<DateTime>(config, "start", DateTime.TryParse);
            TimeSpan interval = ParseElementValue<TimeSpan>(config, "interval", TimeSpan.TryParse);

            if (interval.Ticks <= 0)
                throw new ArgumentException("Argument 'interval' can't be an empty or negative timespan.", nameof(interval));

            if (interval.TotalSeconds < 1)
                throw new ArgumentException("Argument 'interval' cannot be a timespan less than 1 second.", nameof(interval));

            _interval = interval;
        }

        #endregion

        /// <summary>
        /// Changes <see cref="ISchedule.CurrentOccurrence"/> to the next occurence
        /// </summary>
        /// <returns>true if there is a next occurence, false if not</returns>
        public override bool MoveToNext()
        {
            CurrentOccurrence += _interval;
            return true;
        }

        /// <summary>
        /// Moves the current occurrence to the nearest occurence on or after a specified date and time
        /// </summary>
        /// <param name="time">The DateTime to start from</param>
        /// <returns>true if OK, false if there are no occurences on or after the specified date and time</returns>
        public override bool MoveToFirstFrom(DateTime time)
        {
            // Add one tick to get the correct number of "passed occurences" ticks
            time = time.AddTicks(1L);
            if (time <= FirstOccurrence)
            {
                CurrentOccurrence = FirstOccurrence;
            }
            else
            {
                long passedOccurences = ((time.Ticks - 1L) - FirstOccurrence.Ticks) / Interval.Ticks;
                passedOccurences++;

                CurrentOccurrence = FirstOccurrence.AddTicks(passedOccurences * _interval.Ticks);
            }
            return true;
        }

        /// <summary>
        /// Creates a string representation of this object
        /// </summary>
        /// <returns>A string</returns>
        public override string ToString()
        {
            return String.Format(CultureInfo.InvariantCulture, "{0} (\"{1}\")" +
                          "\nCurrentOccurrence = {2:yyyyMMdd hh:mm:ss:fff}" +
                          "\nFirstOccurrence {3:yyyyMMdd hh:mm:ss:fff}" +
                          "\nInterval = {4}", GetType(), Name, CurrentOccurrence, FirstOccurrence, Interval);
        }
    }
}
