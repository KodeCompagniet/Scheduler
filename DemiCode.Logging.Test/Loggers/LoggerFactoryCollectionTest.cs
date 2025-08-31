using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DemiCode.Logging;  // Use ILoggerFactory from DemiCode.Shared
using DemiCode.Logging.Test.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DemiCode.Scheduling.Loggers.Test
{
    [TestClass()]
    public class LoggerFactoryCollectionTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }
        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for Log4netLogger (string)
        ///</summary>
        [DeploymentItem("DemiCode.Scheduling.Loggers.log4net.dll")]
        [TestMethod()]
        public void ConstructorTest()
        {
            LoggerFactoryCollection target = new LoggerFactoryCollection();
            Assert.IsNotNull(target);
        }

        /// <summary>
        ///A test for Log4netLogger (Type)
        ///</summary>
        [DeploymentItem("DemiCode.Scheduling.Loggers.log4net.dll")]
        [TestMethod()]
        public void CreateDefaultLoggerTest()
        {
            LoggerFactoryCollection target = new LoggerFactoryCollection();
            ILoggerFactory loggerFactory = target.GetDefaultLoggerFactory();
            Assert.IsNotNull(loggerFactory);
            Assert.IsInstanceOfType(loggerFactory, typeof(NilLoggerFactory));
        }

        [TestMethod]
        public void AddTest()
        {
            ILoggerFactory factory = (ILoggerFactory)new LoggerFactoryMock();
            LoggerFactoryCollection target = new LoggerFactoryCollection();
            target.Add(factory);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void AddNullTest()
        {
            LoggerFactoryCollection target = new LoggerFactoryCollection();
            target.Add((ILoggerFactory)null);
        }

        [TestMethod]
        public void IndexOfTest()
        {
            LoggerFactoryCollection collection = new LoggerFactoryCollection();
            collection.Add(new NilLoggerFactory());
            Assert.AreEqual(0, ((IList<ILoggerFactory>)collection).IndexOf(collection[0]));
        }

        [TestMethod]
        public void InsertTest()
        {
            LoggerFactoryCollection collection = new LoggerFactoryCollection();
            collection.Add(new NilLoggerFactory());
            ((IList<ILoggerFactory>)collection).Insert(0, new ConsoleLoggerFactory());
            Assert.AreEqual(2, collection.Count);
            Assert.IsInstanceOfType(collection[0], typeof(ConsoleLoggerFactory));
        }

        [TestMethod]
        public void ContainsTest()
        {
            LoggerFactoryCollection collection = new LoggerFactoryCollection();
            NilLoggerFactory factory = new NilLoggerFactory();
            collection.Add(factory);
            Assert.IsTrue(((ICollection<ILoggerFactory>) collection).Contains(factory));
            Assert.IsFalse(((ICollection<ILoggerFactory>)collection).Contains(new NilLoggerFactory()));
        }

        [TestMethod]
        public void CopyToTest()
        {
            LoggerFactoryCollection collection = new LoggerFactoryCollection();
            NilLoggerFactory factory = new NilLoggerFactory();
            collection.Add(factory);

            ILoggerFactory[] array = new ILoggerFactory[collection.Count];
            ((ICollection<ILoggerFactory>) collection).CopyTo(array, 0);
        }

        [TestMethod]
        public void IsReadOnlyTest()
        {
            LoggerFactoryCollection collection = new LoggerFactoryCollection();
            NilLoggerFactory factory = new NilLoggerFactory();
            collection.Add(factory);

            Assert.IsFalse(((ICollection<ILoggerFactory>)collection).IsReadOnly);

            ReadOnlyCollection<ILoggerFactory> roc = new ReadOnlyCollection<ILoggerFactory>(collection);
            Assert.IsTrue(((ICollection<ILoggerFactory>)roc).IsReadOnly);
        }

        [TestMethod]
        public void GetEnumeratorTest()
        {
            LoggerFactoryCollection collection = new LoggerFactoryCollection();
            NilLoggerFactory factory = new NilLoggerFactory();
            collection.Add(factory);

            IEnumerator<ILoggerFactory> e = ((IEnumerable<ILoggerFactory>) collection).GetEnumerator();
            Assert.IsNotNull(e);

            IEnumerator en = ((IEnumerable)collection).GetEnumerator();
            Assert.IsNotNull(en);
        }

        [TestMethod]
        public void ClearTest()
        {
            LoggerFactoryCollection collection = new LoggerFactoryCollection();
            NilLoggerFactory factory = new NilLoggerFactory();
            collection.Add(factory);

            ((ICollection<ILoggerFactory>)collection).Clear();

            Assert.AreEqual(0, collection.Count);
        }

        [TestMethod]
        public void RemoveTest()
        {
            LoggerFactoryCollection collection = new LoggerFactoryCollection();
            NilLoggerFactory factory = new NilLoggerFactory();
            collection.Add(factory);

            Assert.IsTrue(((ICollection<ILoggerFactory>) collection).Remove(factory));

            Assert.AreEqual(0, collection.Count);
        }

        [TestMethod]
        public void RemoveAtTest()
        {
            LoggerFactoryCollection collection = new LoggerFactoryCollection();
            NilLoggerFactory factory = new NilLoggerFactory();
            collection.Add(factory);

            ((IList<ILoggerFactory>)collection).RemoveAt(0);

            Assert.AreEqual(0, collection.Count);
        }

        [TestMethod]
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
