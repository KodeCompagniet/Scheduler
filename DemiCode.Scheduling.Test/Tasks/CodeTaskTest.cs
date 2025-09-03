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

            var task = new CodeTask(typeof(SimpleClass),  "SimpleMethod");
            task.Execute(loggerFactory);

            SimpleClass.SimpleMethodWasCalled.Should().BeTrue();

            task = new CodeTask(typeof(SimpleClass), "SimpleMethod", "value1", "value2");
            task.Execute(loggerFactory);

            SimpleClass.SimpleMethodWithParametersWasCalled.Should().BeTrue();

            task = new CodeTask(typeof(SimpleClass), "OtherMethodWithParameters", "value1", "value2");
            task.Execute(loggerFactory);

            SimpleClass.OtherMethodWithParametersWasCalled.Should().BeTrue();
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

            DisposableClass.DisposeWasCalled.Should().BeTrue();
        }

        [Test]
        public void NullParamTest()
        {
            try
            {
                new CodeTask(null, "SimpleMethod");
                Assert.Fail("Did not receive expected exception");
            }
            catch(ArgumentNullException) {}
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
            object[] parameters = new object[]{"value1", 2};

            Type[] types;
            object[] values;
            CodeTask.SplitParameterTypesAndValues(parameters, out types, out values);

            types.Length.Should().Be(2);
            values.Length.Should().Be(2);
            types[0].Should().Be(typeof(string));
            types[1].Should().Be(typeof(int));
            values[0].Should().Be("value1");
            values[1].Should().Be(2);

        }
    }


}

