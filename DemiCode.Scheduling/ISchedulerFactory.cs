using System.Collections.Generic;
using DemiCode.Logging;
using DemiCode.Scheduling.Schedules;

namespace DemiCode.Scheduling
{
    /// <summary>
    /// This interface represents a factory for creating <see cref="Scheduler"/> instances.
    /// </summary>
    public interface ISchedulerFactory
    {
        /// <summary>
        /// Creates a Scheduler instance with no schedules. Use the <see cref="Scheduler.Add"/> to add schedules.
        /// </summary>
        /// <returns>An empty Scheduler.</returns>
        Scheduler Create();

        /// <summary>
        /// Creates an evaluation Scheduler instance with no schedules.
        /// </summary>
        /// <param name="loggerFactory">A logger factory used to log information during scheduler operation.</param>
        /// <returns>A Scheduler configured with the collection of schedules.</returns>
        Scheduler Create(ILoggerFactory loggerFactory);

        /// <summary>
        /// Creates a Scheduler instance initialized with a collection of schedules.
        /// </summary>
        /// <param name="schedules">A collection of ISchedule objects</param>
        /// <returns>A Scheduler configured with the collection of schedules.</returns>
        Scheduler Create(IEnumerable<ISchedule> schedules);

        /// <summary>
        /// Creates a Scheduler instance initialized with a collection of schedules.
        /// </summary>
        /// <param name="schedules">A collection of ISchedule objects</param>
        /// <param name="loggerFactory">A logger factory used to log information during scheduler operation.</param>
        /// <returns>A Scheduler configured with the collection of schedules.</returns>
        Scheduler Create(IEnumerable<ISchedule> schedules, ILoggerFactory loggerFactory);

        /// <summary>
        /// Creates a Scheduler with schedules from the application configuration file.
        /// </summary>
        /// <param name="sectionName">The section name as defined in the .config files configSections/section</param>
        /// <returns>The default Scheduler configured in the configuration file.</returns>
        /// <seealso href="85482458-faf3-4de0-95c0-ef0b3709a1ea.htm" target="_blank">Scheduler configuration using .NET configuration files</seealso>
        Scheduler CreateFromConfig(string sectionName);

        /// <summary>
        /// Creates a Scheduler with schedules from the application configuration file.
        /// </summary>
        /// <param name="sectionName">The section name as defined in the .config files configSections/section</param>
        /// <param name="reloadConfigFromDisk">If true, config is reloaded from disk instead of using optionally cached data</param>
        /// <returns>The default Scheduler configured in the configuration file.</returns>
        /// <seealso href="85482458-faf3-4de0-95c0-ef0b3709a1ea.htm" target="_blank">Scheduler configuration using .NET configuration files</seealso>
        Scheduler CreateFromConfig(string sectionName, bool reloadConfigFromDisk);

        /// <summary>
        /// Creates a Scheduler with schedules from the application configuration file.
        /// </summary>
        /// <param name="sectionName">The section name as defined in the .config files configSections/section</param>
        /// <param name="schedulerName">The named scheduler to create</param>
        /// <returns>The named Scheduler configured in the configuration file.</returns>
        /// <seealso href="85482458-faf3-4de0-95c0-ef0b3709a1ea.htm" target="_blank">Scheduler configuration using .NET configuration files</seealso>
        Scheduler CreateFromConfig(string sectionName, string schedulerName);

        /// <summary>
        /// Creates a Scheduler with schedules from the application configuration file.
        /// </summary>
        /// <param name="sectionName">The section name as defined in the .config files configSections/section</param>
        /// <param name="schedulerName">The named scheduler to create</param>
        /// <param name="reloadConfigFromDisk">If true, config is reloaded from disk instead of using optionally cached data</param>
        /// <returns>The named Scheduler configured in the configuration file.</returns>
        /// <seealso href="85482458-faf3-4de0-95c0-ef0b3709a1ea.htm" target="_blank">Scheduler configuration using .NET configuration files</seealso>
        Scheduler CreateFromConfig(string sectionName, string schedulerName, bool reloadConfigFromDisk);

        /// <summary>
        /// Creates a Scheduler with schedules from the specified configuration file (.config).
        /// </summary>
        /// <param name="configFile">Path to the .config file to use.</param>
        /// <param name="sectionName">The section name as defined in the .config files configSections/section</param>
        /// <returns>The named Scheduler configured in the configuration file.</returns>
        /// <seealso href="85482458-faf3-4de0-95c0-ef0b3709a1ea.htm" target="_blank">Scheduler configuration using .NET configuration files</seealso>
        Scheduler CreateFromConfigFile(string configFile, string sectionName);

        /// <summary>
        /// Creates a Scheduler with schedules from the specified configuration file (.config).
        /// </summary>
        /// <param name="configFile">Path to the .config file to use.</param>
        /// <param name="sectionName">The section name as defined in the .config files configSections/section</param>
        /// <param name="schedulerName">The named scheduler to create</param>
        /// <returns>The named Scheduler configured in the configuration file.</returns>
        /// <seealso href="85482458-faf3-4de0-95c0-ef0b3709a1ea.htm" target="_blank">Scheduler configuration using .NET configuration files</seealso>
        Scheduler CreateFromConfigFile(string configFile, string sectionName, string schedulerName);
    }

}