using System;
using DemiCode.Scheduling.Schedules;

namespace DemiCode.Scheduling.Test.Mocks
{
    public class NamedOneShotSchedule : OneShotSchedule
    {
        public NamedOneShotSchedule(string name) : base(DateTime.Now)
        {
            Name = name;
        }
    }
}