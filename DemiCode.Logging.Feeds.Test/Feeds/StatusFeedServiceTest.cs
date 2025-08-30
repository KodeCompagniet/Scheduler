using NUnit.Framework;
using System.ServiceModel.Syndication;
using System.Xml;
using System.IO;

namespace DemiCode.Logging.Feeds.Test
{
    [TestFixture]
    public class StatusFeedServiceTest
    {
        [Test]
        public void LoadTest()
        {
            StatusFeedService sfs = new StatusFeedService();
            SyndicationFeedFormatter formatter = sfs.CreateFeedByFormat("rss");

            StringWriter sw = new StringWriter();
            using (XmlTextWriter xtw = new XmlTextWriter(sw))
            {
                formatter.WriteTo(xtw);
            }

            Assert.AreNotEqual("", sw.ToString());
        }

    }
}
