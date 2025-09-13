using System;

namespace DemiCode.Scheduling.Time
{
    internal static class Dt
    {
        /// <summary>
        /// Creates a DateTime object with the specified parameters.
        /// </summary>
        public static DateTime At(int year, int month = 1, int day = 1, int hour = 0, int minute = 0, int second = 0) =>
            new DateTime(year, month, day, hour, minute, second);
    }
}