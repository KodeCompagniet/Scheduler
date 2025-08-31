using System;
using NUnit.Framework;

namespace DemiCode.Scheduling.Test
{
    [TestFixture]
    public class TimeHelperTest
    {
        [Test]
        public void GetTimeTest()
        {
            DateTime expetedNow = DateTime.Now.ToUniversalTime();
            DateTime now = TimeHelper.GetTime();

            Assert.AreEqual(DateTimeKind.Utc, now.Kind);

            Console.WriteLine("{0:yyyyMMdd hh:mm:ss:fffff}", now);
            Console.WriteLine("{0:yyyyMMdd hh:mm:ss:fffff}", now.ToLocalTime());

            Assert.AreEqual(expetedNow.Date, now.Date);
            Assert.AreEqual(expetedNow.TimeOfDay.Hours, now.TimeOfDay.Hours);
            Assert.AreEqual(expetedNow.TimeOfDay.Minutes, now.TimeOfDay.Minutes);
            Assert.AreEqual(expetedNow.TimeOfDay.Seconds, now.TimeOfDay.Seconds);
        }

        [Test]
        public void SetTimeTest()
        {
            DateTime originalNow = TimeHelper.GetTime().ToUniversalTime();
            DateTime modifiedNow = originalNow.AddHours(1);

            TimeHelper.SetTime(modifiedNow);

            try
            {
                DateTime currentNow = TimeHelper.GetTime();
                Assert.AreEqual(modifiedNow.Hour, currentNow.Hour);
            }
            finally
            {
                TimeHelper.SetTime(TimeHelper.GetTime().AddHours(-1));
            }
        }

        [Test]
        public void Change()
        {
            var originalDate = DateTime.Now;
            var newDate = new DateTime(2008, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            using (TimeHelper.Change(newDate))
            {
                var now = DateTime.Now;
                Assert.That(now.Year, Is.EqualTo(newDate.Year));
            }

            var restoredNow = DateTime.Now;
            Assert.That(restoredNow.Year, Is.EqualTo(originalDate.Year));
        }
    }
}