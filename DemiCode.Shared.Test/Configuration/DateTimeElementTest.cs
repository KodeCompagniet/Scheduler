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
            Assert.AreEqual(now, element.Value);
        }

        [Test]
        public void Equals_SameParamsGivesTrue()
        {
            var now = DateTime.Now;

            var left = new DateTimeElement(now);
            var right = new DateTimeElement(now);

            Assert.AreEqual(left, right);
        }

        [Test]
        public void Equals_DifferentParamsGivesFalse()
        {
            var now = DateTime.Now;

            var left = new DateTimeElement(now);
            var right = new DateTimeElement(now.AddDays(1));

            Assert.AreNotEqual(left, right);
        }

        [Test]
        public void Equals_NoParamsGivesTrue()
        {
            var left = new DateTimeElement();
            var right = new DateTimeElement();

            Assert.AreEqual(left, right);
        }

        [Test]
        public void FormatTest()
        {
            var now = new DateTime(2008, 7, 6, 5, 4, 3, 2);
            var element = new DateTimeElement(now);
            var nowFormatted = element.ToString();
            Assert.AreEqual("2008-07-06 05:04:03", nowFormatted);
        }

        [Test]
        public void Equals_DifferentFormatGivesFalse()
        {
            var left = new DateTimeElement();
            var right = new FormatTestElement();
            Assert.AreNotEqual(left, right);
        }

        private class FormatTestElement : DateTimeElement
        {
            public FormatTestElement() : base("yy-MM-dd") {}
        }
    }
}
