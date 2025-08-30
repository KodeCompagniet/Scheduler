using DemiCode.Logging;

namespace DemiCode.Scheduling.Tasks
{
    /// <summary>
    /// Represent a task that can be set up to execute when a schedule triggers.
    /// </summary>
    public interface ITask
    {
        /// <summary>
        /// Execute the task with the given parameters.
        /// </summary>
        /// <param name="loggerFactory">A logger factory which the task can use to create loggers for logging information, warnings and errors</param>
        void Execute(ILoggerFactory loggerFactory);
    }
}
