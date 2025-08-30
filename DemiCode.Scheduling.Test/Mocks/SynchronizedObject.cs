using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Threading;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;

namespace DemiCode.Scheduling.Test.Mocks
{
    /// <summary>
    /// A simple synchronized object for doing sync event testing.
    /// The <seealso cref="ISynchronizeInvoke"/> implementation delegates its work to Juval Lowy's <see cref="Synchronizer"/> class.
    /// 
    /// 
    /// </summary>
    internal class SynchronizedObject : ISynchronizeInvoke, IDisposable
    {
        private bool _disposed = false;
        private Synchronizer _synchronizer;
        private AutoResetEvent _trigger = null;

        public SynchronizedObject()
        {
            _synchronizer = new Synchronizer();
            Event += new EventHandler<EventArgs>(SynchronizedObject_Event);
        }

        public event EventHandler<EventArgs> Event;

        /// <summary>
        /// This method uses <see cref="EventHelper.RaiseEvents"/> and its own event handler
        /// to simulate raising an event on a target object that requires synchronization.
        /// </summary>
        /// <param name="trigger">The <see cref="AutoResetEvent"/> trigger is used to signal successfull event execution.</param>
        /// <returns>The array of potetially failed event handlers returned from <see cref="EventHelper.RaiseEvents"/> </returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public ReadOnlyCollection<object> RaiseEventSynchronized(AutoResetEvent trigger)
        {
            _trigger = trigger;

            return EventHelper.RaiseEvents(this, Event, EventArgs.Empty, true, null);
        }

        private void SynchronizedObject_Event(object sender, EventArgs e)
        {
            if (_trigger != null)
                _trigger.Set();
        }

        #region ISynchronizeInvoke Members

        public IAsyncResult BeginInvoke(Delegate method, object[] args)
        {
            return _synchronizer.BeginInvoke(method, args);
        }

        public object EndInvoke(IAsyncResult result)
        {
            return _synchronizer.EndInvoke(result);
        }

        public object Invoke(Delegate method, object[] args)
        {
            return _synchronizer.Invoke(method, args);
        }

        /// <summary>
        /// Always returns true in order to force synchronization.
        /// </summary>
        public bool InvokeRequired
        {
            get { return true; }
        }

        #endregion

		#region Dispose and dtor stuff
		/// <summary>
		/// Destructor
		/// </summary>
        ~SynchronizedObject()
		{
			Dispose(false);
		}

		/// <summary>
		/// Disposes the AlarmTimer
		/// </summary>
		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		private void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				// If disposing equals true, dispose all managed 
				// and unmanaged resources.
				if (disposing)
				{
					// Dispose managed resources.
                    _synchronizer.Dispose();
                    _synchronizer = null;
				}

				// Release unmanaged resources. If disposing is false, 
				// only the following code is executed.
			}

			_disposed = true;
		}
		#endregion
    }
}
