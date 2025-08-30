using System;
using System.Runtime.Serialization;
using System.ServiceProcess;
using DemiCode.Scheduling;
using DemiCode.Scheduling.Configuration;
using log4net;

namespace DemiCode.SchedulerService
{
    public partial class ServiceImplementation : ServiceBase
    {
        private static readonly ILog _log = LogManager.GetLogger(typeof(ServiceImplementation));

        public ServiceImplementation()
        {
            InitializeComponent();
        }

        private ConfigurationFileWatcher _fileWatcher;
        private Scheduler _scheduler;

        public const string SchedulerServiceName = "DemiCode Scheduler Service";
        private const string ConfigSectionName = "scheduling";

        protected override void OnStart(string[] args)
        {
            // Watch configuration file for changes
            _fileWatcher = new ConfigurationFileWatcher();
            _fileWatcher.ConfigurationFileChanged += _fileWatcher_ConfigurationFileChanged;
            _log.Debug("Watching configuration file");

            // Create default scheduler from configuration
            _scheduler = SchedulerFactory.CreateFromConfig(ConfigSectionName);
            _log.Debug("Scheduler constructed");

            // Start the engine
            StartScheduler(_scheduler);
        }

        private void StartScheduler(Scheduler scheduler)
        {
            scheduler.Start();

            _log.Debug("Scheduler started");
        }

        private readonly object _reconfigureLock = new object();
        private Scheduler ReconfigureExistingScheduler()
        {
            _log.Debug("Trying to lock for reconfiguration...");
            lock (_reconfigureLock)
            {
                _log.Debug("Reconfiguring Scheduler...");

                bool runningState = _scheduler.IsRunning;
                if (runningState)
                {
                    _scheduler.Stop();
                }
                _scheduler.Dispose();
                _scheduler = null;

                Scheduler newScheduler = SchedulerFactory.CreateFromConfig(ConfigSectionName, true);
                if (runningState)
                {
                    StartScheduler(newScheduler);
                }

                return newScheduler;
            }
        }

        void _fileWatcher_ConfigurationFileChanged(object sender, EventArgs e)
        {
            _log.Debug("Configuration file changed");

            try
            {
                if (_scheduler != null)
                {
                    _scheduler = ReconfigureExistingScheduler();
                }
                else
                {
                    _scheduler = SchedulerFactory.CreateFromConfig(ConfigSectionName);
                    StartScheduler( _scheduler);
                }

                _log.Debug("Scheduler reconfigured");
            }
            catch (Exception ex)
            {
                _log.Error("Failed during Scheduler reconfiguration", ex);
            }
        }

        protected override void OnPause()
        {
            _log.Debug("Pausing scheduler");

            _scheduler.Stop();
        }

        protected override void OnContinue()
        {
            _log.Debug("Continuing scheduler");
            StartScheduler(_scheduler);
        }

        protected override void OnStop()
        {
            if (_scheduler != null)
            {
                _log.Debug("Stopping scheduler");

                _scheduler.Dispose();
                _scheduler = null;
            }
        }
    }
}
