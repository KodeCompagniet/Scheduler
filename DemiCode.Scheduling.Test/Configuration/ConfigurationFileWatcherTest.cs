using System.Threading;
using DemiCode.Scheduling.Test;
using NUnit.Framework;

namespace DemiCode.Scheduling.Configuration.Test
{
    [TestFixture]
    public class ConfigurationFileWatcherTest
    {

        [Test]
        public void ConfigurationFileChangedEventTest()
        {

            // Watch configuration file for changes
            using (ConfigurationFileWatcher watcher = new ConfigurationFileWatcher())
            {
                using (AutoResetEvent trigger = new AutoResetEvent(false))
                {
                    watcher.ConfigurationFileChanged += ((sender, e) => trigger.Set());

                    Thread.Sleep(50);
                    FileHelper.TouchFile(watcher.ConfigFile.FullName);

                    Assert.IsTrue(trigger.WaitOne(2000, true));
                }
            }
        }

    }
}
