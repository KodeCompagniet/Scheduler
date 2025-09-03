using System;
using NUnit.Framework;

namespace DemiCode.Configuration.Test
{
    [TestFixture]
    public class DateElementTest
    {
        [Test]
        public void Constructor1Test()
        {
            new DateElement();
        }

        [Test]
        public void Constructor2Test()
        {
            var now = DateTime.Now;
            var element = new DateElement(now);
            element.Value.Should().Be(now);
        }

        [Test]
        public void FormatTest()
        {
            var now = new DateTime(2008, 7, 6);
            var element = new DateElement(now);
            var nowFormatted = element.ToString();
            nowFormatted.Should().Be("2008-07-06");
        }
    }
}
