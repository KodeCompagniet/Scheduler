using System;
using DemiCode.Scheduling;
using DemiCode.Scheduling.Alarms;

namespace SchedulerConsole
{
    internal static class Program
    {
        static void Main()
        {
            // 1. Create default scheduler from configuration
            using (Scheduler _scheduler = SchedulerFactory.CreateFromConfig("scheduling"))
            {
                // 2. Hook up to events
                _scheduler["Weekly"].Alarm += new EventHandler<AlarmEventArgs>(weekly_Alarm);
                _scheduler["Interval"].Alarm += new EventHandler<AlarmEventArgs>(interval_Alarm);

                // 3. Start the engine
                _scheduler.Start();

                // 4. Wait for user input
                Console.WriteLine("Press [ENTER] to exit");
                Console.ReadLine();

                // 5. Stop the engine
                _scheduler.Stop();
            }
        }

        private static void interval_Alarm(object sender, AlarmEventArgs e)
        {
            Console.WriteLine("Interval: alarm time {0:HH:mm:ss:ff}, signaled at {1:HH:mm:ss:ff}, actual at {2:HH:mm:ss:ff}", e.SignalTime, e.ActualSignalTime, DateTime.Now);
        }

        private static void weekly_Alarm(object sender, AlarmEventArgs e)
        {
            Console.WriteLine("Weekly: Event alarm time {0}, signaled at {1}", e.SignalTime, e.ActualSignalTime);
        }

    }
}