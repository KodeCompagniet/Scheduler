using System;
using System.Threading;
using DemiCode.Scheduling;
using DemiCode.Scheduling.Configuration;
using log4net;

namespace TestConsole
{
    public class Job
    {
        public void Execute()
        {
            
        }
    }

	class Program
	{
        private static ILog _log = LogManager.GetLogger(typeof(Program));

        private static ConfigurationFileWatcher _fileWatcher = null;
        private static Scheduler _scheduler = null;

        protected static void OnStart()
        {
            // Watch configuration file for changes
            _fileWatcher = new ConfigurationFileWatcher();
            _fileWatcher.ConfigurationFileChanged += new ConfigurationFileChangedEventHandler(_fileWatcher_ConfigurationFileChanged);
            _log.Debug("Watching configuration file");

            // Create default scheduler from configuration
            _scheduler = SchedulerFactory.CreateFromConfig("scheduling");
            _log.Debug("Scheduler constructed");

            // Start the engine
            _scheduler.Start();
            _log.Debug("Scheduler started");
        }

        protected static void OnStop()
        {
            if (_scheduler != null)
            {
                _log.Debug("Stopping scheduler");

                _scheduler.Dispose();
                _scheduler = null;
            }
        }

        private static Scheduler ReconfigureExistingScheduler()
        {
            lock (_scheduler)
            {
                _log.Debug("Reconfiguring Scheduler...");

                bool runningState = _scheduler.IsRunning;
                if (runningState)
                {
                    _scheduler.Stop();
                }
                _scheduler.Dispose();
                _scheduler = null;

                Scheduler newScheduler = SchedulerFactory.CreateFromConfig("scheduling", true);
                if (runningState)
                {
                    newScheduler.Start();
                }

                return newScheduler;
            }
        }

        static void _fileWatcher_ConfigurationFileChanged(object sender, EventArgs e)
        {
            _log.Debug("Configuration file changed");

            try
            {
                if (_scheduler != null)
                {
                    _log.Debug("Trying to lock");
                    if (!Monitor.TryEnter(_scheduler))
                    {
                        _log.Debug("Lock already taken");
                        return;
                    }

                    _scheduler = ReconfigureExistingScheduler();
                }
                else
                {
                    _scheduler = SchedulerFactory.CreateFromConfig("scheduling");
                    _scheduler.Start();
                }

                _log.Debug("Scheduler reconfigured");
            }
            catch (Exception ex)
            {
                _log.Error("Failed during Scheduler reconfiguration", ex);
            }
        }


		static void Main(string[] args)
		{
			OnStart();

			Console.WriteLine("Press ENTER...");
			Console.ReadLine();

		    OnStop();
		}

	}
}
