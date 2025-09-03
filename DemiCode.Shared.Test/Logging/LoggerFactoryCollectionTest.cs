using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DemiCode.Logging.Test.Mocks;
using NUnit.Framework;

namespace DemiCode.Logging.Test
{
    [TestFixture]
    public class LoggerFactoryCollectionTest
    {

        /// <summary>
        ///A test for Log4netLogger (string)
        ///</summary>
        [Test]
        public void ConstructorTest()
        {
            LoggerFactoryCollection target = new LoggerFactoryCollection();
            target.Should().NotBeNull();
        }

        /// <summary>
        ///A test for Log4netLogger (Type)
        ///</summary>
        [Test]
        public void CreateDefaultLoggerTest()
        {
            LoggerFactoryCollection target = new LoggerFactoryCollection();
            ILoggerFactory loggerFactory = target.GetDefaultLoggerFactory();
            loggerFactory.Should().NotBeNull();
            loggerFactory.Should().BeOfType<NilLoggerFactory>();
        }

        [Test]
        public void AddTest()
        {
            ILoggerFactory factory = new LoggerFactoryMock();
            LoggerFactoryCollection target = new LoggerFactoryCollection();
            target.Add(factory);
        }

        [Test]
        public void AddNullTest()
        {
            LoggerFactoryCollection target = new LoggerFactoryCollection();
            Assert.Throws<ArgumentNullException>(() => target.Add(null));
        }

        [Test]
        public void IndexOfTest()
        {
            LoggerFactoryCollection collection = new LoggerFactoryCollection();
            collection.Add(new NilLoggerFactory());
            ((IList<ILoggerFactory>)collection).IndexOf(collection[0]).Should().Be(0);
        }

        [Test]
        public void InsertTest()
        {
            LoggerFactoryCollection collection = new LoggerFactoryCollection();
            collection.Add(new NilLoggerFactory());
            ((IList<ILoggerFactory>)collection).Insert(0, new ConsoleLoggerFactory());
            collection.Count.Should().Be(2);
            collection[0].Should().BeOfType<ConsoleLoggerFactory>();
        }

        [Test]
        public void ContainsTest()
        {
            LoggerFactoryCollection collection = new LoggerFactoryCollection();
            NilLoggerFactory factory = new NilLoggerFactory();
            collection.Add(factory);
            ((ICollection<ILoggerFactory>) collection).Contains(factory).Should().BeTrue();
            ((ICollection<ILoggerFactory>)collection).Contains(new NilLoggerFactory()).Should().BeFalse();
        }

        [Test]
        public void CopyToTest()
        {
            LoggerFactoryCollection collection = new LoggerFactoryCollection();
            NilLoggerFactory factory = new NilLoggerFactory();
            collection.Add(factory);

            ILoggerFactory[] array = new ILoggerFactory[collection.Count];
            ((ICollection<ILoggerFactory>) collection).CopyTo(array, 0);
        }

        [Test]
        public void IsReadOnlyTest()
        {
            LoggerFactoryCollection collection = new LoggerFactoryCollection();
            NilLoggerFactory factory = new NilLoggerFactory();
            collection.Add(factory);

            ((ICollection<ILoggerFactory>)collection).IsReadOnly.Should().BeFalse();

            ReadOnlyCollection<ILoggerFactory> roc = new ReadOnlyCollection<ILoggerFactory>(collection);
            ((ICollection<ILoggerFactory>)roc).IsReadOnly.Should().BeTrue();
        }

        [Test]
        public void GetEnumeratorTest()
        {
            LoggerFactoryCollection collection = new LoggerFactoryCollection();
            NilLoggerFactory factory = new NilLoggerFactory();
            collection.Add(factory);

            IEnumerator<ILoggerFactory> e = ((IEnumerable<ILoggerFactory>) collection).GetEnumerator();
            e.Should().NotBeNull();

            IEnumerator en = ((IEnumerable)collection).GetEnumerator();
            en.Should().NotBeNull();
        }

        [Test]
        public void ClearTest()
        {
            LoggerFactoryCollection collection = new LoggerFactoryCollection();
            NilLoggerFactory factory = new NilLoggerFactory();
            collection.Add(factory);

            ((ICollection<ILoggerFactory>)collection).Clear();

            collection.Count.Should().Be(0);
        }

        [Test]
        public void RemoveTest()
        {
            LoggerFactoryCollection collection = new LoggerFactoryCollection();
            NilLoggerFactory factory = new NilLoggerFactory();
            collection.Add(factory);

            ((ICollection<ILoggerFactory>) collection).Remove(factory).Should().BeTrue();

            collection.Count.Should().Be(0);
        }

        [Test]
        public void RemoveAtTest()
        {
            LoggerFactoryCollection collection = new LoggerFactoryCollection();
            NilLoggerFactory factory = new NilLoggerFactory();
            collection.Add(factory);

            ((IList<ILoggerFactory>)collection).RemoveAt(0);

            collection.Count.Should().Be(0);
        }

        [Test]
        public void ItemTest()
        {
            LoggerFactoryCollection collection = new LoggerFactoryCollection();
            NilLoggerFactory factory = new NilLoggerFactory();
            collection.Add(new NilLoggerFactory());
            collection[0] = factory;
            collection[0].Should().BeSameAs(factory);
        }
    }
}
