using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DemiCode.Logging;
using DemiCode.Scheduling.Configuration.Tasks;
using DemiCode.Scheduling.Schedules;
using DemiCode.Scheduling.Tasks;

namespace DemiCode.Scheduling.Tasks
{
    /// <summary>
    /// A collection of tasks in a <see cref="Scheduler"/>.
    /// </summary>
    public class TaskCollection : IList<ITask>, IDisposable
    {
        private readonly Scheduler _scheduler;
        private List<ITask> _tasks = new List<ITask>();
        private readonly Collection<TaskHandler> _taskHandlers = new Collection<TaskHandler>();
        private const bool isReadOnly = false;

        internal TaskCollection(Scheduler scheduler)
        {
            if (scheduler == null)
                throw new ArgumentNullException(nameof(scheduler));

            _scheduler = scheduler;
        }

        int IList<ITask>.IndexOf(ITask task)
        {
            return _tasks.IndexOf(task);
        }

        void IList<ITask>.Insert(int index, ITask task)
        {
            throw new NotSupportedException("Inserting task at specific index is not supported. Use Add method to append tasks.");
        }

        void IList<ITask>.RemoveAt(int index)
        {
            //To implement RemoveAt, implement removing event handlers from scheduler first
            //_tasks.RemoveAt(index);
            throw new NotSupportedException("Removing tasks is not supported");
        }

        /// <summary>
        /// Return the task at the specified index.
        /// </summary>
        public ITask this[int index]
        {
            get
            {
                return _tasks[index];
            }
            set
            {
                //TODO: implement removing/replacing event handlers from scheduler first
                throw new NotSupportedException("Replacing tasks is not supported");
            }
        }

        void ICollection<ITask>.Clear()
        {
            //TODO: implement removing/replacing event handlers from scheduler first
            //_tasks.Clear();
            throw new NotSupportedException("Removing tasks is not supported");
        }

        bool ICollection<ITask>.Contains(ITask task)
        {
            return _tasks.Contains(task);
        }

        void ICollection<ITask>.CopyTo(ITask[] array, int arrayIndex)
        {
            _tasks.CopyTo(array, arrayIndex);
        }

        bool ICollection<ITask>.Remove(ITask item)
        {
            //TODO: implement removing/replacing event handlers from scheduler first
            throw new NotSupportedException("Removing tasks is not supported");
        }

        /// <summary>
        /// Number of tasks in the collection.
        /// </summary>
        public int Count
        {
            get { return _tasks.Count; }
        }

        bool ICollection<ITask>.IsReadOnly
        {
            get { return isReadOnly; }
        }

        IEnumerator<ITask> IEnumerable<ITask>.GetEnumerator()
        {
            return _tasks.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _tasks.GetEnumerator();
        }

        /// <summary>
        /// Add tasks according to the specified configuration collection.
        /// </summary>
        internal void Add(TaskElementCollection tasks)
        {
            foreach (TaskElementBase task in tasks)
            {
                var taskImpl = task.GetTaskInstance();
                if (task.Count > 0)
                {
                    foreach (ScheduleReferenceElement scheduleref in task)
                    {
                        Add(taskImpl, scheduleref.ScheduleName);
                    }
                }
                else
                {
                    Add(taskImpl);
                }
            }
        }

        /// <summary>
        /// Add a task to be executed when <see cref="Scheduler.ScheduledAlarm"/> fires.
        /// </summary>
        /// <param name="task">The task to execute</param>
        public void Add(ITask task)
        {
            Add(task, _scheduler.Loggers.GetDefaultLoggerFactory());
        }

        /// <summary>
        /// Add a task to be executed when <see cref="Scheduler.ScheduledAlarm"/> fires.
        /// </summary>
        /// <param name="task">The task to execute</param>
        /// <param name="parameters">A dictionary of parameters to be passed to the task when it is executed</param>
        [Obsolete("Parameters dictionary is no longer used. Use the Add(ITask task) method")]
        public void Add(ITask task, IDictionary parameters)
        {
            Add(task);
        }

