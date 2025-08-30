using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Syndication;
using System.ServiceModel.Web;
using System.Text;

namespace DemiCode.Logging.Feeds
{
    /// <summary>
    /// Implement feed syndication for the status feed.
    /// </summary>
    public class StatusFeedService : IStatusFeed
    {
        public SyndicationFeedFormatter CreateFeed()
        {
            // Return ATOM or RSS based on query string
            // rss -> http://localhost:8731/DemiCode/StatusFeed/
            // atom -> http://localhost:8731/DemiCode/StatusFeed/?format=atom
            string query = WebOperationContext.Current.IncomingRequest.UriTemplateMatch.QueryParameters["format"];
            if (String.IsNullOrEmpty(query))
                query = "rss";

            return CreateFeedByFormat(query);
        }

        public SyndicationFeedFormatter CreateFeedByFormat(string format)
        {
            // Create a new Syndication Feed.
            SyndicationFeed feed = FeedLoader.LoadFeedFrom(".\\TestData", "StatusFeed*.xml");
            feed.Title = new TextSyndicationContent("Status");

            if (format == "atom")
            {
                return new Atom10FeedFormatter(feed);
            }

            return new Rss20FeedFormatter(feed);
        }
    }
}
