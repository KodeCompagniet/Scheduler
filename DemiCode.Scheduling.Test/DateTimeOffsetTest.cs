using DemiCode.Scheduling.Time;
using NUnit.Framework;

namespace DemiCode.Scheduling.Test
{
    [TestFixture]
    public class DateTimeOffsetTest
    {
        [Test]
        public void ValidateAddingMonthFromJanuaryToFebruaryLeapYear()
        {
            var date = Dto.At(2008, 1, 31);

            var result = date.AddMonths(1);

            Assert.That(result, Is.EqualTo(Dto.At(2008, 2, 29)));
        }

        [Test]
        public void ValidateAddingMonthFromJanuaryToFebruaryNonLeapYear()
        {
            var date = Dto.At(2007, 1, 31);

            var result = date.AddMonths(1);

            Assert.That(result, Is.EqualTo(Dto.At(2007, 2, 28)));
        }

        [Test]
        public void ValidateAddingMonthFromFebruaryToMarch()
        {
            var date = Dto.At(2007, 2, 28);

            var result = date.AddMonths(1);

            Assert.That(result, Is.EqualTo(Dto.At(2007, 3, 28)));
        }
    }
}