        /// <summary>
        /// Add a task to be executed only when <see cref="ScheduleBase.Alarm"/> for a specific schedule is fired.
        /// </summary>
        /// <param name="task">The task to execute</param>
        /// <param name="parameters">(Ignored) A dictionary of parameters to be passed to the task when it is executed</param>
        /// <param name="scheduleName">Name of the schedule that the task will be attached to.</param>
        [Obsolete("Parameters dictionary is no longer used. Use the Add(ITask task, string scheduleName) method")]
        public void Add(ITask task, IDictionary parameters, string scheduleName)
        {
            Add(task, scheduleName);
        }

        /// <summary>
        /// Add a task to be executed only when <see cref="ScheduleBase.Alarm"/> for a specific schedule is fired.
        /// </summary>
        /// <param name="task">The task to execute</param>
        /// <param name="scheduleName">Name of the schedule that the task will be attached to.</param>
        public void Add(ITask task, string scheduleName)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            ISchedule schedule = null;
            if (!String.IsNullOrEmpty(scheduleName))
            {
                if (!_scheduler.TryGetSchedule(scheduleName, out schedule))
                {
                    throw new ArgumentException("The scheduleName parameter does not reference an existing schedule", nameof(scheduleName));
                }
            }

            var loggerFactory = _scheduler.Loggers.GetDefaultLoggerFactory();
            Add(task, schedule, loggerFactory);
        }

        #region Dispose and dtor stuff

        private bool _disposed;

        /// <summary>
        /// Destructor
        /// </summary>
        ~TaskCollection()
        {
            Dispose(false);
        }

        /// <summary>
        /// Disposes the AlarmTimer
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
                    if (_tasks != null)
                    {
                        foreach (var task in _tasks)
                        {
                            var dt = task as IDisposable;
                            if (dt != null)
                                dt.Dispose();
                        }
                        _tasks = null;
                    }
                }

                // Release unmanaged resources. If disposing is false, 
                // only the following code is executed.
            }

            _disposed = true;
        }

        #endregion

        /// <summary>
        /// Add a list of tasks to this collection.
        /// </summary>
        /// <param name="tasks">The list of tasks to add.</param>
        public void AddRange(IEnumerable<ITask> tasks)
        {
            if (tasks == null)
                throw new ArgumentNullException(nameof(tasks));

            var loggerFactory = _scheduler.Loggers.GetDefaultLoggerFactory();
            foreach (var task in tasks)
            {
                Add(task, loggerFactory);
            }
        }

        /// <summary>
        /// Add a list of tasks to this collection.
        /// </summary>
        /// <param name="tasks">The list of tasks to add.</param>
        /// <param name="scheduleName">The name of a schedule to connect the tasks to.</param>
        public void AddRange(IEnumerable<ITask> tasks, string scheduleName)
        {
            if (tasks == null)
                throw new ArgumentNullException(nameof(tasks));
            if (scheduleName == null)
                throw new ArgumentNullException(nameof(scheduleName));

            ISchedule schedule;
            if (!_scheduler.TryGetSchedule(scheduleName, out schedule))
            {
                throw new ArgumentException("The scheduleName parameter does not reference an existing schedule", nameof(scheduleName));
            }

            var loggerFactory = _scheduler.Loggers.GetDefaultLoggerFactory();
            foreach (var task in tasks)
            {
                Add(task, schedule, loggerFactory);
            }
        }

        private void Add(ITask task, ILoggerFactory loggerFactory)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            var handler = new TaskHandler(task, loggerFactory);
            _tasks.Add(task);
            _taskHandlers.Add(handler);

            // Attach to global alarm
            _scheduler.ScheduledAlarm += handler.ScheduledAlarmEventHandler;
        }

        private void Add(ITask task, ISchedule schedule, ILoggerFactory loggerFactory)
        {
            if (task == null)
                throw new ArgumentNullException(nameof(task));

            var handler = new TaskHandler(task, loggerFactory);
            _tasks.Add(task);
            _taskHandlers.Add(handler);

            // Attach to specific schedule
            schedule.Alarm += handler.ScheduledAlarmEventHandler;
        }
    }
}
