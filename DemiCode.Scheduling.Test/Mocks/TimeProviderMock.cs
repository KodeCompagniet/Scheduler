using System;
using DemiCode.Scheduling.Time;

namespace DemiCode.Scheduling.Test.Mocks
{
    public class TimeProviderMock : ITimeProvider
    {
        private DateTime _now = DateTime.Now;

        public DateTime Now
        {
            get { return _now; }
            set { _now = value; }
        }
    }
}