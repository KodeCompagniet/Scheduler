using System;
using ApplicationTaskConsole.Tasks;
using DemiCode.Scheduling;
using DemiCode.Scheduling.Schedules;

namespace ApplicationTaskConsole
{
    class Program
    {
        static void Main()
        {
            // 1. Create default scheduler from configuration
            using (Scheduler scheduler = SchedulerFactory.CreateFromConfig("scheduling"))
            {
                //// 2. Create one (or more) schedules
                //IntervalSchedule schedule = new IntervalSchedule(DateTime.Now, TimeSpan.FromSeconds(5));
                //scheduler.Add(schedule);

                //// 3. Create our task
                //ApplicationTask task = new ApplicationTask("cmd", "/K \"echo Application executed\"");
                //scheduler.Tasks.Add(task);

                // 4. Start the engine
                scheduler.Start();

                // 5. Wait for user input
                Console.WriteLine("Press [ENTER] to exit");
                Console.ReadLine();

                // 7. Stop the engine
                scheduler.Stop();
            }
            
        }
    }
}
