using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using DemiCode.Scheduling;

namespace DemiCode.SchedulerService
{
    public class SchedulerHostedService : IHostedService, IDisposable
    {
        private readonly ILogger<SchedulerHostedService> _logger;
        private readonly IConfiguration _configuration;
        private readonly IHostApplicationLifetime _appLifetime;
        private Scheduler _scheduler;
        private FileSystemWatcher _fileWatcher;
        private readonly object _reconfigureLock = new object();

        public const string SchedulerServiceName = "DemiCode Scheduler Service";
        private const string ConfigSectionName = "scheduling";

        public SchedulerHostedService(
            ILogger<SchedulerHostedService> logger,
            IConfiguration configuration,
            IHostApplicationLifetime appLifetime)
        {
            _logger = logger;
            _configuration = configuration;
            _appLifetime = appLifetime;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("DemiCode Scheduler Service is starting.");

            _appLifetime.ApplicationStarted.Register(OnStarted);
            _appLifetime.ApplicationStopping.Register(OnStopping);
            _appLifetime.ApplicationStopped.Register(OnStopped);

            // Watch configuration file for changes
            SetupFileWatcher();
            _logger.LogDebug("Watching configuration file");

            // Create default scheduler from configuration
            _scheduler = SchedulerFactory.CreateFromConfig(ConfigSectionName);
            _logger.LogDebug("Scheduler constructed");

            // Start the engine
            StartScheduler(_scheduler);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("DemiCode Scheduler Service is stopping.");

            if (_scheduler != null)
            {
                _logger.LogDebug("Stopping scheduler");
                _scheduler.Stop();
            }

            return Task.CompletedTask;
        }

        private void StartScheduler(Scheduler scheduler)
        {
            scheduler.Start();
            _logger.LogDebug("Scheduler started");
        }

        private void SetupFileWatcher()
        {
            try
            {
                var configPath = Path.Combine(Directory.GetCurrentDirectory(), "app.config");
                var directory = Path.GetDirectoryName(configPath);
                var fileName = Path.GetFileName(configPath);

                _fileWatcher = new FileSystemWatcher(directory, fileName)
                {
                    NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size,
                    EnableRaisingEvents = true
                };

                _fileWatcher.Changed += OnConfigurationFileChanged;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not set up configuration file watcher. Configuration changes will require service restart.");
            }
        }

        private Scheduler ReconfigureExistingScheduler()
        {
            _logger.LogDebug("Trying to lock for reconfiguration...");
            lock (_reconfigureLock)
            {
                _logger.LogDebug("Reconfiguring Scheduler...");

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

        private void OnConfigurationFileChanged(object sender, FileSystemEventArgs e)
        {
            _logger.LogDebug("Configuration file changed");

            try
            {
                if (_scheduler != null)
                {
                    _scheduler = ReconfigureExistingScheduler();
                }
                else
                {
                    _scheduler = SchedulerFactory.CreateFromConfig(ConfigSectionName);
                    StartScheduler(_scheduler);
                }

                _logger.LogDebug("Scheduler reconfigured");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed during Scheduler reconfiguration");
            }
        }

        private void OnStarted()
        {
            _logger.LogInformation("DemiCode Scheduler Service has started.");
        }

        private void OnStopping()
        {
            _logger.LogInformation("DemiCode Scheduler Service is performing graceful shutdown.");
        }

        private void OnStopped()
        {
            _logger.LogInformation("DemiCode Scheduler Service has stopped.");
        }

        public void Dispose()
        {
            if (_fileWatcher != null)
            {
                _fileWatcher.Changed -= OnConfigurationFileChanged;
                _fileWatcher.Dispose();
                _fileWatcher = null;
            }

            if (_scheduler != null)
            {
                _scheduler.Dispose();
                _scheduler = null;
            }
        }
    }
}