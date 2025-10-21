using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Xml;
using System.Globalization;

namespace DemiCode.Scheduling.Schedules
{
    /// <summary>
    /// A Schedule of one time on certain weekdays.
    /// </summary>
    /// <seealso cref="ISchedule"/>
    /// <seealso cref="ScheduleBase"/>
    /// <seealso cref="Scheduler"/>
    public class WeeklySchedule : ScheduleBase
    {
        private readonly DateTime _fromDate;
        private readonly DateTime _toDate;
        private readonly TimeSpan _timeOfDay;
        private readonly List<DayOfWeek> _days;

        /// <summary>
        /// Get the starting date for the schedule.
        /// </summary>
        public DateTime FromDate { get { return _fromDate; } }

        /// <summary>
        /// Get the ending date for the schedule.
        /// </summary>
        public DateTime ToDate { get { return _toDate; } }

        /// <summary>
        /// Get the time of day of schedule.
        /// </summary>
        public TimeSpan TimeOfDay { get { return _timeOfDay; } }

        /// <summary>
        /// Get the list of DayOfWeek values, telling the days of the schedule.
        /// </summary>
        public ReadOnlyCollection<DayOfWeek> Days { get { return _days.AsReadOnly(); } }

        #region ctors
        /// <summary>
        /// Creates a WeeklySchedule object.
        /// </summary>
        /// <param name="name">The name of this Schedule.</param>
        /// <param name="fromDate">The starting date for the schedule. The TimeOfDay part of this DateTime is ignored.</param>
        /// <param name="toDate">The ending date for the schedule. The TimeOfDay part of this DateTime is ignored.</param>
        /// <param name="timeOfDay">The time of day of schedule</param>
        /// <param name="days">A collection of DayOfWeek values, telling the days of the schedule.</param>
        /// <param name="additionalData">User-defined data associated with the object.</param>
        public WeeklySchedule(string name, DateTime fromDate, DateTime toDate, TimeSpan timeOfDay, IEnumerable<DayOfWeek> days, object additionalData)
            : base(name, fromDate, additionalData)
        {
            if (days == null)
                throw new ArgumentNullException("days", "Days array cannot be null");

            if (fromDate.Date >= toDate.Date)
                throw new ArgumentException("Argument 'fromDate' must be a date before argument 'toDate'.", "fromDate");

            var theDays = new List<DayOfWeek>(days);
            if (theDays.Count <= 0)
                throw new ArgumentException("Argument 'days' must contain at least one DayOfWeek.", "days");

            theDays.Sort();

            for (int i = 1; i < theDays.Count; i++)
            {
                if (theDays[i] == theDays[i - 1])
                    throw new ArgumentException("Argument 'days' can't contain duplicates.");
            }

            _fromDate = fromDate;
            _toDate = toDate;
            _timeOfDay = timeOfDay;
            _days = theDays;

            //NOTE: Gives error CA2214, call to virtual method within constructor.
            //      Could be solved by moving to a Create method.
            MoveToFirstFrom(fromDate.Date);
            FirstOccurrence = CurrentOccurrence;
        }

        /// <summary>
        /// Creates a WeeklySchedule object.
        /// </summary>
        /// <param name="fromDate">The starting date for the schedule. The TimeOfDay part of this DateTime is ignored.</param>
        /// <param name="toDate">The ending date for the schedule. The TimeOfDay part of this DateTime is ignored.</param>
        /// <param name="timeOfDay">The time of day of schedule</param>
        /// <param name="days">A collection of DayOfWeek values, telling the days of the schedule.</param>
        /// <param name="additionalData">User-defined data associated with the object.</param>
        public WeeklySchedule(DateTime fromDate, DateTime toDate, TimeSpan timeOfDay, IEnumerable<DayOfWeek> days, object additionalData)
            : this(String.Empty, fromDate, toDate, timeOfDay, days, additionalData)
        {
        }

