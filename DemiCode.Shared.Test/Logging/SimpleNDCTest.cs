using NUnit.Framework;

namespace DemiCode.Logging.Test
{
    [TestFixture]
    public class SimpleNDCTest
    {
        [Test]
        public void ConstructorTest()
        {
            using (new SimpleNDC("test"))
            {}
        }
    }


}
