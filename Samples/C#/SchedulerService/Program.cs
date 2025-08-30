using System.Collections.Generic;
using System.ServiceProcess;
using System.Text;
using DemiCode.Scheduling;
using System.Configuration;
using System.Threading;

namespace SampleService
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun 
                = new ServiceBase[] { new SampleService() };

            ServiceBase.Run(ServicesToRun);
        }
    }
}