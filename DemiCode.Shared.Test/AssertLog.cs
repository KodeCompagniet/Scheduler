using System;
using log4net;
using log4net.Appender;
using log4net.Core;
using NUnit.Framework;

namespace DemiCode.Shared.Test
{
    /// <summary>
    /// Perform asserts on the current log4net MemoryAppender.
    /// </summary>
    internal static class AssertLog
    {
        public static MemoryAppender GetMemoryAppender(string appenderName)
        {
            var repository = LogManager.GetRepository();
            var appenders = repository.GetAppenders();

            foreach (var appender in appenders)
            {
                if (appender is MemoryAppender && appender.Name == appenderName)
                {
                    return (MemoryAppender)appender;
                }
            }

            throw new InvalidOperationException("MemoryAppender [" + appenderName + "] is not configured");
        }

        /// <summary>
        /// Clear any events from the current log.
        /// </summary>
        public static void ClearLog(string appenderName)
        {
            GetMemoryAppender(appenderName).Clear();
        }

        /// <summary>
        /// Assert that the current number of events in the log is equal to the expected value.
        /// </summary>
        public static void EventCountIsEqual(string appenderName, int expected)
        {
            GetMemoryAppender(appenderName).GetEvents().Length.Should().Be(expected);
        }

        /// <summary>
        /// Assert that the current number of Error events in the log is equal to the expected value.
        /// </summary>
        public static void ErrorCountIsEqual(string appenderName, int expected)
        {
            GetEventCount(GetMemoryAppender(appenderName), Level.Error).Should().Be(expected);
        }

        /// <summary>
        /// Assert that the current number of Info events in the log is equal to the expected value.
        /// </summary>
        public static void InfoCountIsEqual(string appenderName, int expected)
        {
            GetEventCount(GetMemoryAppender(appenderName), Level.Info).Should().Be(expected);
        }

        private static int GetEventCount(MemoryAppender appender, Level level)
        {
            var cnt = 0;
            foreach (var loggingEvent in appender.GetEvents())
            {
                if (loggingEvent.Level.Equals(level))
                    cnt++;
            }
            return cnt;
        }

        /// <summary>
        /// Assert that the current number of Warn events in the log is equal to the expected value.
        /// </summary>
        public static void WarnCountIsEqual(string appenderName, int expected)
        {
            GetEventCount(GetMemoryAppender(appenderName), Level.Warn).Should().Be(expected);
        }
    }
}
