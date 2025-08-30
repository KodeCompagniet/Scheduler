Imports DemiCode.Scheduling
Imports DemiCode.Scheduling.Alarms

Public Class SampleService

    Public Const SchedulerServiceName As String = "DemiCode Scheduler Service VB.Net sample"

    Private _scheduler As Scheduler

    Protected Overrides Sub OnStart(ByVal args() As String)
        ' 1. Create default scheduler from configuration
        _scheduler = SchedulerFactory.CreateFromConfig("scheduling")

        ' 2. Hook up to events
        AddHandler _scheduler.ScheduledAlarm, AddressOf scheduler_ScheduledAlarm
        AddHandler _scheduler.Item("FiveSecInterval").Alarm, AddressOf fiveSecInterval_Alarm

        ' 3. Start the engine
        _scheduler.Start()

    End Sub

    Protected Overrides Sub OnPause()
        _scheduler.Stop()
    End Sub

    Protected Overrides Sub OnContinue()
        _scheduler.Start()
    End Sub

    Protected Overrides Sub OnStop()
        ' Dispose our scheduler when the service is stopped
        If Not _scheduler Is Nothing Then
            _scheduler.Dispose()
            _scheduler = Nothing
        End If
    End Sub

    Sub fiveSecInterval_Alarm(ByVal sender As Object, ByVal e As AlarmEventArgs)
        EventLog.WriteEntry("Scheduler Five Sec Interval", String.Format("Event alarm time {0}, signaled at {1:yyyy-MM-dd HH:mm:ss:ff}", e.SignalTime, e.ActualSignalTime))
    End Sub
    Sub scheduler_ScheduledAlarm(ByVal sender As Object, ByVal e As AlarmEventArgs)
        EventLog.WriteEntry("Scheduler", String.Format("Event alarm time {0}, signaled at {1}", e.SignalTime, e.ActualSignalTime))
    End Sub

End Class
