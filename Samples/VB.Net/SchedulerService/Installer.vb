Imports System.ComponentModel
Imports System.Configuration.Install
Imports System.ServiceProcess

Public Class SampleServiceInstaller

    Private _serviceInstaller As ServiceInstaller
    Private _processInstaller As ServiceProcessInstaller

    Public Sub New()
        MyBase.New()

        'This call is required by the Component Designer.
        InitializeComponent()

        '  Instantiate installers for process and services.
        _processInstaller = New ServiceProcessInstaller
        _serviceInstaller = New ServiceInstaller

        ' The services run under the system account.
        _processInstaller.Account = ServiceAccount.LocalSystem
        _processInstaller.Username = Nothing
        _processInstaller.Password = Nothing

        ' Startup type
        _serviceInstaller.StartType = ServiceStartMode.Manual

        ' NB: ServiceName must equal those on ServiceBase derived classes.            
        _serviceInstaller.ServiceName = SampleService.SchedulerServiceName

        ' Dependencies
        _serviceInstaller.ServicesDependedOn = New String() {}


        ' Add installers to collection. Order is not important.
        Installers.AddRange(New System.Configuration.Install.Installer() {_processInstaller, _serviceInstaller})

    End Sub

End Class
