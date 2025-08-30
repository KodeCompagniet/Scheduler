using System;
using DemiCode.Configuration;

namespace DemiCode.Configuration
{
    /// <summary>
    /// A configuration element for representing a list of <see cref="DayOfWeek"/> values.
    /// </summary>
    public class DaysElement : EnumElement<DayOfWeek>
    {
        /// <summary>
        /// Initialize a new instance of <see cref="DaysElement"/>.
        /// </summary>
        public DaysElement() : base("days", "day") { }

        /// <summary>
        /// Initialize a new instance of <see cref="DaysElement"/> with <see cref="EnumElement{T}.Value"/> set to <paramref name="days"/>.
        /// </summary>
        /// <param name="days">The value to use.</param>
        public DaysElement(DayOfWeek[] days) : base(days, "days", "day") { }
    }
}