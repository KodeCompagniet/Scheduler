using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Text;

namespace DemiCode.Scheduling.Test
{
    internal static class FileHelper
    {
        /// <summary>
        /// Update the file last modified timestamp to now.
        /// </summary>
        public static void TouchFile(string file)
        {
            File.SetLastWriteTime(file, DateTime.Now);
        }

        private static string GetConfigurationFile()
        {
            // In .NET Core/5+, we need to use ConfigurationManager
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            return config.FilePath;
        }

        public static void BackupConfigFile()
        {
            string configFile = GetConfigurationFile();
            string configBackup = Path.ChangeExtension(configFile, "backup");
            File.Copy(configFile, configBackup, true);
        }

        public static void ReplaceConfigFile(string file)
        {
            string configFile = GetConfigurationFile();
            string configBackup = Path.ChangeExtension(configFile, "backup");

            File.Copy(configFile, configBackup, true);
            File.Copy(file, configFile, true);
        }

        public static void RestoreConfigFile()
        {
            string configFile = GetConfigurationFile();
            string configBackup = Path.ChangeExtension(configFile, "backup");

            if (File.Exists(configBackup))
            {
                File.Copy(configBackup, configFile, true);
            }
        }

        public static void Copy(string source, string destination)
        {
            File.Copy(source, destination, true);
        }
    }
}
