using System;
using DemiCode.Logging;
using NUnit.Framework;

namespace DemiCode.Logging.Configuration.Test
{
    [TestFixture]
    public class LoggerElementTest
    {
        [Test]
        public void ConstructorTest()
        {
            new LoggerElement();
        }

        [Test]
        public void Constructor2Test()
        {
            var element = new LoggerElement(typeof(NilLoggerFactory));
            element.LoggerType.Should().Be(typeof (NilLoggerFactory));
        }

        [Test]
        public void GetLoggerTest()
        {
            var factory = new LoggerElement(typeof(NilLoggerFactory)).GetLoggerInstance();
            factory.Should().NotBeNull();
            factory.Should().BeOfType<NilLoggerFactory>();
        }

        [Test]
        public void GetLogger2Test()
        {
            Assert.Throws<InvalidOperationException>(() => new LoggerElement().GetLoggerInstance());
        }

        [Test]
        public void EqualsTest()
        {
            var left = new LoggerElement();
            var right = new LoggerElement();
            right.Should().Be(left);
        }

        [Test]
        public void Equals2Test()
        {
            var left = new LoggerElement(typeof(NilLoggerFactory));
            var right = new LoggerElement(typeof(NilLoggerFactory));
            right.Should().Be(left);
        }

        [Test]
        public void Equals3Test()
        {
            var left = new LoggerElement(typeof(NilLoggerFactory));
            var right = new LoggerElement(typeof(ConsoleLoggerFactory));
            right.Should().NotBe(left);
        }

    }
}
