using System;
using System.Collections.Generic;
using System.Reflection;
using DemiCode.Logging;
using DemiCode.Shared.Factories;

namespace DemiCode.Scheduling.Tasks
{
    /// <summary>
    /// CodeTask executes a method in a .NET class.
    /// </summary>
    public class CodeTask : ITask
    {
        ///<summary>
        /// The type of the class containing the code to execute.
        ///</summary>
        public Type CodeClass { get; private set; }

        /// <summary>
        /// The name of the method to execute on <see cref="CodeClass"/>
        /// </summary>
        public string Method { get; private set; }

        /// <summary>
        /// A list of parameters to pass to <see cref="Method"/>.
        /// </summary>
        public object[] Parameters { get; private set; }

        private readonly Type[] parameterTypes;
        private readonly object[] parameterValues;
        private readonly MethodInfo methodInstance;

        ///<summary>
        /// Create a <see cref="CodeTask"/> instance.
        ///</summary>
        /// <param name="codeClass">The type of the class containing the code to execute. If <paramref name="method"/> is a non-static method,  this class must support a parameterless constructor.</param>
        /// <param name="method">The name of the method to execute on <see cref="CodeClass"/>. The method can either static or non-static.</param>
        /// <param name="parameters">A list of parameters to pass to <see cref="Method"/>.</param>
        public CodeTask(Type codeClass, string method, params object[] parameters)
        {
            if (codeClass == null)
                throw new ArgumentNullException("codeClass");
            if (String.IsNullOrEmpty(method))
                throw new ArgumentException("Method name must be a valid static or non-static method", "method");
            if (parameters == null)
                throw new ArgumentNullException("parameters");

            SplitParameterTypesAndValues(parameters, out parameterTypes, out parameterValues);

            methodInstance = codeClass.GetMethod(method, parameterTypes);
            if (methodInstance == null)
                throw new ArgumentException("Specified method does not exist on the class", "method");

            if (!methodInstance.IsStatic && (codeClass.GetConstructor(new Type[0]) == null))
                throw new ArgumentException("Specified class does not have a parameterless constructor", "codeClass");

            CodeClass = codeClass;
            Method = method;
            Parameters = parameters;
        }

        /// <summary>
        /// Execute the task with the given parameters.
        /// </summary>
        /// <param name="loggerFactory">A logger factory which the task can use to create loggers for logging information, warnings and errors</param>
        public void Execute(ILoggerFactory loggerFactory)
        {
            if (loggerFactory == null)
                throw new ArgumentNullException("loggerFactory");

            var logger = loggerFactory.CreateLogger(typeof(CodeTask));

            var codeInstance = Factory.CreateInstance<object>(CodeClass);
            var disposableCode = codeInstance as IDisposable;

            try
            {
                logger.Debug("Invoking '{0}' on class '{1}'", Method, CodeClass.FullName);
                methodInstance.Invoke(codeInstance, parameterValues);
                logger.Debug("Invoke successfull");
            }
            catch (Exception e)
            {
                logger.Error("Error occured during execution of method {0} on type {1}", e, Method, CodeClass.FullName);
            }
            finally
            {
                if (disposableCode != null)
                    disposableCode.Dispose();
            }
        }

        /// <summary>
        /// Get an array of the types for each parameter excluding "class" and "method" parameters.
        /// </summary>
        internal static void SplitParameterTypesAndValues(object[] parameters, out Type[] types, out object[] values)
        {
            var typearray = new List<Type>(parameters.Length);
            var valuearray = new List<object>(parameters.Length);
            for (var i = 0; i < parameters.Length; i++)
            {
                var value = parameters[i];
                if (value == null)
                {
                    typearray.Add(typeof(object));
                    valuearray.Add(null);
                }
                else
                {
                    typearray.Add(value.GetType());
                    valuearray.Add(value);
                }
            }
            types = typearray.ToArray();
            values = valuearray.ToArray();
        }
    }
}