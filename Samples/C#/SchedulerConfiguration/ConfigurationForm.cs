using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using DemiCode.Scheduling.Configuration;
using DemiCode.Scheduling.Configuration.Schedules;

namespace SchedulerConfiguration
{
    public partial class ConfigurationForm : Form
    {
        private string _pathToConfigurationFile;
        private string _schedulerSection;

        public ConfigurationForm()
        {
            InitializeComponent();
        }

        private void ConfigurationForm_Load(object sender, EventArgs e)
        {
            // ExecutablePath will be the name of the executable from which we will load and save configuration
            _pathToConfigurationFile = Application.ExecutablePath + ".config";

            // Section name of our scheduler section in .config file
            _schedulerSection = "scheduling";

            // Fill our checkbox list with days and load settings from config file
            FillDayList();

            LoadSettings();
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            SaveSettings();
        }


        private void FillDayList()
        {
            foreach (DayOfWeek day in Enum.GetValues(typeof(DayOfWeek)))
            {
                daysListBox.Items.Add(day, false);
            }
        }

        /// <summary>
        /// Load Scheduler configuration from .config file and populate GUI
        /// </summary>
        /// <remarks>NOTE: this method is hardcoded to look for a schedule named 'MyWeeklySchedule'</remarks>
        private void LoadSettings()
        {
            SchedulerSettings settings = SchedulerSettingsManager.GetConfigurationSection(_pathToConfigurationFile, _schedulerSection);
            SchedulerElement scheduler = settings.Schedulers[settings.DefaultScheduler];

            foreach (ScheduleElementBase schedule in scheduler)
            {
                WeeklyScheduleElement ws = schedule as WeeklyScheduleElement;
                if (ws == null || schedule.Name != "MyWeeklySchedule") continue;
                
                if (ws.FromDate.IsEmpty)
                {
                    fromDate.Checked = false;
                }
                else
                {
                    fromDate.Value = ws.FromDate.Value;
                    fromDate.Checked = true;
                }

                if (ws.ToDate.IsEmpty)
                {
                    toDate.Checked = false;
                }
                else
                {
                    toDate.Value = ws.ToDate.Value;
                    toDate.Checked = true;
                }

                TimeSpan tod = ws.TimeOfDay.Value;
                timeOfDay.Value = new DateTime(2008, 1, 1, tod.Hours, tod.Minutes, tod.Seconds);

                daysListBox.Items.Clear();
                List<DayOfWeek> days = new List<DayOfWeek>(ws.Days.Value);
                foreach (DayOfWeek day in Enum.GetValues(typeof(DayOfWeek)))
                {
                    if (days.Contains(day))
                    {
                        daysListBox.Items.Add(day, true);
                    }
                    else
                    {
                        daysListBox.Items.Add(day, false);
                    }
                }
            }
        }

        /// <summary>
        /// Collect data from GUI and save it to .config file.
        /// </summary>
        private void SaveSettings()
        {
            SchedulerSettings settings = SchedulerSettingsManager.GetWritableConfigurationSection(_schedulerSection, _pathToConfigurationFile);
            SchedulerElement defaultScheduler = settings.Schedulers[settings.DefaultScheduler];

            TimeSpan timeOfDayValue = timeOfDay.Value.TimeOfDay;
            DateTime fromDateValue = fromDate.Checked ? fromDate.Value : DateTime.MinValue;
            DateTime toDateValue = toDate.Checked ? toDate.Value : DateTime.MinValue;

            List<DayOfWeek> selectedDays = new List<DayOfWeek>();
            foreach (DayOfWeek day in daysListBox.CheckedItems)
            {
                selectedDays.Add(day);
            }

            WeeklyScheduleElement schedule = new WeeklyScheduleElement("MyWeeklySchedule", fromDateValue, toDateValue, timeOfDayValue, selectedDays.ToArray());
            defaultScheduler.Add(schedule);

            string updatedConfigFile = Path.ChangeExtension(_pathToConfigurationFile, ".updated.config");
            SchedulerSettingsManager.SaveConfigurationSection(settings, updatedConfigFile);

            //Note: to write back to the original config file use:
            //SchedulerSettingsManager.SaveConfigurationSection(settings);

            MessageBox.Show("Configuration successfully saved to " + updatedConfigFile);
        }

    }
}
