using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Text;
using System.Xml;
using DemiCode.Scheduling.Alarms;
using DemiCode.Logging;
using DemiCode.Scheduling.Schedules;
using DemiCode.Scheduling.Tasks;
using DemiCode.Scheduling.Time;

namespace DemiCode.Scheduling
{

    /// <summary>
    /// This class will fire an event each time one of the schedules in the ISchedule
    /// collection occurs.
    /// </summary>
    public class Scheduler : IDisposable, IEnumerable<ISchedule>
    {
        #region Privates

        private bool _disposed; // = false;
        private bool _externalStopSignaled; // = false;
        private readonly object _processingEventsLock = new object();
        private AlarmTimer _managedTimer; // = null;
        private readonly IAlarmTimer _timer;
        private readonly ITimeProvider _timeProvider; // = null;
        private readonly List<ISchedule> _schedules;
        private readonly List<ISchedule> _schedulesDone = new List<ISchedule>(16);
        private DateTime _scheduleQueueOccurrence;
        private readonly List<ISchedule> _scheduleQueue = new List<ISchedule>(16);
        private ILogger _logger;
        private ILoggerFactory _loggerFactory;

        #endregion

        #region Public properties

        /// <summary>
        /// The tasks configured in this scheduler.
        /// </summary>
        public TaskCollection Tasks { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating wether the <see cref="DemiCode.Scheduling.Scheduler"/> should raise the <see cref="DemiCode.Scheduling.Scheduler.ScheduledAlarm"/> event.
        /// </summary>
        public bool Enabled
        {
            get { return _timer.Enabled; }
            set
            {
                if (value)
                {
                    Start();
                }
                else
                {
                    Stop();
                }
            }
        }

        /// <summary>
        /// Returns the number of Schedules actually contained in the Scheduler.
        /// </summary>
        public int Count { get { return _schedules.Count; } }

        /// <summary>
        /// A collection of <see cref="ILoggerFactory"/> implementations. 
        /// </summary>
        public LoggerFactoryCollection Loggers { get; private set; }

        #endregion

        #region Public events

        /// <summary>
        /// When there are no more events to fire, the EndOfSchedule event will fire. It means that all
        /// SecheduledItems are in the past.
        /// </summary>
        public event EventHandler<EventArgs> EndOfSchedule;

        /// <summary>
        /// This event fires each time an ISchedule's time occurs. 
        /// </summary>
        /// <remarks>
        /// This event fires whenever one of the schedules in this Scheduler occurs.
        /// If you need fine-grained schedule events, use the <see cref="ISchedule.Alarm"/> event on the
        /// individual schedule instances.
        /// </remarks>
        public event EventHandler<AlarmEventArgs> ScheduledAlarm;

        #endregion

        #region obsolete factory stuff
        /// <summary>
        /// Creates a Scheduler with schedules from the configuration file (.config).
        /// </summary>
        /// <param name="sectionName">The section name as defined in the .config files configSections/section</param>
        /// <returns>The default Scheduler configured in the configuration file.</returns>
        /// <seealso href="d9a6a49a-098b-4d25-aa79-eb78b4634e34.htm" target="_self">Scheduler configuration using .NET configuration files</seealso>
        [Obsolete("Use SchedulerFactory.CreateFromConfig")]
        public static Scheduler CreateFromConfig(string sectionName)
        {
            return SchedulerFactory.CreateFromConfig(sectionName);
        }

        /// <summary>
        /// Creates a Scheduler with schedules from the configuration file (.config).
        /// </summary>
        /// <param name="sectionName">The section name as defined in the .config files configSections/section</param>
        /// <param name="schedulerName">The named scheduler to create</param>
        /// <returns>The named Scheduler configured in the configuration file.</returns>
        /// <seealso href="d9a6a49a-098b-4d25-aa79-eb78b4634e34.htm" target="_self">Scheduler configuration using .NET configuration files</seealso>
        [Obsolete("Use SchedulerFactory.CreateFromConfig")]
        public static Scheduler CreateFromConfig(string sectionName, string schedulerName)
        {
            return SchedulerFactory.CreateFromConfig(sectionName, schedulerName);
        }

        #endregion

        #region Dispose and dtor stuff
        /// <summary>
        /// Destructor
        /// </summary>
        ~Scheduler()
        {
            Dispose(false);
        }

        /// <summary>
        /// Disposes the internal <see cref="AlarmTimer"/> resource.
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
                    StopInternal();

                    ClearSchedules();

                    DisconnectEventHandlers();

                    // Dispose managed resources.
                    if (_managedTimer != null)
                    {
                        _managedTimer.Dispose();
                        _managedTimer = null;
                    }

                    if (Tasks != null)
                    {
                        Tasks.Dispose();
                        Tasks = null;
                    }
                }

