using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using DemiCode.Scheduling.Tasks;
using DemiCode.Configuration;

namespace DemiCode.Scheduling.Configuration.Tasks
{
    /// <summary>
    /// Strong typed class containing a <see cref="ApplicationTask"/> configuration element read from a configuration file.
    /// </summary>
    /// <remarks>This class is intended to be used by the <see cref="TaskElementCollection"/> class.</remarks>
    public class ApplicationTaskElement : TaskElementBase, IEquatable<ApplicationTaskElement>
    {
        /// <summary>
        /// Construct a new <see cref="ApplicationTaskElement"/> instance.
        /// </summary>
        public ApplicationTaskElement()
            : base("ApplicationTask")
        {
        }

        /// <summary>
        /// Construct a new <see cref="ApplicationTaskElement"/> instance.
        /// </summary>
        /// <param name="path">The path to the command to execute.</param>
        /// <param name="arguments">A list of arguments to pass to the executable.</param>
        public ApplicationTaskElement(string path, params string[] arguments)
            : this()
        {
            Path = path;

            for (int i = 0; i < arguments.Length; i++)
            {
                AddAdditionalProperty("arg"+i, arguments[i]);    
            }
        }

        /// <summary>
        /// The path to the command to execute.
        /// </summary>
        [ConfigurationProperty("path", IsRequired = true, IsKey = false)]
        public string Path
        {
            get { return ((string)base["path"]); }
            set { base["path"] = value; }
        }

        /// <summary>
        /// Get an instance of the task type.
        /// </summary>
        /// <returns>An <see cref="ApplicationTask"/> instance.</returns>
        public override ITask GetTaskInstance()
        {
            var argsList = new List<string>(AdditionalProperties.Values);
            var args = new List<string>(argsList.Count);
            foreach (string arg in argsList)
            {
                args.Add(arg);
            }
            return new ApplicationTask(Path, args.ToArray());
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
        public bool Equals(ApplicationTaskElement compareTo)
        {
            if (compareTo == null) return false;
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
            if (ReferenceEquals(this, compareTo)) return true;
            return Equals(compareTo as ApplicationTaskElement);
        }
    }
}
