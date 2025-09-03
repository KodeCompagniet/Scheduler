using NUnit.Framework;
using System;
using System.Threading;
using DemiCode.Scheduling.Test.Mocks;
using System.Collections.ObjectModel;

namespace DemiCode.Scheduling.Test
{
    /// <summary>
    ///This is a test class for DemiCode.Scheduling.EventHelper and is intended
    ///to contain all DemiCode.Scheduling.EventHelper Unit Tests
    ///</summary>
    [TestFixture]
    public class EventHelperTest
    {

        /// <summary>
        ///A test for RaiseEvents&lt;&gt; (object, EventHandler&lt;TEventArgs&gt;, TEventArgs)
        ///</summary>
        [Test]
        public void RaiseEventsTest()
        {
            using (AutoResetEvent trigger = new AutoResetEvent(false))
            {
                object sender = this;
                EventArgs eventArgs = EventArgs.Empty;
                EventHandler<EventArgs> evh = delegate
                {
                    trigger.Set();
                };

                ReadOnlyCollection<object> failedEvents = EventHelper.RaiseEvents(sender, evh, eventArgs);

                bool result = trigger.WaitOne(500, true);

                result.Should().BeTrue();
                failedEvents.Should().NotBeNull();
                failedEvents.Count.Should().Be(0);
            }
        }

        /// <summary>
        ///A test for RaiseEvents&lt;&gt; (object, EventHandler&lt;TEventArgs&gt;, TEventArgs, bool)
        ///</summary>
        [Test]
        public void RaiseEventsSynchronizedTest()
        {
            using (AutoResetEvent trigger = new AutoResetEvent(false))
            {
                using (SynchronizedObject synchronizedObject = new SynchronizedObject())
                {
                    ReadOnlyCollection<object> failedEvents = synchronizedObject.RaiseEventSynchronized(trigger);

                    bool result = trigger.WaitOne(500, true);

                    result.Should().BeTrue();
                    failedEvents.Should().NotBeNull();
                    failedEvents.Count.Should().Be(0);
                }
            }
        }

        /// <summary>
        /// This test uses a failing event handler to test that we get correct results from <see cref="EventHelper.RaiseEvents{TEventArgs}(object,EventHandler{TEventArgs},TEventArgs)"/>.
        /// </summary>
        [Test]
        public void RaiseEventsFailingTest()
        {
            EventHandler<EventArgs> evh = delegate
            {
                throw new ApplicationException();
            };

            ReadOnlyCollection<object> failedEvents = EventHelper.RaiseEvents(this, evh, EventArgs.Empty);

            failedEvents.Should().NotBeNull();
            failedEvents.Count.Should().Be(1);
        }
    }


}
