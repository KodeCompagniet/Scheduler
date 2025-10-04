using System;
using System.Collections;

namespace DemiCode.Scheduling.Test.Mocks
{
    /// <summary>
    /// Class that enables testing of Factory.CreateInstance.
    /// </summary>
    internal class SimpleClass
    {
        [ThreadStatic] public static bool SimpleMethodWasCalled = false;
        [ThreadStatic] public static bool SimpleMethodWith1ParameterWasCalled = false;
        [ThreadStatic] public static bool SimpleMethodWithParametersWasCalled = false;
        [ThreadStatic] public static bool OtherMethodWithParametersWasCalled = false;
        [ThreadStatic] public static bool DisposeWasCalled = false;

        public SimpleClass()
        { }

        private string _constructorParameter1;
        [ThreadStatic] private string _constructorParameter1ThreadStatic;

        public string ConstructorParameter1
        {
            get { return _constructorParameter1; }
            set { _constructorParameter1 = value; }
        }

        public string ConstructorParameter1ThreadStatic
        {
            get { return _constructorParameter1ThreadStatic; }
            set { _constructorParameter1ThreadStatic = value; }
        }

        public SimpleClass(string param1)
        {
            _constructorParameter1 = param1;
            _constructorParameter1ThreadStatic = param1;
        }

        /// <summary>
        /// Bad constructor
        /// </summary>
        public SimpleClass(bool throwException)
        {
            if (throwException)
                throw new InvalidOperationException("Bad constructor");
        }

        public static void SimpleMethod()
        {
            SimpleMethodWasCalled = true;
        }

        public void SimpleMethod(string param1)
        {
            SimpleMethodWith1ParameterWasCalled = true;
        }

        public void SimpleMethod(string param1, string param2)
        {
            SimpleMethodWithParametersWasCalled = true;
        }

        public void OtherMethodWithParameters(string param1, string param2)
        {
            OtherMethodWithParametersWasCalled = true;
        }

        public static void ResetTrackerFields()
        {
            ExtendedMethodCalled = false;
            ExtendedMethodWithOneParamCalled = false;
        }

        public static bool ExtendedMethodCalled = false;
        public static bool ExtendedMethodWithOneParamCalled = false;

        public static void ExtendedMethod()
        {
            ExtendedMethodCalled = true;
        }

        public static void ExtendedMethod(string param1)
        {
            ExtendedMethodWithOneParamCalled = true;
        }
    }

    internal class DisposableClass : IDisposable
    {
        [ThreadStatic]
        public static bool DisposeWasCalled = false;

        public DisposableClass()
        { }

        public static void ResetTrackerFields()
        {
            DisposeWasCalled = false;
        }

        public void SimpleMethod() { }

        public void Dispose()
        {
            DisposeWasCalled = true;
            GC.SuppressFinalize(this);
        }

    }
}