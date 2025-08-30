using System;
using DemiCode.Logging;
using DemiCode.Scheduling.Alarms;

namespace DemiCode.Scheduling.Test.Mocks
{
    /// <summary>
    /// A basic implementation of <seealso cref="IAlarmTimer"/> that enables simulation of triggered
    /// alarms. The alarm is configured with an <see cref="IAlarmTimer.AlarmTime"/> and will fire
    /// the <see cref="IAlarmTimer.Elapsed"/> event immediately upon calling <see cref="Enable"/>.
    /// </summary>
    internal class AlarmTimerMock: IAlarmTimer
    {
        #region Privates

        private bool _enabled = false;
        private DateTime _alarmTime;
        private object _additionalData;
        private ILogger _logger = null;
        #endregion

        #region IAlarmTimer Members

        /// <summary>
        /// Set a logger object to use for diagnostics.
        /// </summary>
        public void SetLogger(ILogger logger)
        {
            if (logger == null)
                throw new ArgumentNullException("logger");
 
            _logger = logger;
        }

        public object AdditionalData
        {
            get
            {
                return _additionalData;
            }
            set
            {
                _additionalData = value;
            }
        }

        public DateTime AlarmTime
        {
            get
            {
                return _alarmTime;
            }
            set
            {
                _alarmTime = value;
            }
        }

        public void Disable()
        {
            _enabled = false;
        }

        public event EventHandler<AlarmEventArgs> Elapsed;

        public bool Enable(DateTime alarmTime)
        {
            _alarmTime = alarmTime;
            _enabled = true;
            return true;
        }

        public void FireEvent()
        {
            var eventArgs = new AlarmEventArgs(_alarmTime, _alarmTime, _additionalData);
            EventHelper.RaiseEvents(this, Elapsed, eventArgs);
            //_enabled = false;
        }

        public bool Enable()
        {
            return Enable(_alarmTime);
        }

        public bool Enabled
        {
            get
            {
                return _enabled;
            }
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

        #endregion
    }
}
