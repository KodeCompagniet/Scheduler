using System;
using System.Timers;

namespace DemiCode.Scheduling.Alarms
{
	/// <summary>
	/// Provides data for the <see cref="DemiCode.Scheduling.Alarms.AlarmTimer.Elapsed"/> event.
	/// </summary>
    /// <seealso cref="DemiCode.Scheduling.Alarms.AlarmTimer"/>
	public class AlarmEventArgs : EventArgs
	{
	    /// <summary>
		/// User-defined data associated with the object.
		/// </summary>
		private object _additionalData;

	    /// <summary>
	    /// Gets the time the <see cref="DemiCode.Scheduling.Alarms.AlarmTimer.Elapsed"/> 
	    /// event was raised as configured in the schedule.
	    /// </summary>
	    public DateTime SignalTime { get; private set; }

	    /// <summary>
	    /// Gets the time the <see cref="DemiCode.Scheduling.Alarms.AlarmTimer.Elapsed"/> 
	    /// event was raised as reported by the underlying <see cref="IAlarmTimer"/>.
	    /// </summary>
	    /// <remarks>
	    /// The time reported by the underlying <see cref="IAlarmTimer"/> could be different
	    /// than <see cref="SignalTime"/> due to delays in the timer software or hardware.
	    /// </remarks>
	    public DateTime ActualSignalTime { get; set; }

	    /// <summary>
		/// User-defined data associated with the object.
		/// </summary>
		public object AdditionalData
		{
			get { return _additionalData; }
			set { _additionalData = value; }
		}

        /// <summary>
        /// Convert this instance to text.
        /// </summary>
        /// <returns>A descriptive text of the argument data.</returns>
        public override string ToString()
        {
            return
                String.Format(
                    "AlarmEventArgs SignalTime:{0:yyyyMMdd hh:mm:ss:fff} ActualSignalTime:{1:yyyyMMdd hh:mm:ss:fff}",
                    SignalTime, ActualSignalTime);
        }
        
		#region ctors
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="signalTime">The time the alarm was configured to go off.</param>
        /// <param name="actualSignalTime">The actual time the alarm went off.</param>
        /// <param name="additionalData">User-defined data associated with the object.</param>
        public AlarmEventArgs(DateTime signalTime, DateTime actualSignalTime, object additionalData)
		{
			SignalTime = signalTime;
            ActualSignalTime = actualSignalTime;
			_additionalData = additionalData;
		}


		/// <summary>
		/// This is the copy constructor
		/// </summary>
		/// <param name="e">The AlarmEventArgs structure to be copied</param>
		public AlarmEventArgs(AlarmEventArgs e)
		{
            if (e == null)
                throw new ArgumentNullException("e");
            SignalTime = e.SignalTime;
            ActualSignalTime = e.ActualSignalTime;
            _additionalData = e.AdditionalData;
		}
		#endregion
	}
}
