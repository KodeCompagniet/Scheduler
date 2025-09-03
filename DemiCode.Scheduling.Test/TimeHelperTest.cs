using System;
using System.Runtime.InteropServices;
using NUnit.Framework;

namespace DemiCode.Scheduling.Test
{
    [TestFixture]
    public class TimeHelperTest
    {
        [Test]
        public void GetTimeTest()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                Assert.Ignore("System time manipulation is only supported on Windows");

            DateTime expetedNow = DateTime.Now.ToUniversalTime();
            DateTime now = TimeHelper.GetTime();

            now.Kind.Should().Be(DateTimeKind.Utc);

            Console.WriteLine("{0:yyyyMMdd hh:mm:ss:fffff}", now);
            Console.WriteLine("{0:yyyyMMdd hh:mm:ss:fffff}", now.ToLocalTime());

            now.Date.Should().Be(expetedNow.Date);
            now.TimeOfDay.Hours.Should().Be(expetedNow.TimeOfDay.Hours);
            now.TimeOfDay.Minutes.Should().Be(expetedNow.TimeOfDay.Minutes);
            now.TimeOfDay.Seconds.Should().Be(expetedNow.TimeOfDay.Seconds);
        }

        [Test]
        public void SetTimeTest()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                Assert.Ignore("System time manipulation is only supported on Windows");

            DateTime originalNow = TimeHelper.GetTime().ToUniversalTime();
            DateTime modifiedNow = originalNow.AddHours(1);

            TimeHelper.SetTime(modifiedNow);

            try
            {
                DateTime currentNow = TimeHelper.GetTime();
                currentNow.Hour.Should().Be(modifiedNow.Hour);
            }
            finally
            {
                TimeHelper.SetTime(TimeHelper.GetTime().AddHours(-1));
            }
        }

        [Test]
        public void Change()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                Assert.Ignore("System time manipulation is only supported on Windows");

            var originalDate = DateTime.Now;
            var newDate = new DateTime(2008, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            using (TimeHelper.Change(newDate))
            {
                var now = DateTime.Now;
                now.Year.Should().Be(newDate.Year);
            }

            var restoredNow = DateTime.Now;
            restoredNow.Year.Should().Be(originalDate.Year);
        }
    }
}
