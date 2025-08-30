using System;
using System.IO;
using System.Globalization;
using log4net.Layout;
using log4net.Core;
using log4net.Util;
using log4net.Appender;
using System.Collections.Generic;
using System.ServiceModel.Syndication;
using System.Xml;
using System.Text;

namespace DemiCode.Logging.log4net
{
    /// <summary>
    /// Log a syndication feed item when a specific logging event occurs.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The number of logging events delivered in this e-mail depend on
    /// the value of <see cref="BufferingAppenderSkeleton.BufferSize"/> option. The
    /// <see cref="FeedAppender"/> keeps only the last
    /// <see cref="BufferingAppenderSkeleton.BufferSize"/> logging events in its 
    /// cyclic buffer. This keeps memory requirements at a reasonable level while 
    /// still delivering useful application context.
    /// </para>
    /// </remarks>
    public class FeedAppender : BufferingAppenderSkeleton
    {
        private const string _loggingNamespace = "http://demicode.com/logging";

        #region Public instance constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <remarks>
        /// <para>
        /// Default constructor
        /// </para>
        /// </remarks>
        public FeedAppender()
        {
        }

        #endregion // Public Instance Constructors

        #region Public instance properties

        public int MaxTitleLength { get; set; }

        #endregion // Public Instance Properties

        #region Override implementation of BufferingAppenderSkeleton

        /// <summary>
        /// Sends the contents of the cyclic buffer as an e-mail message.
        /// </summary>
        /// <param name="events">The logging events to send.</param>
        override protected void SendBuffer(LoggingEvent[] events)
        {
            // Note: this code already owns the monitor for this
            // appender. This frees us from needing to synchronize again.
            try
            {
                CreateFeed(events);
            }
            catch (Exception e)
            {
                ErrorHandler.Error("Error occurred writing events to feed.", e);
            }
        }

        #endregion // Override implementation of BufferingAppenderSkeleton

        #region Override implementation of AppenderSkeleton

        /// <summary>
        /// This appender does not use a <see cref="Layout"/>.
        /// </summary>
        /// <value><c>false</c></value>
        /// <remarks>
        /// <para>
        /// This appender does not use a <see cref="Layout"/>.
        /// </para>
        /// </remarks>
        override protected bool RequiresLayout
        {
            get { return false; }
        }

        #endregion // Override implementation of AppenderSkeleton

        #region Protected Methods

        private void CreateFeed(LoggingEvent[] events)
        {
            List<SyndicationItem> items = new List<SyndicationItem>(events.Length);

            for (int i = 0; i < events.Length; i++)
            {
                items.Add(CreateFeedItem(events[i]));
            }

            SyndicationFeed feed = new SyndicationFeed("Events", "", null, Guid.NewGuid().ToString(), DateTimeOffset.UtcNow, items);

            string filename = String.Format("Statusfeed-{0:yyyyMMdd-hhmmssff}.xml", DateTimeOffset.UtcNow);
            using (XmlTextWriter xtw = new XmlTextWriter(filename, Encoding.Unicode))
            {
                feed.SaveAsRss20(xtw);
            }
        }

        private SyndicationItem CreateFeedItem(LoggingEvent loggingEvent)
        {
            string eventText = RenderLoggingEvent(loggingEvent);
            string text = String.Format("{0}", loggingEvent.MessageObject);
            string title = String.Format("{0}: {1}", loggingEvent.Level, (text.Length > MaxTitleLength ? text.Substring(0, MaxTitleLength) + "..." : text));
            string id = loggingEvent.GetHashCode().ToString();

            SyndicationItem item = new SyndicationItem(title, eventText, null, id, loggingEvent.TimeStamp);

            if (loggingEvent.ExceptionObject != null)
            {
                item.ElementExtensions.Add(new SyndicationElementExtension("Exception", _loggingNamespace, loggingEvent.GetExceptionString()));
            }

            //item.AttributeExtensions.Add(_levelQualifiedName, loggingEvent.Level.ToString());
            item.Categories.Add(new SyndicationCategory(loggingEvent.Level.ToString(), _loggingNamespace, loggingEvent.Level.ToString()));

            return item;
        }

        #endregion

        #region Private Instance Fields

        private static XmlQualifiedName _levelQualifiedName = new XmlQualifiedName("level", _loggingNamespace);

        #endregion

    }
}