        /// <summary>
        /// Creates a WeeklySchedule object.
        /// </summary>
        /// <param name="fromDate">The starting date for the schedule. The TimeOfDay part of this DateTime is ignored.</param>
        /// <param name="timeOfDay">The time of day of schedule</param>
        /// <param name="days">A collection of DayOfWeek values, telling the days of the schedule.</param>
        public WeeklySchedule(DateTime fromDate, TimeSpan timeOfDay, IEnumerable<DayOfWeek> days)
            : this(String.Empty, fromDate, DateTime.MaxValue, timeOfDay, days, null)
        {
        }

        /// <summary>
        /// Creates a WeeklySchedule object.
        /// </summary>
        /// <param name="name">The name of this Schedule.</param>
        /// <param name="fromDate">The starting date for the schedule. The TimeOfDay part of this DateTime is ignored.</param>
        /// <param name="timeOfDay">The time of day of schedule</param>
        /// <param name="days">A collection of DayOfWeek values, telling the days of the schedule.</param>
        public WeeklySchedule(string name, DateTime fromDate, TimeSpan timeOfDay, IEnumerable<DayOfWeek> days)
            : this(name, fromDate, DateTime.MaxValue, timeOfDay, days, null)
        {
        }

        /// <summary>
        /// Creates a WeeklySchedule object.
        /// </summary>
        /// <param name="fromDate">The starting date for the schedule. The TimeOfDay part of this DateTime is ignored.</param>
        /// <param name="toDate">The ending date for the schedule. The TimeOfDay part of this DateTime is ignored.</param>
        /// <param name="timeOfDay">The time of day of schedule</param>
        /// <param name="days">A collection of DayOfWeek values, telling the days of the schedule.</param>
        public WeeklySchedule(DateTime fromDate, DateTime toDate, TimeSpan timeOfDay, IEnumerable<DayOfWeek> days)
            : this(String.Empty, fromDate, toDate, timeOfDay, days, null)
        {
        }

        /// <summary>
        /// Creates a WeeklySchedule object.
        /// </summary>
        /// <param name="name">The name of this Schedule.</param>
        /// <param name="fromDate">The starting date for the schedule. The TimeOfDay part of this DateTime is ignored.</param>
        /// <param name="toDate">The ending date for the schedule. The TimeOfDay part of this DateTime is ignored.</param>
        /// <param name="timeOfDay">The time of day of schedule</param>
        /// <param name="days">A collection of DayOfWeek values, telling the days of the schedule.</param>
        public WeeklySchedule(string name, DateTime fromDate, DateTime toDate, TimeSpan timeOfDay, IEnumerable<DayOfWeek> days)
            : this(name, fromDate, toDate, timeOfDay, days, null)
        {
        }

        /// <summary>
        /// Creates a WeeklySchedule object.
        /// </summary>
        /// <remarks>
        /// A WeeklySchedule created using this constructor has no starting or ending date, and
        /// will start immediately.
        /// </remarks>
        /// <param name="timeOfDay">The time of day of schedule</param>
        /// <param name="days">A collection of DayOfWeek values, telling the days of the schedule.</param>
        /// <param name="additionalData">User-defined data associated with the object.</param>
        public WeeklySchedule(TimeSpan timeOfDay, IEnumerable<DayOfWeek> days, object additionalData)
            : this(String.Empty, DateTime.MinValue, DateTime.MaxValue, timeOfDay, days, additionalData)
        {
        }

        /// <summary>
        /// Creates a WeeklySchedule object.
        /// </summary>
        /// <remarks>
        /// A WeeklySchedule created using this constructor has no starting or ending date, and
        /// will start immediately.
        /// </remarks>
        /// <param name="name">The name of this Schedule.</param>
        /// <param name="timeOfDay">The time of day of schedule</param>
        /// <param name="days">A collection of DayOfWeek values, telling the days of the schedule.</param>
        /// <param name="additionalData">User-defined data associated with the object.</param>
        public WeeklySchedule(string name, TimeSpan timeOfDay, IEnumerable<DayOfWeek> days, object additionalData)
            : this(name, DateTime.MinValue, DateTime.MaxValue, timeOfDay, days, additionalData)
        {
        }

