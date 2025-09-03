using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using DemiCode.Logging;
using DemiCode.Logging.Configuration;
using DemiCode.Scheduling.Configuration;
using DemiCode.Scheduling.Schedules;
using DemiCode.Scheduling.Tasks;

namespace DemiCode.Scheduling
{
    /// <summary>
    /// Use the <see cref="SchedulerFactory"/> to create <see cref="Scheduler"/> instances.
    /// </summary>
    public static class SchedulerFactory
    {
        private class SchedulerFactoryInternal : ISchedulerFactory
        {
            private const bool _reloadConfigFromDiskDefault = false;

            public SchedulerFactoryInternal()
            {
            }

            /// <summary>
            /// Creates a Scheduler with schedules from the application configuration file.
            /// </summary>
            /// <param name="sectionName">The section name as defined in the .config files configSections/section</param>
            /// <returns>The default Scheduler configured in the configuration file.</returns>
            /// <seealso href="85482458-faf3-4de0-95c0-ef0b3709a1ea.htm" target="_blank">Scheduler configuration using .NET configuration files</seealso>
            public Scheduler CreateFromConfig(string sectionName)
            {
                return CreateFromConfig(sectionName, _reloadConfigFromDiskDefault);
            }

            /// <summary>
            /// Creates a Scheduler with schedules from the application configuration file.
            /// </summary>
            /// <param name="sectionName">The section name as defined in the .config files configSections/section</param>
            /// <param name="reloadConfigFromDisk">If true, config is reloaded from disk instead of using optionally cached data</param>
            /// <returns>The default Scheduler configured in the configuration file.</returns>
            /// <seealso href="85482458-faf3-4de0-95c0-ef0b3709a1ea.htm" target="_blank">Scheduler configuration using .NET configuration files</seealso>
            public Scheduler CreateFromConfig(string sectionName, bool reloadConfigFromDisk)
            {
                if (sectionName == null)
                    throw new ArgumentNullException(nameof(sectionName));

                return CreateFromConfigInternal(GetSchedulerSettingsSection(sectionName, reloadConfigFromDisk));
            }

            /// <summary>
            /// Creates a Scheduler with schedules from the application configuration file.
            /// </summary>
            /// <param name="sectionName">The section name as defined in the .config files configSections/section</param>
            /// <param name="schedulerName">The named scheduler to create</param>
            /// <returns>The named Scheduler configured in the configuration file.</returns>
            /// <seealso href="85482458-faf3-4de0-95c0-ef0b3709a1ea.htm" target="_blank">Scheduler configuration using .NET configuration files</seealso>
            public Scheduler CreateFromConfig(string sectionName, string schedulerName)
            {
                return CreateFromConfig(sectionName, schedulerName, _reloadConfigFromDiskDefault);
            }

            /// <summary>
            /// Creates a Scheduler with schedules from the application configuration file.
            /// </summary>
            /// <param name="sectionName">The section name as defined in the .config files configSections/section</param>
            /// <param name="schedulerName">The named scheduler to create</param>
            /// <param name="reloadConfigFromDisk">If true, config is reloaded from disk instead of using optionally cached data</param>
            /// <returns>The named Scheduler configured in the configuration file.</returns>
            /// <seealso href="85482458-faf3-4de0-95c0-ef0b3709a1ea.htm" target="_blank">Scheduler configuration using .NET configuration files</seealso>
            public Scheduler CreateFromConfig(string sectionName, string schedulerName, bool reloadConfigFromDisk)
            {
                if (sectionName == null)
                    throw new ArgumentNullException(nameof(sectionName));

                if (schedulerName == null)
                    throw new ArgumentNullException(nameof(schedulerName), "Scheduler name must be a valid scheduler in the configuration file");

                return CreateFromConfigInternal(schedulerName, GetSchedulerSettingsSection(sectionName, reloadConfigFromDisk));
            }

            private static Scheduler CreateFromConfigInternal(SchedulerSettings schedulerSettings)
            {
                return CreateFromConfigInternal(schedulerSettings.DefaultScheduler, schedulerSettings);
            }

