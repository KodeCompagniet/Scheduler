using System;

namespace DemiCode.Scheduling
{
    /// <summary>
    /// DateTime extension methods that simplify date calculations.
    /// </summary>
    internal static class DateTimeHelper
    {
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