        /// <summary>
        /// Creates a WeeklySchedule object.
        /// </summary>
        /// <remarks>
        /// A WeeklySchedule created using this constructor has no starting or ending date, and
        /// will start immediately.
        /// </remarks>
        /// <param name="timeOfDay">The time of day of schedule</param>
        /// <param name="days">A collection of DayOfWeek values, telling the days of the schedule.</param>
        public WeeklySchedule(TimeSpan timeOfDay, IEnumerable<DayOfWeek> days)
            : this(timeOfDay, days, null)
        {
        }

        /// <summary>
        /// Creates a WeeklySchedule object.
        /// </summary>
        /// <remarks>
        /// A WeeklySchedule created using this constructor has no starting or ending date, and
        /// will start immediately.
        /// </remarks>
        /// <param name="name">The name of this Schedule.</param>
        /// <param name="timeOfDay">The time of day of schedule</param>
        /// <param name="days">A collection of DayOfWeek values, telling the days of the schedule.</param>
        public WeeklySchedule(string name, TimeSpan timeOfDay, IEnumerable<DayOfWeek> days)
            : this(name, timeOfDay, days, null)
        {
        }

        /// <summary>
        /// Creates a WeeklySchedule object from an XmlNode
        /// </summary>
        /// <param name="config">An XmlNode representing a WeeklySchedule</param>
        [Obsolete("Deprecated. Use configuration system instead.")]
        public WeeklySchedule(XmlNode config)
            : base(config)
        {
            TestAndSetElementValue(config, "fromDate", out _fromDate, DateTime.TryParse);
            TestAndSetElementValue(config, "toDate", out _toDate, DateTime.TryParse, DateTime.MaxValue);
            TestAndSetElementValue(config, "timeOfDay", out _timeOfDay, TimeSpan.TryParse);

            _days = new List<DayOfWeek>();
            XmlNodeList dayNodes = config.SelectNodes("days/day");
            if (dayNodes == null || dayNodes.Count == 0)
            {
                throw new ArgumentException("The 'days' element collection is empty or doesn't exist.", "config");
            }

            foreach (XmlNode day in dayNodes)
            {
                string dayOfWeek = day.InnerXml;

                _days.Add((DayOfWeek)Enum.Parse(typeof(DayOfWeek), dayOfWeek, true));
            }
        }
        #endregion

        /// <summary>
        /// Changes <see cref="ISchedule.CurrentOccurrence"/> to the first possible occurence from the supplied time.
        /// </summary>
        /// <param name="time">The time to calculate from.</param>
        /// <returns>True if new time found, false if schedule is ended.</returns>
        public override bool MoveToFirstFrom(DateTime time)
        {
            if (time.Date < _fromDate)
            {
                time = _fromDate;
            }

            while (((!_days.Contains(time.DayOfWeek)) || (time.TimeOfDay > _timeOfDay)) && (time.Date <= _toDate))
            {
                time = time.Date.AddDays(1);
            }

            if (time.Date <= _toDate)
            {
                CurrentOccurrence = time.Date.Add(_timeOfDay);
                return true;
            }

            CurrentOccurrence = DateTime.MaxValue;

            return false;
        }

        /// <summary>
        /// Creates a string representation of the current object.
        /// </summary>
        /// <returns>A string</returns>
        public override string ToString()
        {
            string strDays = "";

            foreach (DayOfWeek d in _days)
            {
                if (strDays.Length != 0)
                {
                    strDays += ", ";
                }

                strDays += d;
            }

            return String.Format(CultureInfo.InvariantCulture, "{0}(\"{4}\")\nCurrentOccurence = {1}\nTimeOfDay = {2:hh\\:mm\\:ss}\nDays = {3}",
                GetType(), CurrentOccurrence, _timeOfDay, strDays, Name);
        }
    }
}
