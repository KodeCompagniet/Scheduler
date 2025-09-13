using System;

namespace DemiCode.Scheduling.Time
{
    internal static class Dto
    {
        /// <summary>
        /// Creates a DateTimeOffset object with the specified parameters.
        /// </summary>
        public static DateTimeOffset At(int year, int month = 1, int day = 1, int hour = 0, int minute = 0, int second = 0) =>
            new DateTimeOffset(new DateTime(year, month, day, hour, minute, second), TimeSpan.Zero);
    }
}