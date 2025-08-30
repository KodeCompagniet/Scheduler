using System;
using System.Collections.Generic;
using System.Text;
using DemiCode.Scheduling.Alarms;

namespace DemiCode.Scheduling.Schedules
{
	/// <summary>
    /// The interface for a schedule.
	/// </summary>
	/// <remarks>
    /// <b>Note to implementors:</b> The CompareTo methods of an ISchedule should compare the CurrentOccurrence of two ISchedules.
	/// </remarks>
	public interface ISchedule : IComparable<ISchedule>
	{
        /// <summary>
        /// The name of this schedule.
        /// </summary>
        string Name { get;}

		/// <summary>
		/// The first occurrence of this schedule.
		/// </summary>
		DateTime FirstOccurrence { get;}

		/// <summary>
		/// The current occurrence of this schedule.
		/// </summary>
		DateTime CurrentOccurrence { get;}

		/// <summary>
		/// User-defined data associated with the object.
		/// </summary>
		object AdditionalData { get;set;}

		/// <summary>
		/// Fires when this ISchedule occurs.
		/// </summary>
		event EventHandler<AlarmEventArgs> Alarm;
		
		/// <summary>
		/// Fires when this ISchedule is over.
		/// </summary>
		event EventHandler<EventArgs> EndOfSchedule;

		/// <summary>
        /// Changes <see cref="CurrentOccurrence"/> to the first possible occurence from the supplied time.
		/// </summary>
		/// <param name="time">The time to start from.</param>
		/// <returns>True if new time found, false if schedule is ended.</returns>
		bool MoveToFirstFrom(DateTime time);

		/// <summary>
        /// Changes <see cref="CurrentOccurrence"/> to the next occurence
		/// </summary>
		/// <returns>True if there is a next occurence, false if not.</returns>
		bool MoveToNext();

		/// <summary>
		/// Signal that this ISchedule occurs now.
		/// </summary>
        /// <param name="actualSignalTime">The actual signal time as reported by the underlying timer.</param>
        void ItsTime(DateTime actualSignalTime);

		/// <summary>
		/// Signal that this ISchedule is over.
		/// </summary>
		void ItsOver();
	}
}
