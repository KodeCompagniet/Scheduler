using NUnit.Framework;

namespace DemiCode.Scheduling.Test
{
    [TestFixture]
    public class DateTimeHelperTest
    {
        /// <summary>
        /// Confirm that GetFirstOfMonth returns the first of the next month.
        /// </summary>
        [TestCase(1, 1, 2, TestName = "From 1st Jan to 1st Feb")]
        [TestCase(1, 15, 2, TestName = "From 15th Jan to 1st Feb")]
        [TestCase(1, 31, 2, TestName = "From 31st Jan to 1st Feb")]
        [TestCase(2, 1, 3, TestName = "From 1st Feb to 1st Mar")]
        [TestCase(2, 15, 3, TestName = "From 15th Feb to 1st Mar")]
        [TestCase(2, 28, 3, TestName = "From 28th Feb to 1st Mar")]
        [TestCase(3, 1, 4, TestName = "From 1st Mar to 1st Apr")]
        [TestCase(3, 15, 4, TestName = "From 15th Mar to 1st Apr")]
        public void GetFirstOfMonth_FromAnyDay_YieldFirstOfNextMonth(int fromMonth, int fromDay, int expectedMonth)
        {
            var date = Dt.At(2007, fromMonth, fromDay);

            var newDate = date.GetFirstOfMonth();

            Assert.That(newDate, Is.EqualTo(Dt.At(2007, expectedMonth, 1)));
        }
    }
}