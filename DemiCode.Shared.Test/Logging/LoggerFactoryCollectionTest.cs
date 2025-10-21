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
            Assert.IsNotNull(target);
        }

        /// <summary>
        ///A test for Log4netLogger (Type)
        ///</summary>
        [Test]
        public void CreateDefaultLoggerTest()
        {
            LoggerFactoryCollection target = new LoggerFactoryCollection();
            ILoggerFactory loggerFactory = target.GetDefaultLoggerFactory();
            Assert.IsNotNull(loggerFactory);
            Assert.IsInstanceOf<NilLoggerFactory>(loggerFactory);
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
            Assert.AreEqual(0, ((IList<ILoggerFactory>)collection).IndexOf(collection[0]));
        }

        [Test]
        public void InsertTest()
        {
            LoggerFactoryCollection collection = new LoggerFactoryCollection();
            collection.Add(new NilLoggerFactory());
            ((IList<ILoggerFactory>)collection).Insert(0, new ConsoleLoggerFactory());
            Assert.AreEqual(2, collection.Count);
            Assert.IsInstanceOf<ConsoleLoggerFactory>(collection[0]);
        }

        [Test]
        public void ContainsTest()
        {
            LoggerFactoryCollection collection = new LoggerFactoryCollection();
            NilLoggerFactory factory = new NilLoggerFactory();
            collection.Add(factory);
            Assert.IsTrue(((ICollection<ILoggerFactory>)collection).Contains(factory));
            Assert.IsFalse(((ICollection<ILoggerFactory>)collection).Contains(new NilLoggerFactory()));
        }

        [Test]
        public void CopyToTest()
        {
            LoggerFactoryCollection collection = new LoggerFactoryCollection();
            NilLoggerFactory factory = new NilLoggerFactory();
            collection.Add(factory);

            ILoggerFactory[] array = new ILoggerFactory[collection.Count];
            ((ICollection<ILoggerFactory>)collection).CopyTo(array, 0);
        }

        [Test]
        public void IsReadOnlyTest()
        {
            LoggerFactoryCollection collection = new LoggerFactoryCollection();
            NilLoggerFactory factory = new NilLoggerFactory();
            collection.Add(factory);

            Assert.IsFalse(((ICollection<ILoggerFactory>)collection).IsReadOnly);

            ReadOnlyCollection<ILoggerFactory> roc = new ReadOnlyCollection<ILoggerFactory>(collection);
            Assert.IsTrue(((ICollection<ILoggerFactory>)roc).IsReadOnly);
        }

        [Test]
        public void GetEnumeratorTest()
        {
            LoggerFactoryCollection collection = new LoggerFactoryCollection();
            NilLoggerFactory factory = new NilLoggerFactory();
            collection.Add(factory);

            IEnumerator<ILoggerFactory> e = ((IEnumerable<ILoggerFactory>)collection).GetEnumerator();
            Assert.IsNotNull(e);

            IEnumerator en = ((IEnumerable)collection).GetEnumerator();
            Assert.IsNotNull(en);
        }

        [Test]
        public void ClearTest()
        {
            LoggerFactoryCollection collection = new LoggerFactoryCollection();
            NilLoggerFactory factory = new NilLoggerFactory();
            collection.Add(factory);

            ((ICollection<ILoggerFactory>)collection).Clear();

            Assert.AreEqual(0, collection.Count);
        }

        [Test]
        public void RemoveTest()
        {
            LoggerFactoryCollection collection = new LoggerFactoryCollection();
            NilLoggerFactory factory = new NilLoggerFactory();
            collection.Add(factory);

            Assert.IsTrue(((ICollection<ILoggerFactory>)collection).Remove(factory));

            Assert.AreEqual(0, collection.Count);
        }

        [Test]
        public void RemoveAtTest()
        {
            LoggerFactoryCollection collection = new LoggerFactoryCollection();
            NilLoggerFactory factory = new NilLoggerFactory();
            collection.Add(factory);

            ((IList<ILoggerFactory>)collection).RemoveAt(0);

            Assert.AreEqual(0, collection.Count);
        }

        [Test]
        public void ItemTest()
        {
            LoggerFactoryCollection collection = new LoggerFactoryCollection();
            NilLoggerFactory factory = new NilLoggerFactory();
            collection.Add(new NilLoggerFactory());
            collection[0] = factory;
            Assert.AreSame(factory, collection[0]);
        }
    }
}
