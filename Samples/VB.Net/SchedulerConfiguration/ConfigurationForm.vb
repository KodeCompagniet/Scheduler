Imports DemiCode.Scheduling.Configuration
Imports System.Xml
Imports DemiCode.Scheduling.Configuration.Schedules
Imports System.IO

Public Class ConfigurationForm

    Dim _pathToConfigurationFile As String
    Dim _schedulerSection As String

    Private Sub ConfigurationForm_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        ' ExecutablePath will be the name of the executable from which we will load and save configuration
        _pathToConfigurationFile = Application.ExecutablePath + ".config"

        ' Section name of our scheduler section in .config file
        _schedulerSection = "scheduling"

        ' Fill our checkbox list with days and load settings from config file
        FillDayList()
        LoadSettings()
    End Sub

    Private Sub saveButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles saveButton.Click
        SaveSettings()
    End Sub

    Private Sub FillDayList()
        For Each day As DayOfWeek In [Enum].GetValues(GetType(DayOfWeek))
            daysListBox.Items.Add(day, False)
        Next
    End Sub

    ''' <summary>
    ''' Load Scheduler configuration from .config file and populate GUI
    ''' </summary>
    ''' <remarks>NOTE: this method is hardcoded to look for a schedule named 'MyWeeklySchedule'</remarks>
    Private Sub LoadSettings()

        Dim settings As SchedulerSettings
        Dim scheduler As SchedulerElement

        ' Get the default scheduler 
        settings = SchedulerSettingsManager.GetConfigurationSection(_schedulerSection)
        scheduler = settings.Schedulers.Item(settings.DefaultScheduler)

        ' Loop through schedules and locate the WeeklySchedule called "MyWeeklySchedule"
        For Each schedule As ScheduleElementBase In scheduler

            If schedule.Name = "MyWeeklySchedule" And TypeOf schedule Is WeeklyScheduleElement Then
                Dim ws As WeeklyScheduleElement = schedule

                If Not ws.FromDate.IsEmpty Then
                    fromDate.Value = ws.FromDate.Value
                    fromDate.Checked = True
                Else
                    fromDate.Checked = False
                End If

                If Not ws.ToDate.IsEmpty Then
                    toDate.Value = ws.ToDate.Value
                    toDate.Checked = True
                Else
                    toDate.Checked = False
                End If

                Dim tod As TimeSpan = ws.TimeOfDay.Value
                timeOfDay.Value = New Date(2008, 1, 1, tod.Hours, tod.Minutes, tod.Seconds)

                daysListBox.Items.Clear()

                Dim days As List(Of DayOfWeek) = New List(Of DayOfWeek)(ws.Days.Value)
                For Each day As DayOfWeek In [Enum].GetValues(GetType(DayOfWeek))
                    If days.Contains(day) Then
                        daysListBox.Items.Add(day, True)
                    Else
                        daysListBox.Items.Add(day, False)
                    End If
                Next
            End If

        Next

    End Sub

    ''' <summary>
    ''' Collect data from GUI and save it to .config file.
    ''' </summary>
    Private Sub SaveSettings()

        Dim fromDateValue As DateTime
        Dim toDateValue As DateTime
        Dim timeOfDayValue As TimeSpan
        Dim selectedDays As List(Of DayOfWeek)

        fromDateValue = fromDate.Value
        timeOfDayValue = timeOfDay.Value.TimeOfDay

        If toDate.Checked Then
            toDateValue = toDate.Value
        Else
            toDateValue = DateTime.MaxValue
        End If

        selectedDays = New List(Of DayOfWeek)
        For Each day As DayOfWeek In daysListBox.CheckedItems
            selectedDays.Add(day)
        Next

        Dim settings As SchedulerSettings = SchedulerSettingsManager.GetWritableConfigurationSection(_schedulerSection, _pathToConfigurationFile)
        Dim defaultScheduler As SchedulerElement = settings.Schedulers(settings.DefaultScheduler)

        Dim schedule As WeeklyScheduleElement = New WeeklyScheduleElement("MyWeeklySchedule", fromDateValue, toDateValue, timeOfDayValue, selectedDays.ToArray())
        defaultScheduler.Add(schedule)

        Dim updatedConfigFile = Path.ChangeExtension(_pathToConfigurationFile, ".updated.config")
        SchedulerSettingsManager.SaveConfigurationSection(settings, updatedConfigFile)


        ' Note: to write back to the original config file use:
        ' SchedulerSettingsManager.SaveConfigurationSection(settings)

    End Sub

End Class
