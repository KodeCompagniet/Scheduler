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
            target.Should().NotBeNull();
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
            loggerFactory.Should().NotBeNull();
            loggerFactory.Should().BeOfType<NilLoggerFactory>();
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
            ((IList<ILoggerFactory>)collection).IndexOf(collection[0]).Should().Be(0);
        }

        [TestMethod]
        public void InsertTest()
        {
            LoggerFactoryCollection collection = new LoggerFactoryCollection();
            collection.Add(new NilLoggerFactory());
            ((IList<ILoggerFactory>)collection).Insert(0, new ConsoleLoggerFactory());
            collection.Count.Should().Be(2);
            collection[0].Should().BeOfType<ConsoleLoggerFactory>();
        }

        [TestMethod]
        public void ContainsTest()
        {
            LoggerFactoryCollection collection = new LoggerFactoryCollection();
            NilLoggerFactory factory = new NilLoggerFactory();
            collection.Add(factory);
            ((ICollection<ILoggerFactory>) collection).Contains(factory).Should().BeTrue();
            ((ICollection<ILoggerFactory>)collection).Contains(new NilLoggerFactory()).Should().BeFalse();
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

            ((ICollection<ILoggerFactory>)collection).IsReadOnly.Should().BeFalse();

            ReadOnlyCollection<ILoggerFactory> roc = new ReadOnlyCollection<ILoggerFactory>(collection);
            ((ICollection<ILoggerFactory>)roc).IsReadOnly.Should().BeTrue();
        }

        [TestMethod]
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

        [TestMethod]
        public void ClearTest()
        {
            LoggerFactoryCollection collection = new LoggerFactoryCollection();
            NilLoggerFactory factory = new NilLoggerFactory();
            collection.Add(factory);

            ((ICollection<ILoggerFactory>)collection).Clear();

            collection.Count.Should().Be(0);
        }

        [TestMethod]
        public void RemoveTest()
        {
            LoggerFactoryCollection collection = new LoggerFactoryCollection();
            NilLoggerFactory factory = new NilLoggerFactory();
            collection.Add(factory);

            ((ICollection<ILoggerFactory>) collection).Remove(factory).Should().BeTrue();

            collection.Count.Should().Be(0);
        }

        [TestMethod]
        public void RemoveAtTest()
        {
            LoggerFactoryCollection collection = new LoggerFactoryCollection();
            NilLoggerFactory factory = new NilLoggerFactory();
            collection.Add(factory);

            ((IList<ILoggerFactory>)collection).RemoveAt(0);

            collection.Count.Should().Be(0);
        }

        [TestMethod]
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
