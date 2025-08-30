using System;
using System.Threading;
using DemiCode.Logging;

namespace DemiCode.Scheduling.Alarms
{
	/// <summary>
	/// A oneshot timer that will accept a DateTime to decide when to fire.
	/// </summary>
	public class AlarmTimer : IDisposable, IAlarmTimer
	{
	    private const int MaxInterval = Int32.MaxValue - 1;

		/// <summary>
		/// Occurs when the alarm "rings".
		/// </summary>
		public event EventHandler<AlarmEventArgs> Elapsed;

		private bool _disposed;// = false;
		private Timer _timer;
		private DateTime _alarmTime;
		private object _additionalData;
	    private ILogger _logger;

        /// <summary>
        /// Set a logger object to use for diagnostics.
        /// </summary>
        /// <param name="logger">The logger to assign to the alarm timer.</param>
        public void SetLogger(ILogger logger)
        {
            if (logger == null)
                throw new ArgumentNullException("logger");

            _logger = logger;
        }

		/// <summary>
		/// Gets or sets the DateTime that this alarm is set to.
		/// </summary>
		public DateTime AlarmTime
		{
			get { return _alarmTime; }
			set
			{
				if (_disposed)
				{
					throw new ObjectDisposedException("AlarmTimer");
				}

				if (_timer != null)
				{
					throw new InvalidOperationException("Can't set AlarmTime while alarm is on.");
				}

				_alarmTime = value;
			}
		}

		/// <summary>
        /// Gets or sets a value indicating wether the <see cref="DemiCode.Scheduling.Alarms.AlarmTimer"/> should raise the <see cref="DemiCode.Scheduling.Alarms.AlarmTimer.Elapsed"/> event.
		/// </summary>
		public bool Enabled
		{
			get { return _timer != null; }
			set
			{
				if (value)
				{
					Enable();
				}
				else
				{
					Disable();
				}
			}
		}

		/// <summary>
		/// Gets or sets user-defined data associated with the object.
		/// </summary>
		public object AdditionalData
		{
			get { return _additionalData; }
			set { _additionalData = value; }
		}

		#region ctors
		/// <summary>
        /// Creates an instance of the <see cref="AlarmTimer"/>, with no alarmtime specified.
		/// </summary>
		/// <remarks>
		/// The AlarmTime property must be set, and the the alarm must be turned on (with the TurnOn() method)
		/// before it does anything.
		/// </remarks>
		public AlarmTimer()
		{
		    _logger = ((ILoggerFactory)new NilLoggerFactory()).CreateLogger(typeof(AlarmTimer));
			_timer = null;
		}

		/// <summary>
        /// Creates an instance of the <see cref="AlarmTimer"/> with a preset alarmtime. The alarm is turned on.
		/// </summary>
		/// <param name="additionalData">User-defined data associated with the object.</param>
		public AlarmTimer(object additionalData)
			: this()
		{
			AdditionalData = additionalData;
		}
		#endregion

		#region Dispose and dtor stuff
		/// <summary>
		/// Destructor
		/// </summary>
		~AlarmTimer()
		{
			Dispose(false);
		}

		/// <summary>
		/// Disposes the AlarmTimer.
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				// If disposing equals true, dispose all managed 
				// and unmanaged resources.
				if (disposing)
				{
					// Dispose managed resources.
                    if (_timer != null)
                    {
                        _timer.Dispose();
                        _timer = null;
                    }
				}

				// Release unmanaged resources. If disposing is false, 
				// only the following code is executed.
			}

			_disposed = true;
		}
		#endregion

		/// <summary>
		/// Eventhandler for Elapsed event on _timer.
		/// </summary>
		/// <remarks>
		/// This event
		/// </remarks>
		private void mTimer_Elapsed(object state)
		{
		    var eventArgs = (AlarmEventArgs) state;
		    eventArgs.ActualSignalTime = DateTime.Now;
			EventHelper.RaiseEvents(this, Elapsed, eventArgs, _logger);
		}

	    /// <summary>
	    /// Turns on the alarm.
	    /// </summary>
	    /// <returns>True if the alarm was set, false if not (e.g. if <see cref="AlarmTime"/> is in the past).</returns>
	    public bool Enable()
		{
            var interval = GetIntervalFromNow(); 
            
            if (interval <= 0)
            {
                _logger.Debug("Alarm timer fired immediately (interval was {0})", interval);
                return false;
            }
            
            if (interval > MaxInterval)
			{
                _logger.Debug("Alarm timer will checkpoint-fire in {0} milliseconds", interval);
                _timer = CreateCheckpointTimer();
            }
			else
			{
                _logger.Debug("Alarm timer will fire in {0} milliseconds", interval);
                _timer = CreateElapsedTimer(interval);
			}

		    return true;
		}

	    private long GetIntervalFromNow()
	    {
	        return TicksToMilliseconds(_alarmTime.Ticks - DateTime.Now.Ticks);
	    }

	    private Timer CreateCheckpointTimer()
	    {
            return new Timer(mTimer_Checkpoint, null, MaxInterval, Timeout.Infinite);
	    }

	    private void mTimer_Checkpoint(object state)
	    {
	        var interval = GetIntervalFromNow();

            if (interval > MaxInterval)
            {
                _timer.Change(MaxInterval, Timeout.Infinite);
            }
            else
            {
                _timer.Dispose();

                _timer = CreateElapsedTimer(interval);
            }
	    }

	    private Timer CreateElapsedTimer(long interval)
	    {
	        var eventArgs = new AlarmEventArgs(_alarmTime, _alarmTime, _additionalData);
	        return new Timer(mTimer_Elapsed, eventArgs, interval, Timeout.Infinite);
	    }

	    /// <summary>
	    /// Turns on the alarm, to make it signal at the specified time.
	    /// </summary>
	    /// <param name="alarmTime">A DateTime specifying when the alarm should signal.</param>
	    /// <returns>True if the alarm was set, false if not (e.g. if <paramref name="alarmTime"/> is in the past).</returns>
	    public bool Enable(DateTime alarmTime)
		{
			_alarmTime = alarmTime;
			return Enable();
		}

		/// <summary>
		/// Turns off the alarm.
		/// </summary>
		public void Disable()
		{
            if (_timer != null)
            {
                _timer.Dispose();
                _timer = null;
            }
		}

		/// <summary>
		/// Converts Ticks to milliseconds.
		/// </summary>
		/// <param name="ticks">Number of ticks.</param>
		/// <returns>Number of milliseconds.</returns>
		internal static long TicksToMilliseconds(long ticks)
		{
		    return (long)TimeSpan.FromTicks(ticks).TotalMilliseconds;
		}
	}
}