            private static Scheduler CreateFromConfigInternal(string schedulerName, SchedulerSettings schedulerSettings)
            {
                var schedulerElement = schedulerSettings.Schedulers[schedulerName];
                if (schedulerElement == null)
                    throw new ArgumentException("Scheduler name must correspond to a scheduler item in the specified configuration", nameof(schedulerName));

                var scheduler = new Scheduler();

                try
                {
                    scheduler.AddRange(schedulerElement.GetScheduleInstances());
                    AddLoggers(scheduler, schedulerElement.Loggers);
                    scheduler.Tasks.Add(schedulerElement.Tasks);
                    scheduler.ConfigureScheduler();
                    return scheduler;
                }
                catch
                {
                    scheduler.Dispose();
                    throw;
                }
            }

            private static SchedulerSettings GetSchedulerSettingsSection(string sectionName, bool reloadConfigFromDisk)
            {
                return SchedulerSettingsManager.GetConfigurationSection(sectionName, reloadConfigFromDisk);
            }

            private static SchedulerSettings GetSchedulerSettingsSection(string configFile, string sectionName)
            {
                return SchedulerSettingsManager.GetConfigurationSection(configFile, sectionName);
            }

            private static void AddLoggers(Scheduler scheduler, LoggerElementCollection loggers)
            {
                foreach (LoggerElementBase logger in loggers)
                {
                    scheduler.Loggers.Add(logger.GetLoggerInstance());
                }
            }

            /// <summary>
            /// Creates a Scheduler instance initialized with a collection of schedules.
            /// </summary>
            /// <param name="schedules">A collection of ISchedule objects.</param>
            /// <returns>A Scheduler configured with the collection of schedules.</returns>
            public Scheduler Create(IEnumerable<ISchedule> schedules)
            {
                return CreateInternal(schedules, null, null);
            }

            /// <summary>
            /// Creates a Scheduler instance with no schedules.
            /// </summary>
            /// <returns>A Scheduler configured with the collection of schedules.</returns>
            public Scheduler Create()
            {
                return CreateInternal(new List<ISchedule>(), null, null);
            }

            /// <summary>
            /// Creates a Scheduler instance with no schedules.
            /// </summary>
            /// <param name="loggerFactory">A logger factory used to log information during scheduler operation.</param>
            /// <returns>A Scheduler configured with the collection of schedules.</returns>
            public Scheduler Create(ILoggerFactory loggerFactory)
            {
                return CreateInternal(new List<ISchedule>(), null, loggerFactory);
            }

            /// <summary>
            /// Creates a Scheduler instance initialized with a collection of schedules.
            /// </summary>
            /// <param name="schedules">A collection of ISchedule objects</param>
            /// <param name="loggerFactory">A logger factory used to log information during scheduler operation.</param>
            /// <returns>A Scheduler configured with the collection of schedules.</returns>
            public Scheduler Create(IEnumerable<ISchedule> schedules, ILoggerFactory loggerFactory)
            {
                return CreateInternal(schedules, null, loggerFactory);
            }

            private static Scheduler CreateInternal(IEnumerable<ISchedule> schedules, IEnumerable<ITask> tasks, ILoggerFactory loggerFactory)
            {
                var scheduler = new Scheduler();

                try
                {
                    scheduler.AddRange(schedules);
                    if (loggerFactory != null)
                    {
                        scheduler.Loggers.Add(loggerFactory);
                    }
                    if (tasks != null)
                    {
                        scheduler.Tasks.AddRange(tasks);
                    }
                    scheduler.ConfigureScheduler();
                    return scheduler;
                }
                catch
                {
                    scheduler.Dispose();
                    throw;
                }

            }

