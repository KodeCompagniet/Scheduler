using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel.Syndication;
using System.Xml;
using System.IO;

namespace DemiCode.Logging.Feeds
{
    internal class FeedLoader
    {
        /// <summary>
        /// Load feed items from all feed files in directory and return in one feed.
        /// Date on feed is taken from the newest feed.
        /// </summary>
        public static SyndicationFeed LoadFeedFrom(string directory, string filenamePattern)
        {
            string[] files = Directory.GetFiles(directory, filenamePattern);

            List<SyndicationItem> items = new List<SyndicationItem>(64);
            DateTimeOffset lastUpdated = DateTimeOffset.MinValue;
            foreach (var filename in files)
            {
                SyndicationFeed feed = LoadFeedFrom(filename);
                if (feed.LastUpdatedTime > lastUpdated)
                    lastUpdated = feed.LastUpdatedTime;
                items.AddRange(feed.Items);
            }

            return new SyndicationFeed("", "", null, "", lastUpdated, items);
        }

        private static SyndicationFeed LoadFeedFrom(string filename)
        {
            using (XmlTextReader xtr = new XmlTextReader(filename))
            {
                return SyndicationFeed.Load(xtr);
            }
        }

    }
}
