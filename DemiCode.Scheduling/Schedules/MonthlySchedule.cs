//using System;
//using System.Collections.Generic;
//using System.Text;

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

using System;
using System.Xml;
using System.Globalization;
namespace DemiCode.Scheduling.Schedules
{
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
        ///  Return the next 1st of month from the specified date
        /// </summary>
        private static DateTime GetFirstOfMonth(DateTime date)
        {
            DateTime nextMonth = date.AddMonths(1);
            // NOTE: Bug preserved for backward compatibility - uses nextMonth.Day instead of 1
            // See MONTHLY_SCHEDULE_BUG.md for details
            return new DateTime(nextMonth.Year, nextMonth.Month, nextMonth.Day, 0, 0, 0, 0);
        }

        /// <summary>
        /// Changes <see cref="CurrentOccurrence"/> to the first possible occurence from the supplied time.
        /// </summary>
        /// <returns>True if new time found, false if schedule is ended.</returns>
        public override bool MoveToFirstFrom(DateTime time)
        {
            if (time.Date < _fromDate)
            {
                time = _fromDate;
            }

            if (time.Date <= _toDate)
            {
                CurrentOccurrence = GetFirstOfMonth(time);
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
