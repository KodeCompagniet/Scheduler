namespace SchedulerConfiguration
{
    partial class ConfigurationForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.saveButton = new System.Windows.Forms.Button();
            this.daysListBox = new System.Windows.Forms.CheckedListBox();
            this.daysLabel = new System.Windows.Forms.Label();
            this.fromLabel = new System.Windows.Forms.Label();
            this.toLabel = new System.Windows.Forms.Label();
            this.timeOfDay = new System.Windows.Forms.DateTimePicker();
            this.fromDate = new System.Windows.Forms.DateTimePicker();
            this.toDate = new System.Windows.Forms.DateTimePicker();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.scheduleTypeOneShot = new System.Windows.Forms.RadioButton();
            this.scheduleTypeWeekly = new System.Windows.Forms.RadioButton();
            this.scheduleTypeDaily = new System.Windows.Forms.RadioButton();
            this.scheduleTypeMonthly = new System.Windows.Forms.RadioButton();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(322, 217);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(107, 23);
            this.saveButton.TabIndex = 9;
            this.saveButton.Text = "Save";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // daysListBox
            // 
            this.daysListBox.CheckOnClick = true;
            this.daysListBox.FormattingEnabled = true;
            this.daysListBox.Location = new System.Drawing.Point(192, 70);
            this.daysListBox.Name = "daysListBox";
            this.daysListBox.Size = new System.Drawing.Size(107, 109);
            this.daysListBox.TabIndex = 8;
            // 
            // daysLabel
            // 
            this.daysLabel.AutoSize = true;
            this.daysLabel.Location = new System.Drawing.Point(132, 70);
            this.daysLabel.Name = "daysLabel";
            this.daysLabel.Size = new System.Drawing.Size(54, 13);
            this.daysLabel.TabIndex = 7;
            this.daysLabel.Text = "Recur on:";
            // 
            // fromLabel
            // 
            this.fromLabel.AutoSize = true;
            this.fromLabel.Location = new System.Drawing.Point(132, 16);
            this.fromLabel.Name = "fromLabel";
            this.fromLabel.Size = new System.Drawing.Size(32, 13);
            this.fromLabel.TabIndex = 1;
            this.fromLabel.Text = "Start:";
            // 
            // toLabel
            // 
            this.toLabel.AutoSize = true;
            this.toLabel.Location = new System.Drawing.Point(132, 43);
            this.toLabel.Name = "toLabel";
            this.toLabel.Size = new System.Drawing.Size(32, 13);
            this.toLabel.TabIndex = 3;
            this.toLabel.Text = "Stop:";
            // 
            // timeOfDay
            // 
            this.timeOfDay.CustomFormat = "d.hh:mm:ss";
            this.timeOfDay.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.timeOfDay.Location = new System.Drawing.Point(283, 12);
            this.timeOfDay.Name = "timeOfDay";
            this.timeOfDay.Size = new System.Drawing.Size(107, 20);
            this.timeOfDay.TabIndex = 6;
            // 
            // fromDate
            // 
            this.fromDate.CustomFormat = "yyyy-MM-dd";
            this.fromDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.fromDate.Location = new System.Drawing.Point(170, 12);
            this.fromDate.Name = "fromDate";
            this.fromDate.Size = new System.Drawing.Size(107, 20);
            this.fromDate.TabIndex = 2;
            // 
            // toDate
            // 
            this.toDate.CustomFormat = "yyyy-MM-dd";
            this.toDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.toDate.Location = new System.Drawing.Point(170, 38);
            this.toDate.Name = "toDate";
            this.toDate.ShowCheckBox = true;
            this.toDate.Size = new System.Drawing.Size(107, 20);
            this.toDate.TabIndex = 4;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.scheduleTypeMonthly);
            this.groupBox1.Controls.Add(this.scheduleTypeDaily);
            this.groupBox1.Controls.Add(this.daysListBox);
            this.groupBox1.Controls.Add(this.daysLabel);
            this.groupBox1.Controls.Add(this.toDate);
            this.groupBox1.Controls.Add(this.scheduleTypeWeekly);
            this.groupBox1.Controls.Add(this.fromDate);
            this.groupBox1.Controls.Add(this.toLabel);
            this.groupBox1.Controls.Add(this.timeOfDay);
            this.groupBox1.Controls.Add(this.scheduleTypeOneShot);
            this.groupBox1.Controls.Add(this.fromLabel);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(417, 199);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Settings for \'MyWeeklySchedule\'";
            // 
            // scheduleTypeOneShot
            // 
            this.scheduleTypeOneShot.AutoSize = true;
            this.scheduleTypeOneShot.Enabled = false;
            this.scheduleTypeOneShot.Location = new System.Drawing.Point(6, 20);
            this.scheduleTypeOneShot.Name = "scheduleTypeOneShot";
            this.scheduleTypeOneShot.Size = new System.Drawing.Size(67, 17);
            this.scheduleTypeOneShot.TabIndex = 0;
            this.scheduleTypeOneShot.Text = "One time";
            this.scheduleTypeOneShot.UseVisualStyleBackColor = true;
            // 
            // scheduleTypeWeekly
            // 
            this.scheduleTypeWeekly.AutoSize = true;
            this.scheduleTypeWeekly.Checked = true;
            this.scheduleTypeWeekly.Location = new System.Drawing.Point(6, 66);
            this.scheduleTypeWeekly.Name = "scheduleTypeWeekly";
            this.scheduleTypeWeekly.Size = new System.Drawing.Size(61, 17);
            this.scheduleTypeWeekly.TabIndex = 1;
            this.scheduleTypeWeekly.TabStop = true;
            this.scheduleTypeWeekly.Text = "Weekly";
            this.scheduleTypeWeekly.UseVisualStyleBackColor = true;
            // 
            // scheduleTypeDaily
            // 
            this.scheduleTypeDaily.AutoSize = true;
            this.scheduleTypeDaily.Enabled = false;
            this.scheduleTypeDaily.Location = new System.Drawing.Point(6, 43);
            this.scheduleTypeDaily.Name = "scheduleTypeDaily";
            this.scheduleTypeDaily.Size = new System.Drawing.Size(48, 17);
            this.scheduleTypeDaily.TabIndex = 2;
            this.scheduleTypeDaily.Text = "Daily";
            this.scheduleTypeDaily.UseVisualStyleBackColor = true;
            // 
            // scheduleTypeMonthly
            // 
            this.scheduleTypeMonthly.AutoSize = true;
            this.scheduleTypeMonthly.Enabled = false;
            this.scheduleTypeMonthly.Location = new System.Drawing.Point(6, 89);
            this.scheduleTypeMonthly.Name = "scheduleTypeMonthly";
            this.scheduleTypeMonthly.Size = new System.Drawing.Size(62, 17);
            this.scheduleTypeMonthly.TabIndex = 3;
            this.scheduleTypeMonthly.Text = "Monthly";
            this.scheduleTypeMonthly.UseVisualStyleBackColor = true;
            // 
            // ConfigurationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(445, 249);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.saveButton);
            this.Name = "ConfigurationForm";
            this.Text = "Scheduler Configuration";
            this.Load += new System.EventHandler(this.ConfigurationForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.CheckedListBox daysListBox;
        private System.Windows.Forms.Label daysLabel;
        private System.Windows.Forms.Label fromLabel;
        private System.Windows.Forms.Label toLabel;
        private System.Windows.Forms.DateTimePicker timeOfDay;
        private System.Windows.Forms.DateTimePicker fromDate;
        private System.Windows.Forms.DateTimePicker toDate;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton scheduleTypeMonthly;
        private System.Windows.Forms.RadioButton scheduleTypeDaily;
        private System.Windows.Forms.RadioButton scheduleTypeWeekly;
        private System.Windows.Forms.RadioButton scheduleTypeOneShot;
    }
}

