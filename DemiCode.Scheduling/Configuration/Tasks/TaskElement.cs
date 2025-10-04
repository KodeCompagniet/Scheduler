
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using DemiCode.Scheduling.Tasks;
using DemiCode.Shared.Factories;

namespace DemiCode.Scheduling.Configuration.Tasks
{
    /// <summary>
    /// Strong typed class for containing a task element read from a configuration file.
    /// All <c>task</c> elements within the <c>scheduler/tasks</c> element will
    /// be loaded and exposed as a collection of <see cref="ITask"/> instances.
    /// </summary>
    /// <remarks>This class is intended to be used by the <see cref="TaskElementCollection"/> class</remarks>
    public class TaskElement : TaskElementBase, IEquatable<TaskElement>
    {
        /// <summary>
        /// Get an instance of the task type.
        /// </summary>
        /// <returns>An instance of the type specified in <see cref="TaskType"/>.</returns>
        public override ITask GetTaskInstance()
        {
            if (AdditionalProperties.Values.Count > 0)
            {
                var parameters = AdditionalProperties;
                var values = new List<string>(parameters.Values);
                return Factory.CreateInstance<ITask>(TaskType, values.ToArray());
            }

            return Factory.CreateInstance<ITask>(TaskType);
        }

        /// <summary>
        /// Construct an empty <see cref="TaskElement"/> instance.
        /// </summary>
        public TaskElement() : base("task")
        {
        }

        /// <summary>
        /// Construct an empty <see cref="TaskElement"/> instance.
        /// </summary>
        /// <param name="taskType">The task type. This type must implement the <see cref="ITask"/> interface.</param>
        /// <param name="parameters">A list of parameters to pass to the <paramref name="taskType"/> constructor.</param>
        public TaskElement(Type taskType, params string[] parameters)
            : this()
        {
            if (taskType == null)
                throw new ArgumentNullException("taskType");

            if (taskType.GetInterface(typeof(ITask).Name) == null)
            {
                throw new ArgumentException("Task type must implement the DemiCode.Scheduling.Tasks.ITask interface", "taskType");
            }

            for (int i = 0; i < parameters.Length; i++)
            {
                AddAdditionalProperty("param" + i, parameters[i]);
            }


            TaskType = taskType;
        }

        /// <summary>
        /// The fully qualified type of a task.
        /// </summary>
        [ConfigurationProperty("type", IsRequired = true, IsKey = false)]
        [TypeConverter(typeof(TypeNameConverter))]
        public Type TaskType
        {
            get { return ((Type)base["type"]); }
            set { base["type"] = value; }
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
        public bool Equals(TaskElement compareTo)
        {
            if (ReferenceEquals(this, compareTo)) return true;
            if (compareTo == null) return false;
            if (!Equals(TaskType, compareTo.TaskType)) return false;
            return base.Equals(compareTo);
        }

        ///<summary>
        ///Compares the <see cref="T:System.Configuration.ConfigurationElementCollection"></see> to the specified object.
        ///</summary>
        ///
        ///<returns>
        ///true if the object to compare with is equal to the current <see cref="T:System.Configuration.ConfigurationElementCollection"></see> instance; otherwise, false. The default is false.
        ///</returns>
        ///
        ///<param name="compareTo">The object to compare. </param>
        public override bool Equals(object compareTo)
        {
            return Equals(compareTo as TaskElement);
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
