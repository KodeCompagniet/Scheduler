using DemiCode.Logging;
using DemiCode.Scheduling.Test.Mocks;
using NUnit.Framework;
using System;

namespace DemiCode.Scheduling.Tasks.Test
{
    /// <summary>
    ///This is a test class for DemiCode.Scheduling.Tasks.CodeTask and is intended
    ///to contain all DemiCode.Scheduling.Tasks.CodeTask Unit Tests
    ///</summary>
    [TestFixture]
    public class CodeTaskTest
    {

        /// <summary>
        ///A test for Execute (IDictionary)
        ///</summary>
        [Test]
        public void ExecuteTest()
        {
            ILoggerFactory loggerFactory = new LoggerFactoryMock();

            var task = new CodeTask(typeof(SimpleClass), "SimpleMethod");
            task.Execute(loggerFactory);

            Assert.IsTrue(SimpleClass.SimpleMethodWasCalled);

            task = new CodeTask(typeof(SimpleClass), "SimpleMethod", "value1", "value2");
            task.Execute(loggerFactory);

            Assert.IsTrue(SimpleClass.SimpleMethodWithParametersWasCalled);

            task = new CodeTask(typeof(SimpleClass), "OtherMethodWithParameters", "value1", "value2");
            task.Execute(loggerFactory);

            Assert.IsTrue(SimpleClass.OtherMethodWithParametersWasCalled);
        }

        /// <summary>
        ///Does CodeTask dispose instances?
        ///</summary>
        [Test]
        public void CodeTaskDisposesCodeAfterExecuteTest()
        {
            DisposableClass.ResetTrackerFields();

            ILoggerFactory loggerFactory = new LoggerFactoryMock();

            CodeTask task = new CodeTask(typeof(DisposableClass), "SimpleMethod");
            task.Execute(loggerFactory);

            Assert.IsTrue(DisposableClass.DisposeWasCalled);
        }

        [Test]
        public void NullParamTest()
        {
            try
            {
                new CodeTask(null, "SimpleMethod");
                Assert.Fail("Did not receive expected exception");
            }
            catch (ArgumentNullException) { }
            catch { Assert.Fail("Did not receive expected exception"); }

            try
            {
                new CodeTask(typeof(SimpleClass), null);
                Assert.Fail("Did not receive expected exception");
            }
            catch (ArgumentException) { }
            catch { Assert.Fail("Did not receive expected exception"); }

            var task = new CodeTask(typeof(SimpleClass), "SimpleMethod");
            try
            {
                task.Execute(null);
                Assert.Fail("Did not receive expected exception");
            }
            catch (ArgumentNullException) { }
            catch { Assert.Fail("Did not receive expected exception"); }
        }

        [Test]
        public void SplitParameterTest()
        {
            object[] parameters = new object[] { "value1", 2 };

            Type[] types;
            object[] values;
            CodeTask.SplitParameterTypesAndValues(parameters, out types, out values);

            Assert.AreEqual(2, types.Length);
            Assert.AreEqual(2, values.Length);
            Assert.AreEqual(typeof(string), types[0]);
            Assert.AreEqual(typeof(int), types[1]);
            Assert.AreEqual("value1", values[0]);
            Assert.AreEqual(2, values[1]);

        }
    }


}

