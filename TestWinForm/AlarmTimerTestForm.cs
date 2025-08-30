using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DemiCode.Scheduling;
using DemiCode.Scheduling.Alarms;

namespace TestWinForm
{
	public partial class AlarmTimerTestForm : Form
	{
		public AlarmTimerTestForm()
		{
			InitializeComponent();
		}

		private void AlarmTimerTestForm_Load(object sender, EventArgs e)
		{
			IAlarmTimer alarmTimer = new AlarmTimer();
			alarmTimer.Elapsed += new EventHandler<AlarmEventArgs>(alarmTimer_Elapsed);

			//alarmTimer.Enable(new DateTime(2006, 1, 10, 20, 0, 0));
			alarmTimer.Enable(DateTime.Now.AddSeconds(2));
		}

		void alarmTimer_Elapsed(object sender, AlarmEventArgs e)
		{
			listBox1.Items.Add(string.Format("{0} - {1}", DateTime.Now, e.SignalTime));
		}
	}
}