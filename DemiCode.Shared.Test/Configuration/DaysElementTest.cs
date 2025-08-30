using System;
using DemiCode.Configuration;
using NUnit.Framework;

namespace DemiCode.Configuration.Test
{
    [TestFixture]
    public class DaysElementTest
    {
        [Test]
        public void Constructor1Test()
        {
            new DaysElement();
        }

        [Test]
        public void Constructor2Test()
        {
            var days = new[] {DayOfWeek.Friday, DayOfWeek.Wednesday};
            var element = new DaysElement(days);
            Assert.AreEqual(days, element.Value);
        }

        [Test]
        public void Equals_SameParamsGivesTrue()
        {
            var days = new[] { DayOfWeek.Friday, DayOfWeek.Wednesday };

            var left = new DaysElement(days);
            var right = new DaysElement(days);

            Assert.AreEqual(left, right);
        }

        [Test]
        public void Equals_DifferentParamsGivesFalse()
        {
            var days = new[] { DayOfWeek.Friday, DayOfWeek.Wednesday };
            var moreDays = new[] { DayOfWeek.Friday, DayOfWeek.Wednesday, DayOfWeek.Sunday};

            var left = new DaysElement(days);
            var right = new DaysElement(moreDays);

            Assert.AreNotEqual(left, right);
        }

        [Test]
        public void Equals_NoParamsGivesTrue()
        {
            var left = new DaysElement();
            var right = new DaysElement();

            Assert.AreEqual(left, right);
        }

    }
}