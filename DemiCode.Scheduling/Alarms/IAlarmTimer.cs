using System;
using DemiCode.Logging;

namespace DemiCode.Scheduling.Alarms
{

    /// <summary>
    /// Common interface for alarm timers used by <see cref="Scheduler"/>. 
    /// An alarm is configured with a time when the alarm will fire the <see cref="Elapsed"/> event.
    /// </summary>
    public interface IAlarmTimer
    {
        /// <summary>
        /// Set a logger object to use for diagnostics.
        /// </summary>
        /// <param name="logger">The logger to assign to the alarm timer.</param>
        void SetLogger(ILogger logger);

        /// <summary>
		/// Gets or sets User-defined data associated with the object.
        /// </summary>
        object AdditionalData { get; set; }

        /// <summary>
        /// Gets or sets the DateTime that this alarm is set to.
        /// </summary>
        DateTime AlarmTime { get; set; }

        /// <summary>
        /// Turns off the alarm.
        /// </summary>
        void Disable();

        /// <summary>
        /// Occurs when the alarm "rings".
        /// </summary>
        event EventHandler<AlarmEventArgs> Elapsed;

        /// <summary>
        /// Turns on the alarm, to make it signal at the specified time.
        /// </summary>
        /// <param name="alarmTime">A DateTime specifying when the alarm should signal.</param>
        /// <returns>True if the alarm was set, false if not (e.g. if <paramref name="alarmTime"/> is in the past).</returns>
        bool Enable(DateTime alarmTime);

        /// <summary>
        /// Turns on the alarm.
        /// </summary>
        /// <returns>True if the alarm was set, false if not (e.g. if <see cref="AlarmTime"/> is in the past).</returns>
        bool Enable();

        /// <summary>
        /// Gets or sets a value indicating wether the DemiCode.Scheduling.AlarmTimer should raise the DemiCode.Scheduling.AlarmTimer.Elapsed event.
        /// </summary>
        bool Enabled { get; set; }
    }
}
