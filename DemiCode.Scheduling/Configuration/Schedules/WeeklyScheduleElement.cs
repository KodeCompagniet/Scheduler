using System;
using System.Configuration;
using DemiCode.Scheduling.Schedules;
using DemiCode.Configuration;

namespace DemiCode.Scheduling.Configuration.Schedules
{
    /// <summary>
    /// Strong typed class containing a <see cref="DemiCode.Scheduling.Schedules.WeeklySchedule"/> configuration element read from a configuration file.
    /// </summary>
    /// <remarks>This class is intended to be used by the <see cref="SchedulerElement"/> class</remarks>
    public class WeeklyScheduleElement : ScheduleElementBase, IEquatable<WeeklyScheduleElement>
    {
        private const string _elementName = "WeeklySchedule";

        /// <summary>
        /// Construct a new <see cref="WeeklyScheduleElement"/> instance.
        /// </summary>
        public WeeklyScheduleElement()
            : base(_elementName)
        {
            FromDate.Value = DateTime.MinValue;
            ToDate.Value = DateTime.MinValue;
        }

        /// <summary>
        /// Construct a new <see cref="WeeklyScheduleElement"/> instance.
        /// </summary>
        /// <param name="name">The name of this schedule.</param>
        /// <param name="fromDate">The starting date for the schedule. The TimeOfDay part of this DateTime is ignored.</param>
        /// <param name="toDate">The ending date for the schedule. The TimeOfDay part of this DateTime is ignored.</param>
        /// <param name="timeOfDay">The time of day of schedule</param>
        /// <param name="days">A collection of DayOfWeek values, telling the days of the schedule.</param>
        public WeeklyScheduleElement(string name, DateTime fromDate, DateTime toDate, TimeSpan timeOfDay, DayOfWeek[] days)
            : base(_elementName, name)
        {
            FromDate.Value = fromDate;
            ToDate.Value = toDate;
            TimeOfDay.Value = timeOfDay;
            Days.Value = days;
        }

        /// <summary>
        /// Construct a new <see cref="WeeklyScheduleElement"/> instance.
        /// </summary>
        /// <param name="name">The name of this schedule.</param>
        /// <param name="timeOfDay">The time of day of schedule</param>
        /// <param name="days">A collection of DayOfWeek values, telling the days of the schedule.</param>
        public WeeklyScheduleElement(string name, TimeSpan timeOfDay, DayOfWeek[] days)
            : this(name, DateTime.MinValue, DateTime.MinValue, timeOfDay, days)
        {
        }

        /// <summary>
        /// Construct a new <see cref="WeeklyScheduleElement"/> instance.
        /// </summary>
        /// <param name="name">The name of this schedule.</param>
        /// <param name="fromDate">The starting date for the schedule. The TimeOfDay part of this DateTime is ignored.</param>
        /// <param name="timeOfDay">The time of day of schedule</param>
        /// <param name="days">A collection of DayOfWeek values, telling the days of the schedule.</param>
        public WeeklyScheduleElement(string name, DateTime fromDate, TimeSpan timeOfDay, DayOfWeek[] days)
            : this(name, fromDate, DateTime.MinValue, timeOfDay, days)
        {
        }

        /// <summary>
        /// The start date.
        /// </summary>
        [ConfigurationProperty("fromDate", IsRequired = true, IsKey = false)]
        public DateElement FromDate
        {
            get { return (DateElement)base["fromDate"]; }
            set { base["fromDate"] = value; }
        }

        /// <summary>
        /// The end date.
        /// </summary>
        [ConfigurationProperty("toDate", IsRequired = false, IsKey = false)]
        public DateElement ToDate
        {
            get { return (DateElement)base["toDate"]; }
            set { base["toDate"] = value; }
        }

        /// <summary>
        /// The time of day.
        /// </summary>
        [ConfigurationProperty("timeOfDay", IsRequired = true, IsKey = false)]
        public TimeSpanElement TimeOfDay
        {
            get { return (TimeSpanElement)base["timeOfDay"]; }
            set { base["timeOfDay"] = value; }
        }

        /// <summary>
        /// The days.
        /// </summary>
        [ConfigurationProperty("days", IsRequired = true, IsKey = false)]
        public DaysElement Days
        {
            get { return (DaysElement)base["days"]; }
            set { base["days"] = value; }
        }

        /// <summary>
        /// Get a <see cref="WeeklySchedule"/> instance.
        /// </summary>
        /// <returns>A <see cref="WeeklySchedule"/> instance.</returns>
        public override ISchedule GetScheduleInstance()
        {
            var fromDate = (FromDate.Value == DateTime.MinValue ? DateTime.MinValue : FromDate.Value);
            var toDate = (ToDate.Value == DateTime.MinValue ? DateTime.MaxValue : ToDate.Value);
            return new WeeklySchedule(Name, fromDate, toDate, TimeOfDay.Value, Days.Value);
        }

        ///<summary>
        ///Indicates whether the current object is equal to another object of the same type.
        ///</summary>
        ///
        ///<returns>
        ///true if the current object is equal to the other parameter; otherwise, false.
        ///</returns>
        ///
        ///<param name="weeklyScheduleElement">An object to compare with this object.</param>
        public bool Equals(WeeklyScheduleElement weeklyScheduleElement)
        {
            if (weeklyScheduleElement == null) return false;
            // Our base will compare all properties
            return base.Equals(weeklyScheduleElement);
        }

        ///<summary>
        ///Compares the current <see cref="T:System.Configuration.ConfigurationElement"></see> instance to the specified object.
        ///</summary>
        ///
        ///<returns>
        ///true if the object to compare with is equal to the current <see cref="T:System.Configuration.ConfigurationElement"></see> instance; otherwise, false. The default is false. 
        ///</returns>
        ///
        ///<param name="compareTo">The object to compare with.</param>
        public override bool Equals(object compareTo)
        {
            if (ReferenceEquals(this, compareTo)) return true;
            return Equals(compareTo as WeeklyScheduleElement);
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
