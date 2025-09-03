using System;
using System.Diagnostics;
using DemiCode.Scheduling.Schedules;
using NUnit.Framework;

namespace DemiCode.Scheduling.Test
{
    internal static class ScheduleAssert
    {
        [DebuggerStepThrough]
        public static void Success(ISchedule instance, DateTime targetTime, DateTime expectedTime)
        {
            instance.MoveToFirstFrom(targetTime).Should().BeTrue();
            instance.CurrentOccurrence.Should().Be(expectedTime);
        }

    }
}
