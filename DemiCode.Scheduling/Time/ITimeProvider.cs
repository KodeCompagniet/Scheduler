using System;
using System.Collections.Generic;
using System.Text;

namespace DemiCode.Scheduling.Time
{
    internal interface ITimeProvider
    {
        DateTime Now { get; }
    }
}