                // Release unmanaged resources. If disposing is false, 
                // only the following code is executed.
            }

            _disposed = true;
        }

        
        /// <summary>When disposing, we must clear out any schedules since these, with event handlers, will keep our scheduler going.</summary>
        /// <remarks>Case 7797</remarks>
        private void ClearSchedules()
        {
            _schedules.Clear();
        }

        /// <summary>
        /// When disposing our <see cref="Scheduler"/> we must also remove any event handlers since these will keep our scheduler going.
        /// </summary>
        /// <remarks>Case 7797</remarks>
        private void DisconnectEventHandlers()
        {
            var endOfSchedule = EndOfSchedule;
            if (endOfSchedule != null)
            {
                foreach (EventHandler<EventArgs> handler in EndOfSchedule.GetInvocationList())
                {
                    EndOfSchedule -= handler;
                }
            }

            var scheduledAlarm = ScheduledAlarm;
            if (scheduledAlarm != null)
            {
                foreach (EventHandler<AlarmEventArgs> handler in ScheduledAlarm.GetInvocationList())
                {
                    ScheduledAlarm -= handler;
                }
            }
        }

        #endregion

        #region Indexers

        /// <summary>
        /// Return a schedule by name.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the schedule name does not exist in this scheduler.</exception>
        /// <param name="scheduleName">Name of the schedule to retrieve.</param>
        /// <returns></returns>
        public ISchedule this[string scheduleName]
        {
            get
            {
                ISchedule schedule;
                if (!TryGetSchedule(scheduleName, out schedule))
                {
                    throw new ArgumentOutOfRangeException("scheduleName", scheduleName, "The specified schedule does not exist in this scheduler");
                }
                return schedule;
            }
        }

        /// <summary>
        /// Return a schedule by index.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if the schedule name does not exist in this scheduler.</exception>
        /// <param name="index">Index of the schedule to retrieve.</param>
        /// <returns></returns>
        public ISchedule this[int index]
        {
            get
            {
                if (index < 0 || index >= _schedules.Count)
                {
                    throw new ArgumentOutOfRangeException("index", index, "The specified index is out of range");
                }
                return _schedules[index];
            }
        }

        #endregion

        internal bool TryGetSchedule(string scheduleName, out ISchedule schedule)
        {
            for (int i = 0; i < _schedules.Count; i++)
            {
                if (_schedules[i].Name == scheduleName)
                {
                    schedule = _schedules[i];
                    return true;
                }
            }
            schedule = null;
            return false;
        }


        #region ctors

        /// <summary>
        /// Creates an empty Scheduler object.
        /// </summary>
        internal Scheduler()
            : this(new AlarmTimer(), null, null, null, null)
        {
        }

        /// <summary>
        /// Creates an empty Scheduler object configured with the specified alarm timer instance.
        /// </summary>
        internal Scheduler(IAlarmTimer alarmTimer)
            : this(null, alarmTimer, null, null, null)
        {
        }

        /// <summary>
        /// Creates an empty Scheduler object configured with the specified alarm timer instance.
        /// </summary>
        internal Scheduler(IAlarmTimer alarmTimer, ITimeProvider timeProvider)
            : this(null, alarmTimer, timeProvider, null, null)
        {
        }

        /// <summary>
        /// Creates an empty Scheduler object configured with the specified alarm timer instance.
        /// </summary>
        internal Scheduler(IAlarmTimer alarmTimer, ITimeProvider timeProvider, ILoggerFactory loggerFactory)
            : this(null, alarmTimer, timeProvider, loggerFactory, null)
        {
        }

        /// <summary>
        /// Creates an empty Scheduler object configured with the specified alarm timer instance.
        /// </summary>
        private Scheduler(AlarmTimer managedTimer, IAlarmTimer timer, ITimeProvider timeProvider, ILoggerFactory loggerFactory)
        {
            _managedTimer = managedTimer;
            _timer = timer ?? _managedTimer;
            _timeProvider = timeProvider ?? new DefaultTimeProvider();
            _schedules = new List<ISchedule>();
            Tasks = new TaskCollection(this);
            Loggers = new LoggerFactoryCollection();
            if (loggerFactory != null)
            {
                Loggers.Add(loggerFactory);
            }
            _loggerFactory = Loggers.GetDefaultLoggerFactory();
            _logger = _loggerFactory.CreateLogger(typeof(Scheduler));
            _timer.Elapsed += AlarmEventHandler;
        }

        /// <summary>
        /// Creates a Scheduler object initialized with a collection of schedules.
        /// </summary>
        /// <param name="schedules">A collection of ISchedule objects</param>
        internal Scheduler(IEnumerable<ISchedule> schedules)
            : this(schedules, null)
        { }
        /// <summary>
        /// Creates a Scheduler object initialized with a collection of schedules.
        /// </summary>
        /// <param name="schedules">A collection of ISchedule objects</param>
        internal Scheduler(IEnumerable<ISchedule> schedules)
        {
            AddRange(schedules);
        }

        /// <summary>
        /// Creates a Scheduler object.
        /// </summary>
        /// <param name="schedule">A schedule</param>
        internal Scheduler(ISchedule schedule)
            : this()
        {
            Add(schedule);
        }

        #endregion

        /// <summary>
        /// Adds a schedule to the scheduler
        /// </summary>
        /// <remarks>
        /// It is not possible to add schedules to the scheduler while it is running.
        /// Attempts to add a schedule while the scheduler is running will result in an
        /// InvalidOperationException.
        /// </remarks>
        /// <param name="schedule">The schedule to add to the scheduler</param>
        public void Add(ISchedule schedule)
        {
            if (_timer.Enabled)
            {
                throw new InvalidOperationException("Can't add ISchedule to this Scheduler while this Scheduler is enabled.");
            }

            _schedules.Add(schedule);
        }

        /// <summary>
        /// Adds a collection of schedules to the scheduler
        /// </summary>
        /// <remarks>
        /// It is not possible to add schedules to the scheduler while it is running.
        /// Attempts to add a schedule while the scheduler is running will result in an
        /// InvalidOperationException.
        /// </remarks>
        /// <param name="schedules">The schedules to add to the scheduler</param>
        public void AddRange(IEnumerable<ISchedule> schedules)
        {
            if (_timer.Enabled)
            {
                throw new InvalidOperationException("Can't add ISchedules to this Scheduler while this Scheduler is enabled.");
            }

            _schedules.AddRange(schedules);
        }

        /// <summary>
        /// Sorts all schedules on the first occurrence from parameter time, removing those that have no more occurrences.
        /// </summary>
        private void Prepare(DateTime time)
        {
            for (var i = _schedules.Count - 1; i >= 0; i--)
            {
                var schedule = _schedules[i];
                if (schedule.MoveToFirstFrom(time)) continue;

                schedule.ItsOver();
                _schedulesDone.Add(schedule);
                _schedules.RemoveAt(i);
            }

            _schedules.Sort((left, right) => left.CurrentOccurrence.CompareTo(right.CurrentOccurrence));
            _logger.Debug("Current queue at {1:yyyyMMdd hh:mm:ss:fff}:\n\n{0}\n\n", this, time);
        }

        private void QueueSchedules()
        {
            _scheduleQueue.Clear();
            if (_schedules.Count == 0)
                return;

            _scheduleQueue.Add(_schedules[0]);
            _scheduleQueueOccurrence = RoundToNearestSecond(_schedules[0].CurrentOccurrence);
            for (int i = 1; i < _schedules.Count; i++)
            {
                var occurrence = RoundToNearestSecond(_schedules[i].CurrentOccurrence);
                if (occurrence != _scheduleQueueOccurrence)
                    break;
                _scheduleQueue.Add(_schedules[i]);
            }
        }

        /// <summary>
        /// Starts the Scheduler object's timers.
        /// </summary>
        /// <returns>true if successfull, false if not.</returns>
        public bool Start()
        {
            _logger.Debug("Starting scheduler...");
            _externalStopSignaled = false;
            try
            {
                StopInternal();
                return Continue();
            }
            finally
            {
                _logger.Debug("Scheduler started");
            }
        }

        /// <summary>
        /// Stops or pauses the Scheduler object's timers.
        /// </summary>
        public void Stop()
        {
            _logger.Debug("Stopping scheduler...");

            _externalStopSignaled = true;

            try
            {
                lock (_processingEventsLock)
                {
                    StopInternal();
                    IsRunning = false;
                }
            }
            finally
            {
                _logger.Debug("Scheduler stopped");
            }
        }

        /// <summary>
        /// Return true if the Scheduler is started, false if not.
        /// </summary>
        public bool IsRunning { get; private set; }

        private void StopInternal()
        {
            _timer.Disable();
        }

        private bool Continue()
        {
            return Continue(_timeProvider.Now);
        }

        private bool Continue(DateTime checkpoint)
        {
            Prepare(checkpoint);

            if (_externalStopSignaled || _schedules.Count == 0)
            {
                EventHelper.RaiseEvents(this, EndOfSchedule, EventArgs.Empty, _logger);
                IsRunning = false;
                _externalStopSignaled = false;
                return false;
            }

            QueueSchedules();

            var occurrence = _scheduleQueueOccurrence;

            _logger.Debug("Next occurence is {0:yyyyMMdd hh:mm:ss:fff}", occurrence);
            if (!_timer.Enable(occurrence))
            {
                var e = new AlarmEventArgs(occurrence, occurrence, null);
                AlarmEventHandler(this, e);
            }
            IsRunning = true;

            return true;
        }

        /// <summary>
        /// Round to nearest second.
        /// </summary>
        internal static DateTime RoundToNearestSecond(DateTime dateTime)
        {
            var sec = (int)Math.Round(((double)dateTime.Millisecond) / 1000);
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, 0).AddSeconds(sec);
        }

        private void AlarmEventHandler(object sender, AlarmEventArgs e)
        {
            StopInternal();

            lock (_processingEventsLock)
            {
                using (_loggerFactory.CreateNDC("InAlarmEventHandler"))
                {
                    for (var i = 0; i < _scheduleQueue.Count; i++)
                    {
                        var schedule = _schedules[i];

                        //Fire the ISchedule's own event
                        _logger.Debug(
                            "Signaling schedule {1} Occurrence {0:yyyyMMdd hh:mm:ss:fff} SignalTime {2:yyyyMMdd hh:mm:ss:fff} Actual signaltime {3:yyyyMMdd hh:mm:ss:fff} ",
                            schedule.CurrentOccurrence, schedule.Name, e.SignalTime, e.ActualSignalTime);

                        schedule.ItsTime(e.ActualSignalTime);

                        //Fire this schedulers event
                        var eventArgs = new AlarmEventArgs(e.SignalTime, e.ActualSignalTime, schedule.AdditionalData);
                        EventHelper.RaiseEvents(this, ScheduledAlarm, eventArgs, _logger);

                        _logger.Debug("Signaling schedule {0} done", schedule.Name);
                    }

                    // The following trick is necessary in order to avoid the last occurrence to be queued again.
                    // This could happen since we're "second aliasing" using RoundToNearestSecond()
                    var lastOccurrence = _scheduleQueue[_scheduleQueue.Count - 1].CurrentOccurrence;
                    var now = _timeProvider.Now;
                    if (now > lastOccurrence)
                        Continue(now);
                    else
                        Continue(lastOccurrence.AddTicks(1L));
                }
            }
        }

        /// <summary>
        /// Returns a string listing all Schedules in this Scheduler.
        /// </summary>
        /// <returns>A string listing all Schedules in this Scheduler.</returns>
        override public string ToString()
        {
            var sb = new StringBuilder(GetType().ToString());

            for (var i = 0; i < _schedules.Count; i++)
            {
                sb.Append(" \n");
                sb.Append(_schedules[i].ToString());
            }

            return sb.ToString();
        }

        #region IEnumerable Members

        /// <summary>
        /// Enumerates all schedules in the scheduler
        /// </summary>
        IEnumerator<ISchedule> IEnumerable<ISchedule>.GetEnumerator()
        {
            return _schedules.GetEnumerator();
        }

        /// <summary>
        /// Enumerates all schedules in the scheduler
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return _schedules.GetEnumerator();
        }

        #endregion

        /// <summary>
        /// Used by <see cref="SchedulerFactory"/> after instance is created.
        /// Configures properties on the newly created instance.
        /// </summary>
        internal void ConfigureScheduler()
        {
            _loggerFactory = Loggers.GetDefaultLoggerFactory();
            _logger = _loggerFactory.CreateLogger(typeof(Scheduler));
            _timer.SetLogger(_logger);
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
