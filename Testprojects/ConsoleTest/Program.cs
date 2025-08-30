using System;
using System.Collections.Generic;
using DemiCode.Logging;
using DemiCode.Scheduling;
using DemiCode.Scheduling.Alarms;
using DemiCode.Scheduling.Schedules;

namespace ConsoleTest
{
    class Program
    {
        private static List<ISchedule> list = new List<ISchedule>();
        private static DateTime _lastPocketRun = DateTime.MinValue; //.Now.AddYears(-1);
        private static DateTime _lastDealerRun = _lastPocketRun;
        private static Scheduler _sc;

        private static void CreateScheduler()
        {
            if (_sc != null)
            {
                _sc.Stop();
                _sc.Dispose();
                _sc = null;
            }

            ILoggerFactory loggerFactory = new ConsoleLoggerFactory();
            //_sc = SchedulerFactory.Create(list, loggerFactory, null);
            _sc = SchedulerFactory.Create(list);
            _sc.Start();
        }

        private static ISchedule Find(string key)
        {
            lock (_sc)
            {
                foreach (ISchedule s in _sc)
                {
                    if (s.AdditionalData != null && s.AdditionalData.ToString().Equals(key))
                        return s;
                }
            }
            return null;
        }

        private static void UnSchedule(object key)
        {
            ISchedule old = Find(key.ToString());
            if (old != null)
            {
                list.Remove(old);
            }
        }

        private static void Schedule(string key, DateTime lastRun)
        {
            TimeSpan interval = TimeSpan.FromSeconds(2);
            UnSchedule(key);
            ISchedule s1 = new IntervalSchedule(key, new DateTime(lastRun.Year, 
                lastRun.Month, lastRun.Day, lastRun.Hour, lastRun.Minute, lastRun.Second, 0), interval, key); // because of bug when old firstoccurence
            s1.Alarm += sc_ScheduledAlarm;
            list.Add(s1);
        }

        private static void PrintSchedules()
        {
            lock (_sc)
            {
                foreach (ISchedule s in _sc)
                {
                    Console.WriteLine("Schedule: " + s);
                }
            }
        }

        private static void sc_ScheduledAlarm(object sender, AlarmEventArgs e)
        {
            string key = "(ukjent)";
            if (e.AdditionalData != null)
            {
                key = e.AdditionalData.ToString();
                if (key.StartsWith("Pocket"))
                    _lastPocketRun = DateTime.Now;
                else if (key.StartsWith("Dealer"))
                    _lastDealerRun = DateTime.Now;

                Console.WriteLine( DateTime.Now + " " + key + ": signalled" );
            }
            else
            {
                Console.WriteLine( DateTime.Now + " key unknown: signalled" );
            }
        }

        static void Main(string[] args)
        {
            CreateScheduler();
            bool run = true;
            while (run)
            {
                string cmd = Console.ReadLine();
                switch (cmd)
                {
                    case "q":
                        run = false;
                        break;
                    case "b":
                        ScheduleBoth();
                        break;
                    case "r":
                        RemoveSchedule();
                        break;
                    case "a":
                        AddSchedule();
                        break;
                    case "p":
                        PrintSchedules();
                        break;
                }
            }

            if (_sc != null)
            {
                _sc.Stop();
                _sc.Dispose();
            }
        }

        private static void RemoveSchedule()
        {
            UnSchedule("Dealer");
            CreateScheduler();
            Console.WriteLine(DateTime.Now.ToString() + " Dealer unscheduled");
        }

        private static void AddSchedule()
        {
            Console.WriteLine(DateTime.Now.ToString() + " Dealer scheduled at " + _lastDealerRun);
            Schedule("Dealer", _lastDealerRun);
            CreateScheduler();
        }

        private static void ScheduleBoth()
        {
            Console.Clear();
            Schedule("Pocket", _lastPocketRun);
            Schedule("Dealer", _lastDealerRun);
            CreateScheduler();

            Console.WriteLine(DateTime.Now.ToString() + " both rescheduled!");
        }
    }
}
