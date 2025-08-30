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

            Assert.AreEqual(val, target.AdditionalData, "DemiCode.Scheduling.AlarmEventArgs.AdditionalData was not set correctly.");
        }

        /// <summary>
        ///A test for the AlarmEventArgs copy constructor.
        ///</summary>
        [Test]
        public void CopyConstructorTest()
        {
            AlarmEventArgs e = new AlarmEventArgs(DateTime.Now, DateTime.Now.AddTicks(2), "additional data");
            AlarmEventArgs target = new AlarmEventArgs(e);

            Assert.AreEqual(e.SignalTime, target.SignalTime);
            Assert.AreEqual(e.ActualSignalTime, target.ActualSignalTime);
            Assert.AreEqual(e.AdditionalData, target.AdditionalData);
        }

        [Test]
        [ExpectedException(typeof (ArgumentNullException))]
        public void CopyConstructorWithNullArgumentTest()
        {
            new AlarmEventArgs(null);
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

            Assert.AreEqual(signalTime, target.SignalTime);
            Assert.AreEqual(additionalData, target.AdditionalData);
        }

        /// <summary>
        ///A test for SignalTime
        ///</summary>
        [Test]
        public void SignalTimeTest()
        {
            DateTime val = DateTime.Now.AddDays(1);
            AlarmEventArgs target = new AlarmEventArgs(val, val, "additional data");
            
            Assert.AreEqual(val, target.SignalTime, "DemiCode.Scheduling.AlarmEventArgs.SignalTime was not set correctly.");
        }
    }

}
