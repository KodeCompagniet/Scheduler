using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Text;

namespace DemiCode.Scheduling.Configuration.Tasks
{
    /// <summary>
    /// Strong typed class for containing a schedule reference element read from a configuration file.
    /// </summary>
    /// <remarks>This class is intended to be used by the <see cref="TaskElementBase"/> class</remarks>
    public class ScheduleReferenceElement : ConfigurationElement, IEquatable<ScheduleReferenceElement>
    {
        /// <summary>
        /// Construct an empty <see cref="ScheduleReferenceElement"/> instance.
        /// </summary>
        public ScheduleReferenceElement() { }

        /// <summary>
        /// Construct an empty <see cref="ScheduleReferenceElement"/> instance.
        /// </summary>
        /// <param name="scheduleName">Name of the schedule to reference</param>
        public ScheduleReferenceElement(string scheduleName)
        {
            ScheduleName = scheduleName;
        }

        /// <summary>
        /// The name of a schedule element that a task will be connected to. This attribute must be unique 
        /// within the list of schedule references.
        /// </summary>
        [ConfigurationProperty("name", IsRequired = true, IsKey = true)]
        public string ScheduleName
        {
            get { return ((string)base["name"]); }
            set { base["name"] = value; }
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
        public bool Equals(ScheduleReferenceElement compareTo)
        {
            if (compareTo == null) return false;
            return base.Equals(compareTo);
        }

        ///<summary>
        ///Compares the current <see cref="T:System.Configuration.ConfigurationElement"></see> instance to the specified object.
        ///</summary>
        ///
        ///<returns>
        ///true if the object to compare with is equal to the current <see cref="T:System.Configuration.ConfigurationElement"></see> instance; otherwise, false. The default is false. 
        ///</returns>
        ///
        ///<param name="compareTo">The object to compare with.</param>
        public override bool Equals(object compareTo)
        {
            if (ReferenceEquals(this, compareTo)) return true;
            return Equals(compareTo as ScheduleReferenceElement);
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
