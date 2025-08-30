using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using DemiCode.Scheduling;
using DemiCode.Scheduling.Alarms;
using DemiCode.Scheduling.Schedules;

namespace TestWinForm111
{
    public partial class TestForm : Form
    {
        private List<ISchedule> _scheduleList = new List<ISchedule>();
        private Scheduler _sc = null;

        public TestForm()
        {
            InitializeComponent();

            CreateScheduler();
        }

        private void CreateScheduler()
        {
            if (_sc != null)
            {
                _sc.Stop();
                _sc.Dispose();
                _sc = null;
            }

            _sc = SchedulerFactory.Create(_scheduleList, null);
            _sc.ScheduledAlarm += new EventHandler<AlarmEventArgs>(sc_ScheduledAlarm1);
            _sc.Start();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBox1.Text += DateTime.Now.ToString() + " rescheduled!" + Environment.NewLine;

            _sc.Stop();

            ISchedule s1 = new IntervalSchedule(DateTime.Now.AddSeconds(5), TimeSpan.FromSeconds(5), "interval" + _scheduleList.Count.ToString());
            _scheduleList.Add(s1);

            ISchedule s2 = new IntervalSchedule(DateTime.Now.AddSeconds(5),
                TimeSpan.FromSeconds(5), "interval" + _scheduleList.Count.ToString());
            _scheduleList.Add(s2);

            CreateScheduler();

            // Scenario 1
            /*
            textBox1.Text += DateTime.Now.ToString() + " rescheduled!" + Environment.NewLine;

            _sc.Stop();

            ISchedule s1 = new IntervalSchedule(DateTime.Now.AddSeconds(5), TimeSpan.FromSeconds(15), "interval1");
            _sc.Add(s1);

            ISchedule s2 = new IntervalSchedule(DateTime.Now.AddSeconds(5), 
                TimeSpan.FromSeconds(15), "interval2");
            _sc.Add(s2);

            _sc.Start();             
             */
        }

        void sc_ScheduledAlarm1(object sender, AlarmEventArgs e)
        {
            textBox1.Text += DateTime.Now.ToString() + " " + e.AdditionalData.ToString() + ": signalled " + _sc.Count.ToString() + " schedules" + Environment.NewLine;
        }

        void sc_ScheduledAlarm2(object sender, AlarmEventArgs e)
        {
            textBox1.Text += DateTime.Now.ToString() + " " + e.AdditionalData.ToString() +  Environment.NewLine;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //NOTE: adding eventhandlers while running works like a charm
            //foreach (var schedule in _sc)
            //{
            //    schedule.Alarm += new EventHandler<AlarmEventArgs>(sc_ScheduledAlarm1);
            //}

            _sc.Enabled = !_sc.Enabled;
            textBox1.Text += DateTime.Now.ToString() + " " + _sc.Enabled.ToString() + Environment.NewLine;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBox1.Text += DateTime.Now.ToString() + " rescheduled with individual eventhandlers!" + Environment.NewLine;

            _sc.Stop();

            ISchedule s1 = new IntervalSchedule(DateTime.Now.AddSeconds(5), TimeSpan.FromSeconds(10), "interval1");
            s1.Alarm += new EventHandler<AlarmEventArgs>(sc_ScheduledAlarm1);
            _sc.Add(s1);

            ISchedule s2 = new IntervalSchedule(DateTime.Now.AddSeconds(5),
                TimeSpan.FromSeconds(10), "interval2");
            s2.Alarm += new EventHandler<AlarmEventArgs>(sc_ScheduledAlarm2);
            _sc.Add(s2);

            _sc.Start();
        }
    }
}
