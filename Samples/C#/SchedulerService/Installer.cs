using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace SampleService
{
    [RunInstaller(true)]
    public partial class SampleServiceInstaller : Installer
    {
		private ServiceInstaller _serviceInstaller;
		private ServiceProcessInstaller _processInstaller;

        public SampleServiceInstaller()
		{
            InitializeComponent();

			// Instantiate installers for process and services.
			_processInstaller = new ServiceProcessInstaller();
			_serviceInstaller = new ServiceInstaller();

			// The services run under the system account.
			_processInstaller.Account = ServiceAccount.LocalSystem;
			_processInstaller.Username = null;
			_processInstaller.Password = null;

			//Startup type
			_serviceInstaller.StartType = ServiceStartMode.Manual;

			//NB: ServiceName must equal those on ServiceBase derived classes.            
			_serviceInstaller.ServiceName = SampleService.SchedulerServiceName;

			//Dependencies
			_serviceInstaller.ServicesDependedOn = new string[0];

			// Add installers to collection. Order is not important.
			Installers.AddRange(new System.Configuration.Install.Installer[] {
																				 _processInstaller,
																				 _serviceInstaller});
		}

    }
}