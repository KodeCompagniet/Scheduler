using System;
using System.Configuration;
using System.IO;
using System.Threading;

namespace DemiCode.Scheduling.Configuration
{
    internal delegate void ConfigurationFileChangedEventHandler(object sender, EventArgs e);

    internal class ConfigurationFileWatcher : IDisposable
    {
        private const int WATCHERTIMEOUT = 500;
        private Timer _timer;
        private FileSystemWatcher _watcher;
        private FileInfo _configFile;

        
        public event ConfigurationFileChangedEventHandler ConfigurationFileChanged;

        public ConfigurationFileWatcher()
        {
            _timer = new Timer(new TimerCallback(OnWatchedFileChange), null, Timeout.Infinite, Timeout.Infinite);

            _configFile = GetConfigFileInfo();
            
            _watcher = new FileSystemWatcher();
            _watcher.Path = _configFile.DirectoryName;
            _watcher.Filter = _configFile.Name;
            _watcher.NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.LastWrite | NotifyFilters.FileName;

            _watcher.Changed += new FileSystemEventHandler(Watcher_OnChanged);
            _watcher.Created += new FileSystemEventHandler(Watcher_OnChanged);
            _watcher.Deleted += new FileSystemEventHandler(Watcher_OnChanged);
            _watcher.Renamed += new RenamedEventHandler(Watcher_OnRenamed);

            _watcher.EnableRaisingEvents = true;
        }

        /// <summary>
        /// Get file info on the config file we're watching.
        /// </summary>
        public FileInfo ConfigFile
        {
            get { return _configFile; }
        }

        private static FileInfo GetConfigFileInfo()
        {
            return new FileInfo(AppDomain.CurrentDomain.SetupInformation.ConfigurationFile);
        }

        private void Watcher_OnChanged(object source, FileSystemEventArgs e)
        {
            _timer.Change(WATCHERTIMEOUT, Timeout.Infinite);
        }

        private void Watcher_OnRenamed(object source, RenamedEventArgs e)
        {
            _timer.Change(WATCHERTIMEOUT, Timeout.Infinite);
        }

        private void OnConfigurationFileChanged()
        {
            if (ConfigurationFileChanged != null)
            {
                ConfigurationFileChanged(this, EventArgs.Empty);
            }
        }

        private void OnWatchedFileChange(object state)
        {
            OnConfigurationFileChanged();
        }

        #region IDisposable pattern

        private bool _disposed = false;
        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Cleanup managed resources
                    if (_watcher != null)
                    {
                        _watcher.Dispose();
                        _watcher = null;
                    }

                    if (_timer != null)
                    {
                        _timer.Dispose();
                        _timer = null;
                    }
                }
                _disposed = true;
            }
        }

        ///<summary>
        ///Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        ///</summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}
