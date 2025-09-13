using System;

namespace DemiCode.Scheduling
{
    /// <summary>
    /// DateTime extension methods that simplify date calculations.
    /// </summary>
    internal static class DateTimeHelper
    {
        /// <summary>
        /// Calculates the number of months between two DateTime instances.
        /// The day of the month is ignored.
        /// </summary>
        public static int GetMonthDifference(this DateTime start, DateTime end)
        {
            if (start > end) throw new ArgumentException("Start date must be earlier than end date.");

            var sdt = new DateTime(start.Year, start.Month, 1);
            var edt = new DateTime(end.Year, end.Month, 1);

            int monthsApart = ((edt.Year - sdt.Year) * 12) + edt.Month - sdt.Month;
            return monthsApart;
        }

        /// <summary>
        /// Return the next 1st of month from the specified date
        /// </summary>
        public static DateTime GetFirstOfMonth(this DateTime date)
        {
            var nextMonth = date.AddMonths(1);
            return new DateTime(nextMonth.Year, nextMonth.Month, 1, 0, 0, 0, 0);
        }
    }
}