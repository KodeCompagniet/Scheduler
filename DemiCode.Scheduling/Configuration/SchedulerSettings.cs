using System;
using System.Configuration;
using System.Globalization;
using DemiCode.Configuration;

namespace DemiCode.Scheduling.Configuration
{
    /// <summary>
    /// Configuration section handler class for loading scheduler scheduler configuration from .config files.
    /// </summary>
    /// <example>
    /// <code lang="cs" title="This example retrieves a named section and iterates over the configured schedulers">
    /// SchedulerSettings ss = ConfigurationManager.GetSection("scheduling") as SchedulerSettings;
    /// if (ss != null)
    /// {
    ///     string defaultScheduler = ss.DefaultScheduler;
    ///     foreach(SchedulerElement se in ss.Schedulers)
    ///     {
    ///         string name = se.Name;
    ///     }
    /// }
    /// </code>
    /// <code lang="xml" title="The corresponding configuration file looks like this">
    /// &lt;configSections&gt;
    ///     &lt;section name="scheduling" 
    ///         type="DemiCode.Scheduling.Configuration.SchedulerSettings, DemiCode.Scheduling, Culture=neutral, PublicKeyToken=069d10bcdee4c3e4"/&gt;
    /// &lt;/configSections&gt;
    /// 
    /// &lt;scheduling defaultScheduler="prod"&gt;
    ///     &lt;schedulers&gt;
    ///         &lt;scheduler name="prod"&gt;
    ///             &lt;WeeklySchedule name="WeeklySchedule"&gt;
    ///                 &lt;fromDate&gt;2006-01-01&lt;/fromDate&gt;
    ///                 &lt;toDate&gt;2006-12-31&lt;/toDate&gt;
    ///                 &lt;timeOfDay&gt;22:00:00&lt;/timeOfDay&gt;
    ///                 &lt;days&gt;
    ///                     &lt;day&gt;monday&lt;/day&gt;
    ///                     &lt;day&gt;tuesday&lt;/day&gt;
    ///                     &lt;day&gt;wednesday&lt;/day&gt;
    ///                 &lt;/days&gt;
    ///             &lt;/WeeklySchedule&gt;
    ///         &lt;/scheduler&gt;
    ///         &lt;scheduler name="test"&gt;
    ///             &lt;IntervalCountedSchedule name="IntervalCountedSchedule"&gt;
    ///                 &lt;start&gt;2006-01-01 12:00:00&lt;/start&gt;
    ///                 &lt;occurrences&gt;31&lt;/occurrences&gt;
    ///                 &lt;interval&gt;1.00:00:00&lt;/interval&gt;
    ///             &lt;/IntervalCountedSchedule&gt;
    ///         &lt;/scheduler&gt;
    ///     &lt;/schedulers&gt;
    /// &lt;/scheduling&gt;
    /// </code>
    /// </example>
    /// <seealso cref="ConfigurationManager"/>
    /// <seealso cref="ConfigurationManager.GetSection"/>
    public class SchedulerSettings : ConfigurationSection, IEquatable<SchedulerSettings>
    {
        private const string defaultSchedulerProperty = "defaultScheduler";
        private const string schedulersProperty = "schedulers";

        internal System.Configuration.Configuration WritableConfiguration;

        /// <summary>
        /// Defines the default scheduler instance to use.
        /// </summary>
        [ConfigurationProperty(defaultSchedulerProperty, IsRequired = true, DefaultValue = "")]
        public string DefaultScheduler
        {
            get { return (string)base[defaultSchedulerProperty]; }
            set { base[defaultSchedulerProperty] = value; }
        }

        /// <summary>
        /// Gets the collection of defined <see cref="SchedulerElement"/> objects.
        /// </summary>
        /// <value>
        /// The collection of defined <see cref="SchedulerElement"/> objects.
        /// </value>
        [ConfigurationProperty(schedulersProperty, IsDefaultCollection = false)]
        [ConfigurationCollection(typeof(SchedulerElementCollection))]
        public SchedulerElementCollection Schedulers
        {
            get { return (SchedulerElementCollection)base[schedulersProperty]; }
        }

        /// <summary>
        /// Validates that the scheduler named in <see cref="DefaultScheduler"/> actually 
        /// references a named scheduler element in <see cref="Schedulers"/>.
        /// </summary>
        protected override void PostDeserialize()
        {
            base.PostDeserialize();
            Validate();
        }

        private void Validate()
        {
            // Test if our default scheduler really exist
            if (!String.IsNullOrEmpty(DefaultScheduler))
            {
                if (Schedulers[DefaultScheduler] == null)
                    throw new InvalidConfigurationException(String.Format(CultureInfo.InvariantCulture,
                                                                          "The default scheduler '{0}' does not correspond to a configured scheduler element",
                                                                          DefaultScheduler));
            }
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

        ///<summary>
        ///Indicates whether the current object is equal to another <c>SchedulerSettings</c> object.
        ///</summary>
        ///
        ///<returns>
        ///true if the current object is equal to the other parameter; otherwise, false.
        ///</returns>
        ///
        ///<param name="compareTo">An object to compare with this object.</param>
        public bool Equals(SchedulerSettings compareTo)
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
            return Equals(compareTo as SchedulerSettings);
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
