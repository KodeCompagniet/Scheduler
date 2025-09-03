using System;
using DemiCode.Logging;

namespace DemiCode.Scheduling.Tasks
{
    /// <summary>
    /// TaskSequence executes a sequence of tasks.
    /// </summary>
    /// 
    /// <remarks>
    /// <see cref="TaskSequence"/> executes the tasks in sequence according to their possition in the <see cref="Tasks"/> array.
    /// If a task fails during execution the sequence is terminated and the exception will be logged.
    /// </remarks>
    /// 
    public class TaskSequence : ITask
    {
        /// <summary>
        /// The tasks executed by this <see cref="TaskSequence"/>.
        /// </summary>
        public ITask[] Tasks { get; private set; }

        /// <summary>
        /// Create a new instance with the specified tasks to execute.
        /// </summary>
        /// <param name="tasks">Array of tasks to execute on each call to <see cref="Execute"/>.</param>
        public TaskSequence(ITask[] tasks)
        {
            if (tasks == null)
                throw new ArgumentNullException(nameof(tasks));

            if (tasks.Length == 0)
                throw new ArgumentException("The tasks array must have one or more items", nameof(tasks));
 
            Tasks = tasks;
        }

        /// <summary>
        /// Execute the task.
        /// </summary>
        /// <param name="loggerFactory">A logger factory which the task can use to create loggers for logging information, warnings and errors</param>
        public void Execute(ILoggerFactory loggerFactory)
        {
            if (loggerFactory == null)
                throw new ArgumentNullException(nameof(loggerFactory));

            var taskLogger = loggerFactory.CreateLogger(typeof(TaskSequence));
            for (var taskIndex = 0; taskIndex < Tasks.Length; taskIndex++)
            {
                var task = Tasks[taskIndex];

                try
                {
                    taskLogger.Debug("Executing task '{0}'", task.GetType().FullName);
                    task.Execute(loggerFactory);
                    taskLogger.Debug("Execute successfull");
                }
                catch (Exception e)
                {
                    taskLogger.Error("Failed to execute task {0}. Terminating task sequence.", e, task.GetType().FullName);
                    break;
                }
                finally
                {
                    var disposableTask = task as IDisposable;
                    if (disposableTask != null)
                    {
                        disposableTask.Dispose();
                    }
                }
            }
        }

    }
}
