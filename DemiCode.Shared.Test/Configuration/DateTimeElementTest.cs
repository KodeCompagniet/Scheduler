using System;
using System.Configuration;
using System.Xml;
using System.Globalization;
using NUnit.Framework;

namespace DemiCode.Configuration.Test
{
    [TestFixture]
    public class DateTimeElementTest
    {
        [Test]
        public void Constructor1Test()
        {
            new DateTimeElement();
        }

        [Test]
        public void Constructor2Test()
        {
            var now = DateTime.Now;
            var element = new DateTimeElement(now);
            element.Value.Should().Be(now);
        }

        [Test]
        public void Equals_SameParamsGivesTrue()
        {
            var now = DateTime.Now;

            var left = new DateTimeElement(now);
            var right = new DateTimeElement(now);

            right.Should().Be(left);
        }

        [Test]
        public void Equals_DifferentParamsGivesFalse()
        {
            var now = DateTime.Now;

            var left = new DateTimeElement(now);
            var right = new DateTimeElement(now.AddDays(1));

            right.Should().NotBe(left);
        }

        [Test]
        public void Equals_NoParamsGivesTrue()
        {
            var left = new DateTimeElement();
            var right = new DateTimeElement();

            right.Should().Be(left);
        }

        [Test]
        public void FormatTest()
        {
            var now = new DateTime(2008, 7, 6, 5, 4, 3, 2);
            var element = new DateTimeElement(now);
            var nowFormatted = element.ToString();
            nowFormatted.Should().Be("2008-07-06 05:04:03");
        }

        [Test]
        public void Equals_DifferentFormatGivesFalse()
        {
            var left = new DateTimeElement();
            var right = new FormatTestElement();
            right.Should().NotBe(left);
        }

        private class FormatTestElement : DateTimeElement
        {
            public FormatTestElement() : base("yy-MM-dd") {}
        }
    }
}