            /// <summary>
            /// Creates a Scheduler with schedules from the specified configuration file (.config).
            /// </summary>
            /// <param name="configFile">Path to the .config file to use.</param>
            /// <param name="sectionName">The section name as defined in the .config files configSections/section</param>
            /// <returns>The named Scheduler configured in the configuration file.</returns>
            /// <seealso href="85482458-faf3-4de0-95c0-ef0b3709a1ea.htm" target="_blank">Scheduler configuration using .NET configuration files</seealso>
            public Scheduler CreateFromConfigFile(string configFile, string sectionName)
            {
                if (String.IsNullOrEmpty(configFile))
                    throw new ArgumentNullException(nameof(configFile));
                if (!File.Exists(configFile))
                    throw new ArgumentException("Could not locate configuration file '" + configFile + "'", nameof(configFile));
                if (String.IsNullOrEmpty(sectionName))
                    throw new ArgumentNullException(nameof(sectionName));

                var ss = GetSchedulerSettingsSection(configFile, sectionName);
                return CreateFromConfigInternal(ss);
            }


            /// <summary>
            /// Creates a Scheduler with schedules from the specified configuration file (.config).
            /// </summary>
            /// <param name="configFile">Path to the .config file to use.</param>
            /// <param name="sectionName">The section name as defined in the .config files configSections/section</param>
            /// <param name="schedulerName">The named scheduler to create</param>
            /// <returns>The named Scheduler configured in the configuration file.</returns>
            /// <seealso href="85482458-faf3-4de0-95c0-ef0b3709a1ea.htm" target="_blank">Scheduler configuration using .NET configuration files</seealso>
            public Scheduler CreateFromConfigFile(string configFile, string sectionName, string schedulerName)
            {
                if (String.IsNullOrEmpty(configFile))
                    throw new ArgumentNullException(nameof(configFile));
                if (!File.Exists(configFile))
                    throw new ArgumentException("Could not locate configuration file '" + configFile + "'", nameof(configFile));
                if (String.IsNullOrEmpty(sectionName))
                    throw new ArgumentNullException(nameof(sectionName));
                if (String.IsNullOrEmpty(schedulerName))
                    throw new ArgumentNullException(nameof(schedulerName));

                var ss = GetSchedulerSettingsSection(configFile, sectionName);
                return CreateFromConfigInternal(schedulerName, ss);
            }

        }

        private static readonly ISchedulerFactory DefaultFactory = new SchedulerFactoryInternal();

        /// <summary>
        /// Creates a Scheduler with schedules from the application configuration file.
        /// </summary>
        /// <param name="sectionName">The section name as defined in the .config files configSections/section</param>
        /// <returns>The default Scheduler configured in the configuration file.</returns>
        /// <seealso href="85482458-faf3-4de0-95c0-ef0b3709a1ea.htm" target="_blank">Scheduler configuration using .NET configuration files</seealso>
        public static Scheduler CreateFromConfig(string sectionName)
        {
            return DefaultFactory.CreateFromConfig(sectionName);
        }

        /// <summary>
        /// Creates a Scheduler with schedules from the application configuration file.
        /// </summary>
        /// <param name="sectionName">The section name as defined in the .config files configSections/section</param>
        /// <param name="reloadConfigFromDisk">If true, config is reloaded from disk instead of using optionally cached data</param>
        /// <returns>The default Scheduler configured in the configuration file.</returns>
        /// <seealso href="85482458-faf3-4de0-95c0-ef0b3709a1ea.htm" target="_blank">Scheduler configuration using .NET configuration files</seealso>
        public static Scheduler CreateFromConfig(string sectionName, bool reloadConfigFromDisk)
        {
            return DefaultFactory.CreateFromConfig(sectionName, reloadConfigFromDisk);
        }

        /// <summary>
        /// Creates a Scheduler with schedules from the application configuration file.
        /// </summary>
        /// <param name="sectionName">The section name as defined in the .config files configSections/section</param>
        /// <param name="schedulerName">The named scheduler to create</param>
        /// <returns>The named Scheduler configured in the configuration file.</returns>
        /// <seealso href="85482458-faf3-4de0-95c0-ef0b3709a1ea.htm" target="_blank">Scheduler configuration using .NET configuration files</seealso>
        public static Scheduler CreateFromConfig(string sectionName, string schedulerName)
        {
            return DefaultFactory.CreateFromConfig(sectionName, schedulerName);
        }

