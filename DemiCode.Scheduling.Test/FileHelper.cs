using System;
using System.Collections.Generic;
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

        public static void BackupConfigFile()
        {
            string configFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
            string configBackup = Path.ChangeExtension(configFile, "backup");
            File.Copy(configFile, configBackup, true);
        }

        public static void ReplaceConfigFile(string file)
        {
            string configFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
            string configBackup = Path.ChangeExtension(configFile, "backup");

            File.Copy(configFile, configBackup, true);
            File.Copy(file, configFile, true);
        }

        public static void RestoreConfigFile()
        {
            string configFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
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
