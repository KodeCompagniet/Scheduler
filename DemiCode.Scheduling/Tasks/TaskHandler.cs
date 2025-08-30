using System;
using System.Collections;
using DemiCode.Logging;
using DemiCode.Scheduling.Alarms;
using DemiCode.Scheduling.Tasks;

namespace DemiCode.Scheduling.Tasks
{
    /// <summary>
    /// Container class for a task.
    /// </summary>
    internal class TaskHandler
    {
        private readonly ITask _task;
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger _taskLogger;

        [Obsolete("Parameters are no longer in use")]
        public TaskHandler(ITask task, IDictionary parameters, ILoggerFactory loggerFactory) : this(task, loggerFactory)
        {
        }

        public TaskHandler(ITask task, ILoggerFactory loggerFactory)
        {
            if (task == null)
                throw new ArgumentNullException("task");
            if (loggerFactory == null)
                throw new ArgumentNullException("loggerFactory");

            _task = task;
            _loggerFactory = loggerFactory;
            _taskLogger = _loggerFactory.CreateLogger(typeof(TaskHandler));
        }

        public void ScheduledAlarmEventHandler(object sender, AlarmEventArgs eventArgs)
        {
            try
            {
                _taskLogger.Debug("Executing task '{0}'", _task.GetType().FullName);
                _task.Execute(_loggerFactory);
                _taskLogger.Debug("Execute successfull");
            }
            catch (Exception e)
            {
                _taskLogger.Error("Failed to execute task {0}", e, _task.GetType().FullName);
            }
        }
    }
}