        /// <summary>
        /// Creates a Scheduler with schedules from the application configuration file.
        /// </summary>
        /// <param name="sectionName">The section name as defined in the .config files configSections/section</param>
        /// <param name="schedulerName">The named scheduler to create</param>
        /// <param name="reloadConfigFromDisk">If true, config is reloaded from disk instead of using optionally cached data</param>
        /// <returns>The named Scheduler configured in the configuration file.</returns>
        /// <seealso href="85482458-faf3-4de0-95c0-ef0b3709a1ea.htm" target="_blank">Scheduler configuration using .NET configuration files</seealso>
        public static Scheduler CreateFromConfig(string sectionName, string schedulerName, bool reloadConfigFromDisk)
        {
            return DefaultFactory.CreateFromConfig(sectionName, schedulerName, reloadConfigFromDisk);
        }

        /// <summary>
        /// Creates a Scheduler instance initialized with a collection of schedules.
        /// </summary>
        /// <param name="schedules">A collection of ISchedule objects.</param>
        /// <returns>A Scheduler configured with the collection of schedules.</returns>
        public static Scheduler Create(IEnumerable<ISchedule> schedules)
        {
            return DefaultFactory.Create(schedules);
        }

        /// <summary>
        /// Creates a Scheduler instance with no schedules. Use the <see cref="Scheduler.Add"/> to add schedules.
        /// </summary>
        /// <returns>An empty Scheduler.</returns>
        public static Scheduler Create()
        {
            return DefaultFactory.Create();
        }

        /// <summary>
        /// Creates a Scheduler instance with no schedules.
        /// </summary>
        /// <param name="loggerFactory">A logger factory used to log information during scheduler operation.</param>
        /// <returns>A Scheduler configured with the collection of schedules.</returns>
        public static Scheduler Create(ILoggerFactory loggerFactory)
        {
            return DefaultFactory.Create(loggerFactory);
        }

        /// <summary>
        /// Creates a Scheduler instance initialized with a collection of schedules.
        /// </summary>
        /// <param name="schedules">A collection of ISchedule objects</param>
        /// <param name="loggerFactory">A logger factory used to log information during scheduler operation.</param>
        /// <returns>A Scheduler configured with the collection of schedules.</returns>
        public static Scheduler Create(IEnumerable<ISchedule> schedules, ILoggerFactory loggerFactory)
        {
            return DefaultFactory.Create(schedules, loggerFactory);
        }

        /// <summary>
        /// Creates a Scheduler with schedules from the specified configuration file (.config).
        /// </summary>
        /// <param name="configFile">Path to the .config file to use.</param>
        /// <param name="sectionName">The section name as defined in the .config files configSections/section</param>
        /// <returns>The named Scheduler configured in the configuration file.</returns>
        /// <seealso href="85482458-faf3-4de0-95c0-ef0b3709a1ea.htm" target="_blank">Scheduler configuration using .NET configuration files</seealso>
        public static Scheduler CreateFromConfigFile(string configFile, string sectionName)
        {
            return DefaultFactory.CreateFromConfigFile(configFile, sectionName);
        }

        /// <summary>
        /// Creates a Scheduler with schedules from the specified configuration file (.config).
        /// </summary>
        /// <param name="configFile">Path to the .config file to use.</param>
        /// <param name="sectionName">The section name as defined in the .config files configSections/section</param>
        /// <param name="schedulerName">The named scheduler to create</param>
        /// <returns>The named Scheduler configured in the configuration file.</returns>
        /// <seealso href="85482458-faf3-4de0-95c0-ef0b3709a1ea.htm" target="_blank">Scheduler configuration using .NET configuration files</seealso>
        public static Scheduler CreateFromConfigFile(string configFile, string sectionName, string schedulerName)
        {
            return DefaultFactory.CreateFromConfigFile(configFile, sectionName, schedulerName);
        }

    }

}
