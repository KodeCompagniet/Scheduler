using System;
using System.Xml;
using System.Xml.XPath;
using System.Globalization;

namespace DemiCode.Scheduling.Schedules
{
    /// <summary>
    /// Implementation of a Schedule that will signal at n times at a given interval from
    /// a specified starttime
    /// </summary>
    /// <seealso cref="ISchedule"/>
    /// <seealso cref="ScheduleBase"/>
    /// <seealso cref="Scheduler"/>
    public class IntervalCountedSchedule : ScheduleBase
    {
        private readonly TimeSpan _interval;
        private readonly long _numberOfOccurences;
        private long _remainingOccurences;

        /// <summary>
        /// Gets the interval between occurences
        /// </summary>
        public TimeSpan Interval
        {
            get { return _interval; }
        }

        /// <summary>
        /// Gets the number of remaining occurences
        /// </summary>
        public long RemainingOccurences
        {
            get { return _remainingOccurences; }
        }

        #region ctors
        /// <summary>
        /// Creates an IntervalCountedSchedule object.
        /// </summary>
        /// <param name="name">The name of this Schedule.</param>
        /// <param name="firstOccurence">First occurence</param>
        /// <param name="numberOfOccurrences">Number of occurences. Set to -1 for indefinitely</param>
        /// <param name="interval">Interval between occurences. Minimum 1 second resolution.</param>
        /// <param name="additionalData">User-defined data associated with the object.</param>
        public IntervalCountedSchedule(string name, DateTime firstOccurence, long numberOfOccurrences, TimeSpan interval, object additionalData)
            : base(name, firstOccurence, additionalData)
        {
            if (interval.Ticks <= 0)
                throw new ArgumentException("Argument 'interval' can't be an empty or negative timespan.", "interval");

            if (interval.TotalSeconds < 1)
                throw new ArgumentException("Argument 'interval' cannot be a timespan less than 1 second.", "interval");

            if ((numberOfOccurrences == 0) || (numberOfOccurrences < -1))
                throw new ArgumentException("Argument 'numberOfOccurrences' must be a positive integer or -1 for indefinitly.", "numberOfOccurrences");

            _numberOfOccurences = numberOfOccurrences;
            _interval = interval;
        }

        /// <summary>
        /// Creates an IntervalCountedSchedule object.
        /// </summary>
        /// <param name="firstOccurence">First occurence</param>
        /// <param name="numberOfOccurrences">Number of occurences. Set to -1 for indefinitely</param>
        /// <param name="interval">Interval between occurences. Minimum 1 second resolution.</param>
        /// <param name="additionalData">User-defined data associated with the object.</param>
        public IntervalCountedSchedule(DateTime firstOccurence, long numberOfOccurrences, TimeSpan interval, object additionalData)
            : this(String.Empty, firstOccurence, numberOfOccurrences, interval, additionalData)
        {
        }

        /// <summary>
        /// Creates an IntervalCountedSchedule object.
        /// </summary>
        /// <param name="firstOccurence">First occurence</param>
        /// <param name="numberOfOccurences">Number of occurences. Set to -1 for indefinitely</param>
        /// <param name="interval">Interval between occurences. Minimum 1 second resolution.</param>
        public IntervalCountedSchedule(DateTime firstOccurence, long numberOfOccurences, TimeSpan interval)
            : this(String.Empty, firstOccurence, numberOfOccurences, interval, null)
        {
        }

        /// <summary>
        /// Creates an IntervalCountedSchedule object.
        /// </summary>
        /// <param name="name">The name of this Schedule.</param>
        /// <param name="firstOccurence">First occurence</param>
        /// <param name="numberOfOccurences">Number of occurences. Set to -1 for indefinitely</param>
        /// <param name="interval">Interval between occurences. Minimum 1 second resolution.</param>
        public IntervalCountedSchedule(string name, DateTime firstOccurence, long numberOfOccurences, TimeSpan interval)
            : this(name, firstOccurence, numberOfOccurences, interval, null)
        {
        }

