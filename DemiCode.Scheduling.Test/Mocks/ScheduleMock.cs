using System;
using System.Xml;
using DemiCode.Scheduling.Schedules;

namespace DemiCode.Scheduling.Test.Mocks
{
    public class ScheduleMock : ScheduleBase
    {
        public ScheduleMock(string name) : base(name)
        {
        }

        public ScheduleMock(XmlNode config)
            : base(config)
        {
        }

        /// <summary>
        /// Changes <see cref="CurrentOccurrence"/> to the first possible occurence from the supplied time.
        /// </summary>
        /// <param name="time">The time to calculate from.</param>
        /// <returns>True if new time found, false if schedule is ended.</returns>
        public override bool MoveToFirstFrom(DateTime time)
        {
            CurrentOccurrence = time.AddSeconds(1);
            return true;
        }
    }
}
