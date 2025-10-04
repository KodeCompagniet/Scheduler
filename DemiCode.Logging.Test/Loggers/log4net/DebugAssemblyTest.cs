using System;
using System.Linq;
using System.Reflection;
using DemiCode.Logging;
using DemiCode.Logging.log4net;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DemiCode.Scheduling.Loggers.log4net.Test
{
    [TestClass]
    public class DebugAssemblyTest
    {
        [TestMethod]
        public void InvestigateAssemblyIssue()
        {
            // Create the Log4netLogger instance
            var logger = new Log4netLogger("test");

            // Get type information
            Type loggerType = logger.GetType();
            Console.WriteLine($"Logger Type: {loggerType.FullName}");
            Console.WriteLine($"Logger Assembly: {loggerType.Assembly.FullName}");
            Console.WriteLine($"Logger Assembly Location: {loggerType.Assembly.Location}");

            // Get all interfaces implemented by Log4netLogger
            Type[] interfaces = loggerType.GetInterfaces();
            Console.WriteLine($"\nInterfaces implemented by {loggerType.Name}:");
            foreach (var iface in interfaces)
            {
                Console.WriteLine($"  - {iface.FullName}");
                Console.WriteLine($"    Assembly: {iface.Assembly.FullName}");
                Console.WriteLine($"    Location: {iface.Assembly.Location}");
            }

            // Try to find ILogger interface in current context
            Type iLoggerType = typeof(ILogger);
            Console.WriteLine($"\nILogger from test context:");
            Console.WriteLine($"  FullName: {iLoggerType.FullName}");
            Console.WriteLine($"  Assembly: {iLoggerType.Assembly.FullName}");
            Console.WriteLine($"  Location: {iLoggerType.Assembly.Location}");

            // Check if the interfaces are the same
            var implementedILogger = interfaces.FirstOrDefault(i => i.Name == "ILogger");
            if (implementedILogger != null)
            {
                Console.WriteLine($"\nAre they the same type? {implementedILogger == iLoggerType}");
                Console.WriteLine($"Are they from same assembly? {implementedILogger.Assembly == iLoggerType.Assembly}");

                // Compare assembly identities
                Console.WriteLine($"\nImplemented ILogger Assembly Identity:");
                Console.WriteLine($"  {implementedILogger.Assembly.FullName}");
                Console.WriteLine($"\nExpected ILogger Assembly Identity:");
                Console.WriteLine($"  {iLoggerType.Assembly.FullName}");
            }

            // Try the cast
            try
            {
                ILogger castedLogger = (ILogger)logger;
                Console.WriteLine("\nCast succeeded!");
            }
            catch (InvalidCastException ex)
            {
                Console.WriteLine($"\nCast failed: {ex.Message}");
            }

            // Check all loaded assemblies
            Console.WriteLine("\nAll loaded assemblies containing 'Logging':");
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies().Where(a => a.FullName.Contains("Logging")))
            {
                Console.WriteLine($"  - {assembly.FullName}");
            }
        }
    }
}