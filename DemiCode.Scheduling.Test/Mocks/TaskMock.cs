using System;
using DemiCode.Logging;
using DemiCode.Scheduling.Tasks;

namespace DemiCode.Scheduling.Test.Mocks
{
    internal class TaskMock : ITask
    {
        [ThreadStatic]
        public static bool WasExecutedByThread;

        public bool WasExecuted;

        public TaskMock()
        {}

        public string Param0 { get; private set; }
        public TaskMock(string param0)
        {
            Param0 = param0;
        }

        public void Execute(ILoggerFactory loggerFactory)
        {
            WasExecutedByThread = true;
            WasExecuted = true;
        }
    }

    internal class BadTaskMock : ITask
    {
        [ThreadStatic]
        public static bool WasExecutedByThread;

        public bool WasExecuted;

        public void Execute(ILoggerFactory loggerFactory)
        {
            WasExecutedByThread = true;
            WasExecuted = true;

            throw new InvalidOperationException("This is a bad task");
        }
    }

    internal class DisposableTask : IDisposable, ITask
    {
        [ThreadStatic]
        public static bool DisposeWasCalled = false;

        public DisposableTask()
        { }

        public static void ResetTrackerFields()
        {
            DisposeWasCalled = false;
        }

        public void Dispose()
        {
            DisposeWasCalled = true;
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Execute the task with the given parameters.
        /// </summary>
        /// <param name="loggerFactory">A logger factory which the task can use to create loggers for logging information, warnings and errors</param>
        public void Execute(ILoggerFactory loggerFactory)
        {
            throw new NotImplementedException();
        }
    }
}
