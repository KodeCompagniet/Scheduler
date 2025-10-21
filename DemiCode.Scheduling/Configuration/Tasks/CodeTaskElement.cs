using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using DemiCode.Scheduling.Tasks;

namespace DemiCode.Scheduling.Configuration.Tasks
{
    /// <summary>
    /// Strong typed class containing a <see cref="CodeTask"/> configuration element read from a configuration file.
    /// </summary>
    /// <remarks>This class is intended to be used by the <see cref="TaskElementCollection"/> class</remarks>
    public class CodeTaskElement : TaskElementBase, IEquatable<CodeTaskElement>
    {
        /// <summary>
        /// Construct a new <see cref="CodeTaskElement"/> instance.
        /// </summary>
        public CodeTaskElement()
            : base("CodeTask")
        {
        }

        /// <summary>
        /// Construct a new <see cref="CodeTaskElement"/> instance.
        /// </summary>
        /// <param name="type">The class of the code to execute. </param>
        /// <param name="method">The method on the <see cref="Class"/> to execute.</param>
        /// <param name="parameters">A list of parameters to pass to the method.</param>
        public CodeTaskElement(Type type, string method, params string[] parameters)
            : this()
        {
            Class = type;
            Method = method;

            for (int i = 0; i < parameters.Length; i++)
            {
                AddAdditionalProperty("param" + i, parameters[i]);
            }
        }

        /// <summary>
        /// The class of the code to execute. 
        /// </summary>
        [ConfigurationProperty("class", IsRequired = true, IsKey = false)]
        [TypeConverter(typeof(TypeNameConverter))]
        public Type Class
        {
            get { return ((Type)base["class"]); }
            set { base["class"] = value; }
        }

        /// <summary>
        /// The method on the <see cref="Class"/> to execute.
        /// </summary>
        [ConfigurationProperty("method", IsRequired = true, IsKey = false)]
        public string Method
        {
            // WANTED:   validation of method name against the current class. 
            //                      This cannot be done due to the current architecture of the ConfigurationValidator system
            // SOLUTION: Use PostDeserialize to do validation
            get { return ((string)base["method"]); }
            set { base["method"] = value; }
        }


        /// <summary>
        /// Return the <see cref="CodeTask"/> instance for this configuration element.
        /// </summary>
        /// <returns>A <see cref="CodeTask"/> instance.</returns>
        public override ITask GetTaskInstance()
        {
            var parameters = AdditionalProperties;
            var values = new List<string>(parameters.Values);
            return new CodeTask(Class, Method, values.ToArray());
        }

        /// <summary>
        /// Validate that <see cref="Method"/> is actually a valid method on <see cref="Class"/>
        /// </summary>
        protected override void PostDeserialize()
        {
            base.PostDeserialize();
            Validate();
        }

        ///<summary>
        ///Called before serialization.
        ///</summary>
        ///
        ///<param name="writer">The <see cref="T:System.Xml.XmlWriter"></see> object that will be used to serialize the <see cref="T:System.Configuration.ConfigurationElement"></see> object.</param>
        protected override void PreSerialize(System.Xml.XmlWriter writer)
        {
            base.PreSerialize(writer);
            Validate();
        }

        private void Validate()
        {
            if (String.IsNullOrEmpty(Method))
                throw new ConfigurationErrorsException("The value of 'method' cannot be empty");

            Type[] types;
            object[] values;
            SplitParameterTypesAndValues(AdditionalProperties, out types, out values);
            var method = Class.GetMethod(Method, types);
            if (method == null)
                throw new ConfigurationErrorsException(String.Format("'{0}' is not a valid method on '{1}'. Check that both method name and parameter list matches the implementation", Method, Class.FullName));

            if (!method.IsStatic && (Class.GetConstructor(new Type[0]) == null))
                throw new ConfigurationErrorsException(String.Format("Non-static method '{0}' requires a parameterless constructor on class '{1}'", Method, Class.FullName));
        }

        private static void SplitParameterTypesAndValues(Dictionary<string, string> parameters, out Type[] types, out object[] values)
        {
            var valueList = new List<string>(parameters.Values);
            types = new Type[valueList.Count];
            values = new object[valueList.Count];

            for (var i = 0; i < valueList.Count; i++)
            {
                var val = valueList[i];
                if (val == null)
                {
                    types[i] = typeof(object);
                    values[i] = null;
                }
                else
                {
                    types[i] = val.GetType();
                    values[i] = val;
                }
            }
        }

        ///<summary>
        ///Indicates whether the current object is equal to another object of the same type.
        ///</summary>
        ///
        ///<returns>
        ///true if the current object is equal to the other parameter; otherwise, false.
        ///</returns>
        ///
        ///<param name="compareTo">An object to compare with this object.</param>
        public bool Equals(CodeTaskElement compareTo)
        {
            if (compareTo == null) return false;
            if (ReferenceEquals(this, compareTo)) return true;

            if (!Equals(Class, compareTo.Class)) return false;
            if (!Equals(Method, compareTo.Method)) return false;

            return base.Equals(compareTo);
        }

        ///<summary>
        ///Compares the <see cref="T:System.Configuration.ConfigurationElementCollection"></see> to the specified object.
        /// Elements in this collection will be compared to elements in the <paramref name="compareTo"/> collection.
        /// Additionally, any properties defined on this collection will be compared.
        ///</summary>
        ///
        ///<returns>
        ///true if the object to compare with is equal to the current <see cref="T:System.Configuration.ConfigurationElementCollection"></see> instance; otherwise, false. The default is false.
        ///</returns>
        ///
        ///<param name="compareTo">The object to compare. </param>
        public override bool Equals(object compareTo)
        {
            return Equals(compareTo as CodeTaskElement);
        }

        ///<summary>
        ///Serves as a hash function for this type.
        ///</summary>
        ///<returns>A hash code for the current object.</returns>
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

}
