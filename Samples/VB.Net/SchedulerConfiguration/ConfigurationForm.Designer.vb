<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ConfigurationForm
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.scheduleTypeMonthly = New System.Windows.Forms.RadioButton
        Me.scheduleTypeWeekly = New System.Windows.Forms.RadioButton
        Me.scheduleTypeDaily = New System.Windows.Forms.RadioButton
        Me.groupBox1 = New System.Windows.Forms.GroupBox
        Me.daysListBox = New System.Windows.Forms.CheckedListBox
        Me.daysLabel = New System.Windows.Forms.Label
        Me.toDate = New System.Windows.Forms.DateTimePicker
        Me.fromDate = New System.Windows.Forms.DateTimePicker
        Me.toLabel = New System.Windows.Forms.Label
        Me.timeOfDay = New System.Windows.Forms.DateTimePicker
        Me.scheduleTypeOneShot = New System.Windows.Forms.RadioButton
        Me.fromLabel = New System.Windows.Forms.Label
        Me.saveButton = New System.Windows.Forms.Button
        Me.groupBox1.SuspendLayout()
        Me.SuspendLayout()
        '
        'scheduleTypeMonthly
        '
        Me.scheduleTypeMonthly.AutoSize = True
        Me.scheduleTypeMonthly.Enabled = False
        Me.scheduleTypeMonthly.Location = New System.Drawing.Point(6, 89)
        Me.scheduleTypeMonthly.Name = "scheduleTypeMonthly"
        Me.scheduleTypeMonthly.Size = New System.Drawing.Size(62, 17)
        Me.scheduleTypeMonthly.TabIndex = 3
        Me.scheduleTypeMonthly.Text = "Monthly"
        Me.scheduleTypeMonthly.UseVisualStyleBackColor = True
        '
        'scheduleTypeWeekly
        '
        Me.scheduleTypeWeekly.AutoSize = True
        Me.scheduleTypeWeekly.Checked = True
        Me.scheduleTypeWeekly.Location = New System.Drawing.Point(6, 66)
        Me.scheduleTypeWeekly.Name = "scheduleTypeWeekly"
        Me.scheduleTypeWeekly.Size = New System.Drawing.Size(61, 17)
        Me.scheduleTypeWeekly.TabIndex = 1
        Me.scheduleTypeWeekly.TabStop = True
        Me.scheduleTypeWeekly.Text = "Weekly"
        Me.scheduleTypeWeekly.UseVisualStyleBackColor = True
        '
        'scheduleTypeDaily
        '
        Me.scheduleTypeDaily.AutoSize = True
        Me.scheduleTypeDaily.Enabled = False
        Me.scheduleTypeDaily.Location = New System.Drawing.Point(6, 43)
        Me.scheduleTypeDaily.Name = "scheduleTypeDaily"
        Me.scheduleTypeDaily.Size = New System.Drawing.Size(48, 17)
        Me.scheduleTypeDaily.TabIndex = 2
        Me.scheduleTypeDaily.Text = "Daily"
        Me.scheduleTypeDaily.UseVisualStyleBackColor = True
        '
        'groupBox1
        '
        Me.groupBox1.Controls.Add(Me.scheduleTypeMonthly)
        Me.groupBox1.Controls.Add(Me.scheduleTypeDaily)
        Me.groupBox1.Controls.Add(Me.daysListBox)
        Me.groupBox1.Controls.Add(Me.daysLabel)
        Me.groupBox1.Controls.Add(Me.toDate)
        Me.groupBox1.Controls.Add(Me.scheduleTypeWeekly)
        Me.groupBox1.Controls.Add(Me.fromDate)
        Me.groupBox1.Controls.Add(Me.toLabel)
        Me.groupBox1.Controls.Add(Me.timeOfDay)
        Me.groupBox1.Controls.Add(Me.scheduleTypeOneShot)
        Me.groupBox1.Controls.Add(Me.fromLabel)
        Me.groupBox1.Location = New System.Drawing.Point(12, 12)
        Me.groupBox1.Name = "groupBox1"
        Me.groupBox1.Size = New System.Drawing.Size(417, 199)
        Me.groupBox1.TabIndex = 12
        Me.groupBox1.TabStop = False
        Me.groupBox1.Text = "Settings for 'MyWeeklySchedule'"
        '
        'daysListBox
        '
        Me.daysListBox.CheckOnClick = True
        Me.daysListBox.FormattingEnabled = True
        Me.daysListBox.Location = New System.Drawing.Point(192, 70)
        Me.daysListBox.Name = "daysListBox"
        Me.daysListBox.Size = New System.Drawing.Size(107, 109)
        Me.daysListBox.TabIndex = 8
        '
        'daysLabel
        '
        Me.daysLabel.AutoSize = True
        Me.daysLabel.Location = New System.Drawing.Point(132, 70)
        Me.daysLabel.Name = "daysLabel"
        Me.daysLabel.Size = New System.Drawing.Size(54, 13)
        Me.daysLabel.TabIndex = 7
        Me.daysLabel.Text = "Recur on:"
        '
        'toDate
        '
        Me.toDate.CustomFormat = "yyyy-MM-dd"
        Me.toDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom
        Me.toDate.Location = New System.Drawing.Point(170, 38)
        Me.toDate.Name = "toDate"
        Me.toDate.ShowCheckBox = True
        Me.toDate.Size = New System.Drawing.Size(107, 20)
        Me.toDate.TabIndex = 4
        '
        'fromDate
        '
        Me.fromDate.CustomFormat = "yyyy-MM-dd"
        Me.fromDate.Format = System.Windows.Forms.DateTimePickerFormat.Custom
        Me.fromDate.Location = New System.Drawing.Point(170, 12)
        Me.fromDate.Name = "fromDate"
        Me.fromDate.Size = New System.Drawing.Size(107, 20)
        Me.fromDate.TabIndex = 2
        '
        'toLabel
        '
        Me.toLabel.AutoSize = True
        Me.toLabel.Location = New System.Drawing.Point(132, 43)
        Me.toLabel.Name = "toLabel"
        Me.toLabel.Size = New System.Drawing.Size(32, 13)
        Me.toLabel.TabIndex = 3
        Me.toLabel.Text = "Stop:"
        '
        'timeOfDay
        '
        Me.timeOfDay.CustomFormat = "d.hh:mm:ss"
        Me.timeOfDay.Format = System.Windows.Forms.DateTimePickerFormat.Time
        Me.timeOfDay.Location = New System.Drawing.Point(283, 12)
        Me.timeOfDay.Name = "timeOfDay"
        Me.timeOfDay.Size = New System.Drawing.Size(107, 20)
        Me.timeOfDay.TabIndex = 6
        '
        'scheduleTypeOneShot
        '
        Me.scheduleTypeOneShot.AutoSize = True
        Me.scheduleTypeOneShot.Enabled = False
        Me.scheduleTypeOneShot.Location = New System.Drawing.Point(6, 20)
        Me.scheduleTypeOneShot.Name = "scheduleTypeOneShot"
        Me.scheduleTypeOneShot.Size = New System.Drawing.Size(67, 17)
        Me.scheduleTypeOneShot.TabIndex = 0
        Me.scheduleTypeOneShot.Text = "One time"
        Me.scheduleTypeOneShot.UseVisualStyleBackColor = True
        '
        'fromLabel
        '
        Me.fromLabel.AutoSize = True
        Me.fromLabel.Location = New System.Drawing.Point(132, 16)
        Me.fromLabel.Name = "fromLabel"
        Me.fromLabel.Size = New System.Drawing.Size(32, 13)
        Me.fromLabel.TabIndex = 1
        Me.fromLabel.Text = "Start:"
        '
        'saveButton
        '
        Me.saveButton.Location = New System.Drawing.Point(322, 217)
        Me.saveButton.Name = "saveButton"
        Me.saveButton.Size = New System.Drawing.Size(107, 23)
        Me.saveButton.TabIndex = 11
        Me.saveButton.Text = "Save"
        Me.saveButton.UseVisualStyleBackColor = True
        '
        'ConfigurationForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(445, 258)
        Me.Controls.Add(Me.groupBox1)
        Me.Controls.Add(Me.saveButton)
        Me.Name = "ConfigurationForm"
        Me.Text = "Scheduler Configuration"
        Me.groupBox1.ResumeLayout(False)
        Me.groupBox1.PerformLayout()
        Me.ResumeLayout(False)

    End Sub
    Private WithEvents scheduleTypeMonthly As System.Windows.Forms.RadioButton
    Private WithEvents scheduleTypeWeekly As System.Windows.Forms.RadioButton
    Private WithEvents scheduleTypeDaily As System.Windows.Forms.RadioButton
    Private WithEvents groupBox1 As System.Windows.Forms.GroupBox
    Private WithEvents daysListBox As System.Windows.Forms.CheckedListBox
    Private WithEvents daysLabel As System.Windows.Forms.Label
    Private WithEvents toDate As System.Windows.Forms.DateTimePicker
    Private WithEvents fromDate As System.Windows.Forms.DateTimePicker
    Private WithEvents toLabel As System.Windows.Forms.Label
    Private WithEvents timeOfDay As System.Windows.Forms.DateTimePicker
    Private WithEvents scheduleTypeOneShot As System.Windows.Forms.RadioButton
    Private WithEvents fromLabel As System.Windows.Forms.Label
    Private WithEvents saveButton As System.Windows.Forms.Button

End Class
