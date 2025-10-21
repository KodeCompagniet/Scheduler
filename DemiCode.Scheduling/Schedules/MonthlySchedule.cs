using System;
using System.Xml;
using System.Globalization;

namespace DemiCode.Scheduling.Schedules
{
    /*
* Must support the following scenarios:
* 
*  1. the X day of every Y month
*  2. the n'th named day of every Y month
*  3. 1. with occurence once at T
*  4. 1. with occurence every Z unit starting at T1, ending at T2
*  5. 2. with occurence once at T
*  6. 2. with occurence every Z unit starting at T1, ending at T2
*  
* X : 
*      numeric (31 >= X > 0)
* Y : 
*      numeric (Y > 0)
* Z :
*      numeric. range depends on unit
* unit :
*      minute or hour
* n : 
*      'first', 'second', 'third', 'forth' or 'last'
* named day : 
*      monday, tuesday, wednesday, thursday, friday, saturday, sunday, day, weekday or weekend day
* T, T1, T2 :
*      time of day
* 
* */

    internal class MonthlySchedule : ScheduleBase
    {
        private readonly DateTime _fromDate;
        private readonly DateTime _toDate;

        #region ctors

        /// <summary>
		/// Creates a MonthlySchedule object.
		/// </summary>
		/// <param name="fromDate">The starting date for the schedule. The TimeOfDay part of this DateTime is ignored.</param>
		/// <param name="toDate">The ending date for the schedule. The TimeOfDay part of this DateTime is ignored.</param>
        public MonthlySchedule(DateTime fromDate, DateTime toDate)
            : this(fromDate, toDate, null)
        {
        }

        /// <summary>
        /// Creates a MonthlySchedule object.
        /// </summary>
        /// <param name="fromDate">The starting date for the schedule. The TimeOfDay part of this DateTime is ignored.</param>
        /// <param name="toDate">The ending date for the schedule. The TimeOfDay part of this DateTime is ignored.</param>
        /// <param name="additionalData">User-defined data associated with the object.</param>
        public MonthlySchedule(DateTime fromDate, DateTime toDate, object additionalData)
            : base(string.Format("Monthly_{0:yyyy-MM-dd}_to_{1:yyyy-MM-dd}", fromDate, toDate), fromDate, additionalData)
        {
            if (fromDate.Date >= toDate.Date)
                throw new ArgumentException("Argument 'fromDate' must be a date before argument 'toDate'.", "fromDate");

            _fromDate = fromDate;
            _toDate = toDate;

            //NOTE: Gives error CA2214, call to virtual method within constructor.
            //      Could be solved by moving to a Create method.
            //MoveToFirstFrom(fromDate.Date);
            //FirstOccurrence = CurrentOccurrence;
        }

        /// <summary>
        /// Creates a MonthlySchedule object from an XmlNode
        /// </summary>
        /// <param name="node">An XmlNode representing a MonthlySchedule</param>
        public MonthlySchedule(XmlNode node)
            : base(node)
        {
            TestAndSetElementValue(node, "fromDate", out _fromDate, DateTime.TryParse);
            TestAndSetElementValue(node, "toDate", out _toDate, DateTime.TryParse);
        }

        #endregion

        /// <summary>
        /// Changes <see cref="ISchedule.CurrentOccurrence"/> to the first possible occurence from the supplied time.
        /// </summary>
        /// <returns>True if new time found, false if schedule is ended.</returns>
        public override bool MoveToFirstFrom(DateTime time)
        {
            if (time.Date < _fromDate)
            {
                CurrentOccurrence = _fromDate;
                return true;
            }

            // Are we within the schedule's range?
            if (time.Date <= _toDate)
            {
                // Calculate the target month's date from the schedule's starting month
                var monthDifference = DateTimeHelper.GetMonthDifference(_fromDate, time.Date);
                var dt = _fromDate.AddMonths(monthDifference);

                // If we are behind the monthly day, move to that day
                if (time.Date < dt)
                {
                    // Check if dt exceeds toDate boundary
                    if (dt.Date > _toDate)
                    {
                        CurrentOccurrence = DateTime.MaxValue;
                        return false;
                    }
                    CurrentOccurrence = dt;
                }
                else
                {
                    // Otherwise, skip to the next month
                    var nextOccurrence = _fromDate.AddMonths(monthDifference + 1);

                    // Check if next occurrence exceeds toDate boundary
                    if (nextOccurrence.Date > _toDate)
                    {
                        CurrentOccurrence = DateTime.MaxValue;
                        return false;
                    }

                    CurrentOccurrence = nextOccurrence;
                }

                return true;
            }
            else
            {
                CurrentOccurrence = DateTime.MaxValue;
                return false;
            }
        }

        /// <summary>
        /// Creates a string representation of the current object.
        /// </summary>
        /// <returns>A string</returns>
        public override string ToString()
        {
            return String.Format(CultureInfo.InvariantCulture, "{0}\nCurrentOccurence = {1}",
                this.GetType(), CurrentOccurrence);
        }
    }
}
