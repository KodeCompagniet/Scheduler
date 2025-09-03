using System;
using System.IO;
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using NUnit.Framework;

namespace DemiCode.Scheduling.Test
{
    /// <summary>
    /// Perform asserts on the current log4net MemoryAppender.
    /// </summary>
    internal static class AssertLog
    {
        /// <summary>
        /// Configure log4net with the given config file and clear out the memory appender.
        /// </summary>
        public static void Configure(string pathToConfigFile)
        {
            XmlConfigurator.Configure(new Uri(Path.GetFullPath(pathToConfigFile)));
            ClearLog();
        }

        /// <summary>
        /// Get the event at the specified index.
        /// </summary>
        public static LoggingEvent GetEvent(int index)
        {
            return GetMemoryAppender().GetEvents()[index];
        }

        /// <summary>
        /// Get all events.
        /// </summary>
        public static LoggingEvent[] GetEvents()
        {
            return GetMemoryAppender().GetEvents();
        }

        /// <summary>
        /// Get our current memory appender
        /// </summary>
        /// <returns></returns>
        public static MemoryAppender GetMemoryAppender()
        {
            var repository = LogManager.GetRepository();
            var appenders = repository.GetAppenders();

            foreach (var appender in appenders)
            {
                if (appender.Name == "MemoryAppender")
                {
                    return (MemoryAppender)appender;
                }
            }

            throw new InvalidOperationException("MemoryAppender is not configured");
        }

        /// <summary>
        /// Clear any events from the current log.
        /// </summary>
        public static void ClearLog()
        {
            GetMemoryAppender().Clear();
        }

        /// <summary>
        /// Assert that the current number of events in the log is equal to the expected value.
        /// </summary>
        /// <param name="expected"></param>
        public static void EventCountIsEqual(int expected)
        {
            var a = new EventCountAsserter(expected, false);
            a.Test().Should().BeTrue();
        }

        /// <summary>
        /// Assert that the current number of Error events in the log is equal to the expected value.
        /// </summary>
        /// <param name="expected"></param>
        public static void ErrorCountIsEqual(int expected)
        {
            var a = new EventCountAsserter(expected, Level.Error, false);
            a.Test().Should().BeTrue();
        }

        /// <summary>
        /// Assert that the current number of events in the log is different from the expected value.
        /// </summary>
        /// <param name="expected"></param>
        public static void EventCountIsNotEqual(int expected)
        {
            var a = new EventCountAsserter(expected, true);
            a.Test().Should().BeTrue();
        }

        private class EventCountAsserter
        {
            private readonly int _expected;
            private readonly Level _level;
            private readonly bool _not;

            public EventCountAsserter(int expected, bool not) : this(expected, null, not) { }

            public EventCountAsserter(int expected, Level level, bool not)
            {
                _expected = expected;
                _level = level;
                _not = not;
            }

            public bool Test()
            {
                int actual;

                if (_level == null)
                {
                    actual = GetMemoryAppender().GetEvents().Length;
                }
                else
                {
                    actual = 0;
                    foreach (LoggingEvent loggingEvent in GetMemoryAppender().GetEvents())
                    {
                        if (loggingEvent.Level.Equals(_level))
                            actual++;
                    }
                }

                

                if (_not)
                {
                    Message = String.Format("The number of events in the log did not return the expected value. Not expected: {0}. Actual: {1}.", _expected, actual);

                    return (_expected != actual);
                }

                Message = String.Format("The number of events in the log did not return the expected value. Expected: {0}. Actual: {1}.", _expected, actual);
                return (_expected == actual);
            }

            public string Message { get; private set; }
        }
    }
}
