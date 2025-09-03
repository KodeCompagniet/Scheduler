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
            element.Value.Should().Be(value);
        }

        [Test]
        public void Equals_SameParamsGivesTrue()
        {
            var value = TimeSpan.FromMinutes(42);

            var left = new TimeSpanElement(value);
            var right = new TimeSpanElement(value);

            right.Should().Be(left);
        }

        [Test]
        public void Equals_DifferentParamsGivesFalse()
        {
            var left = new TimeSpanElement(TimeSpan.FromMinutes(42));
            var right = new TimeSpanElement(TimeSpan.FromMinutes(43));

            right.Should().NotBe(left);
        }

        [Test]
        public void Equals_NoParamsGivesTrue()
        {
            var left = new TimeSpanElement();
            var right = new TimeSpanElement();

            right.Should().Be(left);
        }
    }
}
