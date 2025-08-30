Imports DemiCode.Scheduling

Module Program

    Sub Main()
        ' 1. Create default scheduler from configuration
        Using _scheduler As Scheduler = SchedulerFactory.CreateFromConfig("scheduling")

            ' 2. Start the engine
            _scheduler.Start()

            ' 3. Wait for user input
            Console.WriteLine("Press [ENTER] to exit")
            Console.ReadLine()

            ' 4. Stop the engine
            _scheduler.Stop()
        End Using
    End Sub

End Module
