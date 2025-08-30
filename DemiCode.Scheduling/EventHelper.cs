using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections.ObjectModel;
using DemiCode.Logging;

namespace DemiCode.Scheduling
{
	/// <summary>
	/// Helper functions for code exposing events.
	/// </summary>
	/// <remarks>
	/// These helper functions will reise events, marshal them onto the right tread and
	/// remove "dead" eventhandlers.
	/// </remarks>
	internal static class EventHelper
	{
		/// <summary>
		/// This method will raise an event.
		/// </summary>
		/// <remarks>
		/// This method raises an event on the supplied event delegate.
		/// If a call on an event fails (throws an exception) the eventhandler is removed from the event delegate.
		/// When attemptSyncronized is set, eventshandlers on objects implementing the ISyncronizeInvoke will be
		/// marshalled onto the thread that object belongs to. (Typically used for WinForms applications.)
		/// </remarks>
		/// <typeparam name="TEventArgs">This can often be inferred from the eventArgs argument.</typeparam>
		/// <param name="sender">The object that this event is sent on behalf of.</param>
		/// <param name="evh">The event delegate.</param>
		/// <param name="eventArgs">The EventArgs that will be sent with the raised events.</param>
		/// <returns>A list of failed eventhandlers. These are also removed from the evh event delegate.</returns>
        public static ReadOnlyCollection<object> RaiseEvents<TEventArgs>(object sender, EventHandler<TEventArgs> evh, TEventArgs eventArgs)
			where TEventArgs : EventArgs
		{
            return RaiseEvents(sender, evh, eventArgs, true, null);
		}

        /// <summary>
        /// This method will raise an event.
        /// </summary>
        /// <remarks>
        /// This method raises an event on the supplied event delegate.
        /// If a call on an event fails (throws an exception) the eventhandler is removed from the event delegate.
        /// When attemptSyncronized is set, eventshandlers on objects implementing the ISyncronizeInvoke will be
        /// marshalled onto the thread that object belongs to. (Typically used for WinForms applications.)
        /// </remarks>
        /// <typeparam name="TEventArgs">This can often be inferred from the eventArgs argument.</typeparam>
        /// <param name="sender">The object that this event is sent on behalf of.</param>
        /// <param name="evh">The event delegate.</param>
        /// <param name="eventArgs">The EventArgs that will be sent with the raised events.</param>
        /// <param name="logger">Logger for potential problems with calling event handlers.</param>
        /// <returns>A list of failed eventhandlers. These are also removed from the evh event delegate.</returns>
        public static ReadOnlyCollection<object> RaiseEvents<TEventArgs>(object sender, EventHandler<TEventArgs> evh, TEventArgs eventArgs, ILogger logger)
            where TEventArgs : EventArgs
        {
            return RaiseEvents(sender, evh, eventArgs, true, logger);
        }

		/// <summary>
		/// This method will raise an event.
		/// </summary>
		/// <remarks>
		/// This method raises an event on the supplied event delegate.
		/// 
		/// When attemptSyncronized is set, eventshandlers on objects implementing the ISyncronizeInvoke will be
		/// marshalled onto the thread that object belongs to. (Typically used for WinForms applications.)
        /// 
        /// If a call to an eventhandler fails (throws an exception) the eventhandler is removed from the event delegate.
        /// If the <paramref name="logger"/> parameter is supplied a warning is logged with the corresponding exception.
        /// Execution is continued and subsequent eventhandlers are called. 
        /// Failing eventhandlers will be returned from this method for further inspection.
        /// 
        /// </remarks>
		/// <typeparam name="TEventArgs">This can often be inferred from the eventArgs argument.</typeparam>
		/// <param name="sender">The object that this event is sent on behalf of.</param>
		/// <param name="evh">The event delegate.</param>
		/// <param name="eventArgs">The EventArgs that will be sent with the raised events.</param>
		/// <param name="attemptSyncronized">Determines if the call to the delegate should be marshalled onto another thread.</param>
		/// <param name="logger">A logger instance to use in case of exceptions.</param>
		/// <returns>A list of failed eventhandlers. These are also removed from the evh event delegate.</returns>
        public static ReadOnlyCollection<object> RaiseEvents<TEventArgs>(object sender, EventHandler<TEventArgs> evh, TEventArgs eventArgs, bool attemptSyncronized, ILogger logger)
			where TEventArgs : EventArgs
		{
            //PLIL 12.12.2006: RaiseEvents participates in a potential stackoverflow bug.
            //                  I've not been able to reproduce it but the following code
            //                  could be used to halt execution _before_ a stackoverflow 
            //                  is reached, thus ease identification of the original caller.
            //try
            //{
            //    //StackTrace st = new StackTrace(true);
            //    //if (st.FrameCount > 100)
            //    //    Debugger.Break();
            //}
            //catch(Exception ex)
            //{
            //    ex = ex;
            //}

		    var failedTargets = new List<object>();

			if (evh != null)
			{
			    var delegates = evh.GetInvocationList();
                foreach (var del in delegates)
				{
					var eh = del as EventHandler<TEventArgs>;

					if (eh != null)
					{
						try
						{
							var target = del.Target as ISynchronizeInvoke;
							if ((attemptSyncronized) && (target != null) && (target.InvokeRequired))
							{
								//Fire event on targets main thread (Threadsafe version)
                                if (logger != null) logger.Debug("Invoking {0}", eh.Target);
                                
                                target.BeginInvoke(eh, new[] { sender, eventArgs });
                                
                                if (logger != null) logger.Debug("Handler {0} invoked", eh.Target);
							}
							else
							{
								//Fire event on this thread
                                if (logger != null) logger.Debug("Calling {0}", eh.Target);

								eh(sender, eventArgs);	

                                if (logger != null) logger.Debug("Handler {0} called", eh.Target);
							}
						}
						catch(Exception ex)
						{
                            // Remove the failing
							failedTargets.Add(eh.Target);
							evh -= eh;		
                            if (logger != null) logger.Warn("Event handler removed due to exception", ex);
						}
					}
				}
			}

            //if (failedTargets.Count > 0)
            //    throw new InvalidOperationException("Could not signal all event handlers");

            return new ReadOnlyCollection<object>(failedTargets);
		}
	}
}
