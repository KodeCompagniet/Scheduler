using System;
using System.Configuration;
using System.Globalization;
using System.IO;
using DemiCode.Configuration;

namespace DemiCode.Scheduling.Configuration
{
    /// <summary>
    /// Create configuration instances from config files.
    /// </summary>
    public static class SchedulerSettingsManager
    {
        /// <summary>
        /// Serialization state provider used in conjunction with <see cref="PolymorphicElementCollection{V}"/>.
        /// </summary>
        public class SerializationStateProvider : ISerializationStateProvider
        {
            bool ISerializationStateProvider.IsSerializing
            {
                get { return _isSavingConfiguration; }
            }
        }

        [ThreadStatic]
        private static bool _isSavingConfiguration;

        /// <summary>
        /// Load the specified configuration section from .config. 
        /// </summary>
        /// <param name="sectionName">Name of the section to read.</param>
        /// <returns>A <see cref="SchedulerSettings"/> corresponding to <paramref name="sectionName"/>.</returns>
        public static SchedulerSettings GetConfigurationSection(string sectionName)
        {
            return GetConfigurationSection(sectionName, false);
        }

        /// <summary>
        /// Load the specified configuration section from .config. 
        /// </summary>
        /// <param name="sectionName">Name of the section to read.</param>
        /// <param name="readConfigFromDisk">If true, the .Net config file cache will be invalidated for this section before the section is loaded.
        /// If false (default), the section will be read from the cache. Go to <see cref="ConfigurationManager.RefreshSection"/> for more details.</param>
        /// <returns>A <see cref="SchedulerSettings"/> corresponding to <paramref name="sectionName"/>.</returns>
        public static SchedulerSettings GetConfigurationSection(string sectionName, bool readConfigFromDisk)
        {
            if (sectionName == null)
                throw new ArgumentNullException("sectionName", "Section name must be a valid section in the configuration file");

            if (readConfigFromDisk)
            {
                ConfigurationManager.RefreshSection(sectionName);
            }

            var ss = ConfigurationManager.GetSection(sectionName) as SchedulerSettings;
            if (ss == null)
                throw new ArgumentException(String.Format(CultureInfo.InvariantCulture, "Section '{0}' does not point to a valid SchedulerSettings section", sectionName), "sectionName");

            return ss;
        }

        /// <summary>
        /// Get a configuration section from the specified configuration file.
        /// </summary>
        /// <param name="sectionName">Name of the section to read.</param>
        /// <param name="configFile">Full path to a configuration file.</param>
        /// <returns>A <see cref="SchedulerSettings"/> corresponding to <paramref name="sectionName"/> in the <paramref name="configFile"/> file.</returns>
        public static SchedulerSettings GetConfigurationSection(string configFile, string sectionName)
        {
            if (sectionName == null)
                throw new ArgumentNullException("sectionName", "Section name must be a valid section in the configuration file");

            if (configFile == null)
                throw new ArgumentNullException("configFile");

            if (!File.Exists(configFile))
                throw new ArgumentException("Specified configuration file does not exist", "configFile");

            var map = new ExeConfigurationFileMap
            {
                ExeConfigFilename = configFile
            };

            var configuration = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);
            var settings = configuration.GetSection(sectionName) as SchedulerSettings;
            if (settings == null)
                throw new ArgumentException(String.Format(CultureInfo.InvariantCulture, "Section '{0}' does not point to a valid SchedulerSettings section", sectionName), "sectionName");

            return settings;
        }


        /// <summary>
        /// Open a configuration section for writing.
        /// </summary>
        /// <param name="sectionName">Name of the section to read/write.</param>
        /// <param name="configFile">Full path to a configuration file.</param>
        /// <remarks>If <paramref name="configFile"/> does not exist, a new empty section is returned</remarks>
        /// <returns>A writable <see cref="SchedulerSettings"/> corresponding to <paramref name="sectionName"/>.</returns>
        public static SchedulerSettings GetWritableConfigurationSection(string sectionName, string configFile)
        {
            var map = new ExeConfigurationFileMap { ExeConfigFilename = configFile };
            var configuration = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);
            var settings = configuration.GetSection(sectionName) as SchedulerSettings ??
                           AddSection(configuration, sectionName);

