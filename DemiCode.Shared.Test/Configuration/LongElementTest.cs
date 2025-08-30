using System;
using System.Configuration;
using System.Xml;
using NUnit.Framework;

namespace DemiCode.Configuration.Test
{
    [TestFixture]
    public class LongElementTest
    {
        [Test]
        public void Constructor1Test()
        {
            new LongElement();
        }

        [Test]
        public void Constructor2Test()
        {
            var value = 42;
            var element = new LongElement(value);
            Assert.AreEqual(value, element.Value);
        }

        [Test]
        public void Equals_SameParamsGivesTrue()
        {
            var value = 42;

            var left = new LongElement(value);
            var right = new LongElement(value);

            Assert.AreEqual(left, right);
        }

        [Test]
        public void Equals_DifferentParamsGivesFalse()
        {
            var left = new LongElement(42);
            var right = new LongElement(43);

            Assert.AreNotEqual(left, right);
        }

        [Test]
        public void Equals_NoParamsGivesTrue()
        {
            var left = new LongElement();
            var right = new LongElement();

            Assert.AreEqual(left, right);
        }

    }
}
