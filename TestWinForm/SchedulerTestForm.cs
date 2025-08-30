using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DemiCode.Scheduling;
using DemiCode.Scheduling.Schedules;
using DemiCode.Scheduling.Alarms;

namespace TestWinForm
{
	public partial class SchedulerTestForm : Form
	{
		public SchedulerTestForm()
		{
			InitializeComponent();
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			//Scheduler scheduler = new Scheduler(new CompositeSchedule(CreateSchedule(), "composite"));
			Scheduler scheduler = SchedulerFactory.Create(CreateSchedules());
			ISchedule schedule = new OneShotSchedule(new DateTime(2007, 7, 1, 15, 30, 0));
			scheduler.Add(schedule);
			scheduler.ScheduledAlarm += new EventHandler<AlarmEventArgs>(scheduler_ScheduledAlarm);
			scheduler.EndOfSchedule += new EventHandler<EventArgs>(scheduler_EndOfSchedule);
			scheduler.Start();
		}

		private IEnumerable<ISchedule> CreateSchedules()
		{
			List<ISchedule> l = new List<ISchedule>();
			DateTime now = DateTime.Now;
			l.Add(new IntervalCountedSchedule(now.AddSeconds(10), 10, TimeSpan.FromSeconds(5), "Hvert 5. sekund (10x)"));
			l.Add(new IntervalCountedSchedule(now.AddSeconds(21), 3, TimeSpan.FromSeconds(7), "Hvert 7. sekund (3x)"));
			l.Add(new IntervalCountedSchedule(now.AddSeconds(25), 4, TimeSpan.FromSeconds(10), "Hvert 10. sekund (4x)"));

			l[1].Alarm += new EventHandler<AlarmEventArgs>(Form1_Alarm);
			return l;
		}

		void Form1_Alarm(object sender, AlarmEventArgs e)
		{
			listBox1.Items.Add("Special event handler for 7 sec IntervalCountedSchedule.");
		}

		private void scheduler_EndOfSchedule(object sender, EventArgs e)
		{
			listBox1.Items.Add("End of schedule.");
		}


		private void scheduler_ScheduledAlarm(object sender, AlarmEventArgs e)
		{
			listBox1.Items.Add(string.Format("{0:hh:mm:ss} - {1:hh:mm:ss} - {2}",
				e.SignalTime, DateTime.Now, e.AdditionalData));
		}
	}
}