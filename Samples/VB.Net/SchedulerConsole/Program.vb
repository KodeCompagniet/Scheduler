Imports DemiCode.Scheduling
Imports DemiCode.Scheduling.Alarms

Module Program

    Sub Main()
        ' 1. Create default scheduler from configuration
        Using _scheduler As Scheduler = SchedulerFactory.CreateFromConfig("scheduling")
            ' 2. Hook up to events
            AddHandler _scheduler.Item("Interval").Alarm, AddressOf interval_Alarm

            ' 3. Start the engine
            _scheduler.Start()

            ' 4. Wait for user input
            Console.WriteLine("Press [ENTER] to exit")
            Console.ReadLine()

            ' 5. Stop the engine
            _scheduler.Stop()
        End Using
    End Sub

    Private Sub interval_Alarm(ByVal sender As Object, ByVal e As AlarmEventArgs)
        Console.WriteLine("Interval: alarm time {0:HH:mm:ss:ff}, signaled at {1:HH:mm:ss:ff}, actual at {2:HH:mm:ss:ff}", e.SignalTime, e.ActualSignalTime, DateTime.Now)
    End Sub

End Module