            settings.WritableConfiguration = configuration;

            return settings;
        }

        private static SchedulerSettings AddSection(System.Configuration.Configuration configuration, string sectionName)
        {
            if (configuration.Sections.Get(sectionName) != null)
            {
                throw new InvalidOperationException(String.Format("Section '{0}' already exist but is not a compatible type.", sectionName));
            }

            configuration.Sections.Add(sectionName, new SchedulerSettings());
            var settings = configuration.GetSection(sectionName) as SchedulerSettings;
            if (settings == null)
                throw new InvalidOperationException("Could not add a new configuration section");

            return settings;
        }

        /// <summary>
        /// Open a configuration section for writing in the current application configuration file.
        /// </summary>
        /// <param name="sectionName">Name of the section to read/write.</param>
        /// <returns>A writable <see cref="SchedulerSettings"/> corresponding to <paramref name="sectionName"/>.</returns>
        public static SchedulerSettings GetWritableConfigurationSection(string sectionName)
        {
            var configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var settings = configuration.GetSection(sectionName) as SchedulerSettings ??
                           AddSection(configuration, sectionName);

            settings.WritableConfiguration = configuration;

            return settings;
        }

        /// <summary>
        /// Save a configuration section to file.
        /// </summary>
        /// <remarks>The settings object must have been opened using <see cref="GetWritableConfigurationSection(string, string)"/>.</remarks>
        /// <param name="settings">The settings to save.</param>
        /// <param name="saveAsConfigFile">Full path to the configuration file.</param>
        public static void SaveConfigurationSection(SchedulerSettings settings, string saveAsConfigFile)
        {
            if (settings.WritableConfiguration == null)
                throw new ArgumentException("The settings section is not writable. Use GetWritableConfigurationSection to open a writable section", "settings");

            if (CheckFileOrigin(settings, saveAsConfigFile))
            {
                SaveConfigurationSection(settings);
            }
            else
            {


                _isSavingConfiguration = true;
                try
                {
                    settings.WritableConfiguration.SaveAs(saveAsConfigFile, ConfigurationSaveMode.Full);
                }
                finally
                {
                    _isSavingConfiguration = false;
                }
            }
        }

        /// <summary>
        /// Return true if the specified file is the same file as the settings section was originally opened from.
        /// </summary>
        private static bool CheckFileOrigin(ConfigurationElement settings, string saveAsConfigFile)
        {
            if (settings.ElementInformation == null)
                return false;
            return Path.GetFullPath(saveAsConfigFile).Equals(settings.ElementInformation.Source);
        }

        /// <summary>
        /// Save a configuration section.
        /// </summary>
        /// <remarks>The settings object must have been opened using <see cref="GetWritableConfigurationSection(string)"/>.</remarks>
        /// <param name="settings">The settings to save.</param>
        public static void SaveConfigurationSection(SchedulerSettings settings)
        {
            SaveConfigurationSection(settings, ConfigurationSaveMode.Full);
        }

        /// <summary>
        /// Save a configuration section.
        /// </summary>
        /// <remarks>The settings object must have been opened using <see cref="GetWritableConfigurationSection(string)"/>.</remarks>
        /// <param name="settings">The settings to save.</param>
        /// <param name="configurationSaveMode">A <see cref="ConfigurationSaveMode"/> value that determines which property values to save.</param>
        public static void SaveConfigurationSection(SchedulerSettings settings, ConfigurationSaveMode configurationSaveMode)
        {
            if (settings.WritableConfiguration == null)
                throw new ArgumentException("The settings section is not writable. Use GetWritableConfigurationSection to open a writable section", "settings");

            _isSavingConfiguration = true;
            try
            {
                settings.WritableConfiguration.Save(configurationSaveMode);
            }
            finally
            {
                _isSavingConfiguration = false;
            }
        }

    }
}
