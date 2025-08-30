using System;
using System.Linq;
using NUnit.Framework;
using System.ServiceModel.Syndication;

namespace DemiCode.Logging.Feeds.Test
{
    [TestFixture]
    public class FeedLoaderTest
    {
        [Test]
        public void LoadTest()
        {
            SyndicationFeed feed = FeedLoader.LoadFeedFrom(".\\TestData", "StatusfeedLoadtest-*.xml");

            Assert.AreEqual(3, feed.Items.Count<SyndicationItem>());
            Assert.AreEqual(new DateTimeOffset(2008, 3, 31, 12, 0, 0, TimeSpan.Zero), feed.LastUpdatedTime);
        }

    }
}
