using System;
using System.Collections.Generic;
using System.Text;
using DemiCode.Logging.Configuration;

namespace DemiCode.Scheduling.Configuration
{
    /// <summary>
    /// Alias class for <see cref="LoggerElementCollection{V}"/> used by DemiCode Scheduler.
    /// </summary>
    public class LoggerElementCollection : DemiCode.Logging.Configuration.LoggerElementCollection<SchedulerSettingsManager.SerializationStateProvider>
    {
    }
}
