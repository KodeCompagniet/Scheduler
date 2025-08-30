using System;
using System.Xml;

namespace DemiCode.Scheduling.Schedules
{
    /// <summary>
    /// A Schedule of one time on a specific date.
    /// </summary>
    /// <seealso cref="ISchedule"/>
    /// <seealso cref="ScheduleBase"/>
    /// <seealso cref="Scheduler"/>
    public class OneShotSchedule : ScheduleBase
    {
        #region ctors
        /// <summary>
        /// Creates a OneShotSchedule object
        /// </summary>
        /// <param name="name">The name of this Schedule.</param>
        /// <param name="time">The date and time of the one-shot.</param>
        /// <param name="additionalData">User-defined data associated with the object.</param>
        public OneShotSchedule(string name, DateTime time, object additionalData)
            : base(name, time, additionalData)
        {
        }

        /// <summary>
        /// Creates a OneShotSchedule object
        /// </summary>
        /// <param name="time">The date and time of the one-shot.</param>
        /// <param name="additionalData">User-defined data associated with the object.</param>
        public OneShotSchedule(DateTime time, object additionalData)
            : this(String.Empty, time, additionalData)
        {
        }

        /// <summary>
        /// Creates a OneShotSchedule object
        /// </summary>
        /// <param name="name">The name of this Schedule.</param>
        /// <param name="time">The date and time of the one-shot.</param>
        public OneShotSchedule(string name, DateTime time)
            : this(name, time, null)
        {
        }

        /// <summary>
        /// Creates a OneShotSchedule object
        /// </summary>
        /// <param name="time">The date and time of the one-shot.</param>
        public OneShotSchedule(DateTime time)
            : this(String.Empty, time, null)
        {
        }

        /// <summary>
        /// Creates a OneShotScheduler object from an XmlNode
        /// </summary>
        /// <param name="config">An XmlNode representing a OneShotSchedule</param>
        [Obsolete("Deprecated. Use configuration system instead.")]
        public OneShotSchedule(XmlNode config)
            : base(config)
        {
            DateTime firstOccurrence;
            TestAndSetElementValue(config, "time", out firstOccurrence, DateTime.TryParse);
            FirstOccurrence = firstOccurrence;
        }
        #endregion

        /// <summary>
        /// Changes <see cref="ISchedule.CurrentOccurrence"/> to the first posible occurence on or after the supplied DateTime
        /// </summary>
        /// <param name="time">The DateTime that you want to find an occurence after.</param>
        /// <returns>True if there is an occurence after the suplied Datetime, false if last occurence has passed before the supplied DateTime</returns>
        public override bool MoveToFirstFrom(DateTime time)
        {
            if (time > FirstOccurrence)
            {
                CurrentOccurrence = DateTime.MaxValue;
                return false;
            }
    
            CurrentOccurrence = FirstOccurrence;
            return true;
        }
    }
}
