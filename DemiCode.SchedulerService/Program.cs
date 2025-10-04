using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using log4net;
using log4net.Config;

namespace DemiCode.SchedulerService
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Configure log4net
            var logRepository = LogManager.GetRepository(System.Reflection.Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, new FileInfo("log4net.config"));

            var builder = Host.CreateDefaultBuilder(args);

            // Determine if running as Windows Service or Console
            bool isService = !(Environment.UserInteractive && System.Diagnostics.Debugger.IsAttached);

            if (isService)
            {
                builder.UseWindowsService(options =>
                {
                    options.ServiceName = SchedulerHostedService.SchedulerServiceName;
                });
            }

            builder.ConfigureServices((hostContext, services) =>
            {
                services.AddHostedService<SchedulerHostedService>();
            })
            .ConfigureLogging((hostContext, logging) =>
            {
                logging.ClearProviders();
                // Add console logging for development
                if (!isService)
                {
                    logging.AddConsole();
                }
                // The service will use log4net for production logging
            })
            .ConfigureAppConfiguration((hostContext, config) =>
            {
                config.SetBasePath(Directory.GetCurrentDirectory());
                config.AddXmlFile("app.config", optional: false, reloadOnChange: true);
            });

            var host = builder.Build();

            try
            {
                await host.RunAsync();
            }
            catch (Exception ex)
            {
                var logger = host.Services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred while running the service");
                throw;
            }
        }
    }
}