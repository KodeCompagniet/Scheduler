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
            element.Value.Should().Equal(days);
        }

        [Test]
        public void Equals_SameParamsGivesTrue()
        {
            var days = new[] { DayOfWeek.Friday, DayOfWeek.Wednesday };

            var left = new DaysElement(days);
            var right = new DaysElement(days);

            right.Should().Be(left);
        }

        [Test]
        public void Equals_DifferentParamsGivesFalse()
        {
            var days = new[] { DayOfWeek.Friday, DayOfWeek.Wednesday };
            var moreDays = new[] { DayOfWeek.Friday, DayOfWeek.Wednesday, DayOfWeek.Sunday};

            var left = new DaysElement(days);
            var right = new DaysElement(moreDays);

            right.Should().NotBe(left);
        }

        [Test]
        public void Equals_NoParamsGivesTrue()
        {
            var left = new DaysElement();
            var right = new DaysElement();

            right.Should().Be(left);
        }

    }
}