        /// <summary>
        /// Creates a IntervalCountedSchedule object from xml configuration.
        /// </summary>
        /// <param name="config">An XmlNode representing a IntervalCountedSchedule</param>
        [Obsolete("Deprecated. Use configuration system instead.")]
        public IntervalCountedSchedule(XmlNode config)
            : base(config)
        {
            FirstOccurrence = ParseElementValue<DateTime>(config, "start", DateTime.TryParse);
            _numberOfOccurences = ParseElementValue<long>(config, "occurrences", long.TryParse);
            TimeSpan interval = ParseElementValue<TimeSpan>(config, "interval", TimeSpan.TryParse);

            if (interval.Ticks <= 0)
                throw new ArgumentException("Argument 'interval' can't be an empty or negative timespan.", "interval");

            if (interval.TotalSeconds < 1)
                throw new ArgumentException("Argument 'interval' cannot be a timespan less than 1 second.", "interval");

            _interval = interval;
        }

        #endregion

        /// <summary>
        /// Changes <see cref="ISchedule.CurrentOccurrence"/> to the next occurence
        /// </summary>
        /// <returns>true if there is a next occurence, false if not</returns>
        public override bool MoveToNext()
        {
            if (_remainingOccurences > 1)
            {
                _remainingOccurences--;
                CurrentOccurrence += _interval;
                return true;
            }
            else
            {
                _remainingOccurences = 0;
                CurrentOccurrence = DateTime.MaxValue;
                return false;
            }
        }

        /// <summary>
        /// Moves the current occurrence to the nearest occurence on or after a specified date and time
        /// </summary>
        /// <param name="time">The DateTime to start from</param>
        /// <returns>true if OK, false if there are no occurences on or after the specified date and time</returns>
        public override bool MoveToFirstFrom(DateTime time)
        {
            time = time.AddTicks(1L);
            if (time <= FirstOccurrence)
            {
                _remainingOccurences = _numberOfOccurences;
                CurrentOccurrence = FirstOccurrence;
                return true;
            }

            long passedOccurences = ((time.Ticks - 1L) - FirstOccurrence.Ticks) / Interval.Ticks;
            passedOccurences++;

            if (_numberOfOccurences == -1)
            {
                _remainingOccurences = -1;
                CurrentOccurrence = FirstOccurrence + TimeSpan.FromTicks(passedOccurences * _interval.Ticks);
                return true;
            }

            if (passedOccurences < _numberOfOccurences)
            {
                _remainingOccurences = _numberOfOccurences - passedOccurences;
                CurrentOccurrence = FirstOccurrence + TimeSpan.FromTicks(passedOccurences * _interval.Ticks);
                return true;
            }

            _remainingOccurences = 0L;
            CurrentOccurrence = DateTime.MaxValue;

            return false;
        }

        /// <summary>
        /// Creates a string representation of this object
        /// </summary>
        /// <returns>A string</returns>
        public override string ToString()
        {
            return GetType() + "(\"" + Name + "\")" +
                "\nCurrentOccurence = " + CurrentOccurrence.ToString(CultureInfo.InvariantCulture) +
                ", RemainingOccurrences = " + RemainingOccurences +
                "\nFirstOccurence = " + FirstOccurrence.ToString(CultureInfo.InvariantCulture) +
                "\nNumberOfOccurences = " + _numberOfOccurences +
                "\nInterval = " + Interval.Days.ToString(CultureInfo.InvariantCulture) +
                " days, " + Interval.Hours.ToString("00", CultureInfo.InvariantCulture) +
                ":" + Interval.Minutes.ToString("00", CultureInfo.InvariantCulture) +
                ":" + Interval.Seconds.ToString("00", CultureInfo.InvariantCulture) +
                "." + Interval.Milliseconds.ToString("000", CultureInfo.InvariantCulture);
        }
    }
}
