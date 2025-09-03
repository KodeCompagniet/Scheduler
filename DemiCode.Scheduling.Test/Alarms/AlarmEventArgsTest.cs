using NUnit.Framework;
using System;

namespace DemiCode.Scheduling.Alarms.Test
{
    /// <summary>
    ///This is a test class for DemiCode.Scheduling.AlarmEventArgs and is intended
    ///to contain all DemiCode.Scheduling.AlarmEventArgs Unit Tests
    ///</summary>
    [TestFixture]
    public class AlarmEventArgsTest
    {

        /// <summary>
        ///A test for AdditionalData
        ///</summary>
        [Test]
        public void AdditionalDataTest()
        {
            AlarmEventArgs target = new AlarmEventArgs(DateTime.Now, DateTime.Now, null);
            object val = "some additional data";
            target.AdditionalData = val;

            target.AdditionalData.Should().Be(val);
        }

        /// <summary>
        ///A test for the AlarmEventArgs copy constructor.
        ///</summary>
        [Test]
        public void CopyConstructorTest()
        {
            AlarmEventArgs e = new AlarmEventArgs(DateTime.Now, DateTime.Now.AddTicks(2), "additional data");
            AlarmEventArgs target = new AlarmEventArgs(e);

            target.SignalTime.Should().Be(e.SignalTime);
            target.ActualSignalTime.Should().Be(e.ActualSignalTime);
            target.AdditionalData.Should().Be(e.AdditionalData);
        }

        [Test]
        public void CopyConstructorWithNullArgumentTest()
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                new AlarmEventArgs(null);
            });
        }

        /// <summary>
        ///A test for AlarmEventArgs (DateTime, object)
        ///</summary>
        [Test]
        public void ConstructorTest()
        {
            DateTime signalTime = DateTime.Now;
            object additionalData = "additional data";

            AlarmEventArgs target = new AlarmEventArgs(signalTime, signalTime, additionalData);

            target.SignalTime.Should().Be(signalTime);
            target.AdditionalData.Should().Be(additionalData);
        }

        /// <summary>
        ///A test for SignalTime
        ///</summary>
        [Test]
        public void SignalTimeTest()
        {
            DateTime val = DateTime.Now.AddDays(1);
            AlarmEventArgs target = new AlarmEventArgs(val, val, "additional data");
            
            target.SignalTime.Should().Be(val);
        }
    }

}
