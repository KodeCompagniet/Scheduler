using System;
using System.Collections.Generic;
using System.Configuration;
using DemiCode.Scheduling.Tasks;

namespace DemiCode.Scheduling.Configuration.Tasks
{
    /// <summary>
    /// Strong typed class containing a <see cref="TaskSequence"/> configuration element read from a configuration file.
    /// </summary>
    /// <remarks>This class is intended to be used by the <see cref="TaskElementCollection"/> class</remarks>
    public class TaskSequenceElement : TaskElementBase, IEquatable<TaskSequenceElement>
    {
        /// <summary>
        /// Construct a new <see cref="TaskSequenceElement"/> instance.
        /// </summary>
        public TaskSequenceElement()
            : base("TaskSequence")
        {
        }

        /// <summary>
        /// Gets the collection of defined <see cref="TaskElementBase"/> objects.
        /// </summary>
        /// <value>
        /// The collection of defined <see cref="TaskElementBase"/> objects.
        /// </value>
        [ConfigurationProperty("tasks", IsDefaultCollection = false, IsRequired = false)]
        public TaskElementCollection Tasks
        {
            get { return (TaskElementCollection)base["tasks"]; }
        }

        ///<summary>
        ///Called after deserialization.
        ///</summary>
        ///
        protected override void PostDeserialize()
        {
            base.PostDeserialize();
            Validate();
        }

        /// <summary>
        /// Validate that <see cref="Tasks"/> is not empty.
        /// </summary>
        private void Validate()
        {
            // No tasks? 
            if (Tasks.Count == 0)
            {
                throw new ConfigurationErrorsException("tasks collection cannot be empty");
            }

            // Tasks with schedulerefs?
            foreach (TaskElementBase taskElement in Tasks)
            {
                if (taskElement.Count > 0)
                {
                    throw new ConfigurationErrorsException("Tasks in sequence cannot have scheduleref's");
                }
            }
        }

        /// <summary>
        /// Get an instance of the task type.
        /// </summary>
        /// <returns>A <see cref="TaskSequence"/> instance.</returns>
        public override ITask GetTaskInstance()
        {
            var taskElements = SortElements(Tasks);
            var tasks = new List<ITask>(taskElements.Count);

            for (var i = 0; i < taskElements.Count; i++)
            {
                tasks.Add(taskElements[i].GetTaskInstance());
            }

            return new TaskSequence(tasks.ToArray());
        }

        private static IList<TaskElementBase> SortElements(TaskElementCollection taskElementCollection)
        {
            var tasks = new SortedList<int, TaskElementBase>(taskElementCollection.Count);
            foreach (TaskElementBase taskElement in taskElementCollection)
            {
                tasks.Add(taskElement.ElementInformation.LineNumber, taskElement);
            }
            return tasks.Values;
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
        public bool Equals(TaskSequenceElement compareTo)
        {
            if (ReferenceEquals(this, compareTo)) return true;
            if (!base.Equals(compareTo)) return false;
            return Tasks.Equals(compareTo.Tasks);
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
            return Equals(compareTo as TaskSequenceElement);
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

    #region Method validator
    //internal class MethodOnClassValidator : ConfigurationValidatorBase
    //{
    //    public override bool CanValidate(Type type)
    //    {
    //        return (type == typeof(string));
    //    }

    //    public override void Validate(object value)
    //    {
    //        if (String.IsNullOrEmpty((string)value))
    //            throw new ConfigurationErrorsException("The value cannot be empty");
    //    }
    //}
    //internal class MethodOnClassValidatorAttribute : ConfigurationValidatorAttribute
    //{
    //    public override ConfigurationValidatorBase ValidatorInstance { get { return new MethodOnClassValidator(); } }
    //}
    #endregion
}
