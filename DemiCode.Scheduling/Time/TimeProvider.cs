using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace DemiCode.Scheduling.Time
{
    /// <summary>
    /// Default implementation of the time provider interface. This implementation will return the current time using builtin framework methods.
    /// </summary>
    internal class DefaultTimeProvider : ITimeProvider
    {
        public DateTime Now
        {
            get
            {
                //return new DateTime(Stopwatch.GetTimestamp());
                return DateTime.Now;
            }
        }
    }
}
