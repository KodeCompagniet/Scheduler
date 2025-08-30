using System;
using System.Configuration;
using System.Xml;
using NUnit.Framework;

namespace DemiCode.Configuration.Test
{
    [TestFixture]
    public class TimeSpanElementTest
    {
        [Test]
        public void Constructor1Test()
        {
            new TimeSpanElement();
        }

        [Test]
        public void Constructor2Test()
        {
            var value = TimeSpan.FromMinutes(42);
            var element = new TimeSpanElement(value);
            Assert.AreEqual(value, element.Value);
        }

        [Test]
        public void Equals_SameParamsGivesTrue()
        {
            var value = TimeSpan.FromMinutes(42);

            var left = new TimeSpanElement(value);
            var right = new TimeSpanElement(value);

            Assert.AreEqual(left, right);
        }

        [Test]
        public void Equals_DifferentParamsGivesFalse()
        {
            var left = new TimeSpanElement(TimeSpan.FromMinutes(42));
            var right = new TimeSpanElement(TimeSpan.FromMinutes(43));

            Assert.AreNotEqual(left, right);
        }

        [Test]
        public void Equals_NoParamsGivesTrue()
        {
            var left = new TimeSpanElement();
            var right = new TimeSpanElement();

            Assert.AreEqual(left, right);
        }
    }
}
