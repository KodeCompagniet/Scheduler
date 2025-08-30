using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;
using System.Xml;
using DemiCode.Scheduling;
using DemiCode.Scheduling.Alarms;
using DemiCode.Scheduling.Schedules;
using System.Collections.ObjectModel;

namespace SampleService
{


    public partial class SampleService : ServiceBase
    {
        private Scheduler _scheduler = null;

        public const string SchedulerServiceName = "DemiCode Scheduler Service sample";


        public SampleService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            // 1. Create default scheduler from configuration
            _scheduler = SchedulerFactory.CreateFromConfig("scheduling");

            // 2. Hook up to events
            _scheduler.ScheduledAlarm += new EventHandler<AlarmEventArgs>(_scheduler_ScheduledAlarm);
            _scheduler["FiveSecInterval"].Alarm += new EventHandler<AlarmEventArgs>(fiveSecInterval_Alarm);

            // 3. Start the engine
            _scheduler.Start();
        }

        private static void fiveSecInterval_Alarm(object sender, AlarmEventArgs e)
        {
            EventLog.WriteEntry("Scheduler Five Sec Interval", String.Format("Event alarm time {0}, signaled at {1:yyyy-MM-dd HH:mm:ss:ff}", e.SignalTime, e.ActualSignalTime));
        }

        private static void _scheduler_ScheduledAlarm(object sender, AlarmEventArgs e)
        {
            EventLog.WriteEntry("Scheduler", String.Format("Event alarm time {0}, signaled at {1}", e.SignalTime, e.ActualSignalTime));
        }

        protected override void OnPause()
        {
            _scheduler.Stop();
        }

        protected override void OnContinue()
        {
            _scheduler.Start();
        }

        protected override void OnStop()
        {
            if (_scheduler != null)
            {
                _scheduler.Dispose();
                _scheduler = null;
            }
        }
    }
}
