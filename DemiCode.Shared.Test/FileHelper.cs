using System;
using System.IO;

namespace DemiCode.Shared.Test
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
            var configFile = System.Configuration.ConfigurationManager.OpenExeConfiguration(System.Configuration.ConfigurationUserLevel.None).FilePath;
            var configBackup = Path.ChangeExtension(configFile, "backup");
            File.Copy(configFile, configBackup, true);
        }

        public static void ReplaceConfigFile(string file)
        {
            ReplaceConfigFile(null, file);
        }

        public static void ReplaceConfigFile(string path, string file)
        {
            var configFile = System.Configuration.ConfigurationManager.OpenExeConfiguration(System.Configuration.ConfigurationUserLevel.None).FilePath;
            var configBackup = Path.ChangeExtension(configFile, "backup");

            var absFile = file;
            if (!String.IsNullOrEmpty(path))
            {
                absFile = Path.Combine(path, file);
            }
            File.Copy(configFile, configBackup, true);
            File.Copy(absFile, configFile, true);
        }

        public static void RestoreConfigFile()
        {
            var configFile = System.Configuration.ConfigurationManager.OpenExeConfiguration(System.Configuration.ConfigurationUserLevel.None).FilePath;
            var configBackup = Path.ChangeExtension(configFile, "backup");

            if (File.Exists(configBackup))
            {
                File.Copy(configBackup, configFile, true);
            }
        }

    }
}
