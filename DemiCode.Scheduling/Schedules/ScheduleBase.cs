using System;
using System.Xml;
using System.Xml.XPath;
using DemiCode.Scheduling.Alarms;
using System.Globalization;
using DemiCode.Shared.Factories;

namespace DemiCode.Scheduling.Schedules
{
    /// <summary>
    /// ScheduleBase represents a schedule with or without recurrence and implement some basics
    /// required by <see cref="ISchedule"/> implementations.
    /// </summary>
    /// <remarks>
    /// <para>You can inherit from this class to add more parameters or
    /// create more complex recurrence schedules. (Like the last tuesday of every second month,
    /// or set repeat count or end time)
    /// Override the <c>MoveToFirstFrom(DateTime dateTime)</c> method to implement
    /// new recurring schedules.</para>
    /// <para>Current occurrence should always be stored in the protected field
    /// mCurrentOccurence. The <c>MoveToFirstFrom(DateTime dateTime)</c>
    /// method changes the mCurrentOccurence field.</para>
    /// <para>CompareTo compares the currentOccurence of two ISchedule instances.</para>
    /// <para>The ToString method can be overriden to give more informative results.</para>
    /// </remarks>
    public abstract class ScheduleBase : ISchedule
    {
        #region static factory stuff
        /// <summary>
        /// Creates a schedule object from an xmlnode.
        /// </summary>
        /// <remarks>
        /// The xmlnode must have a type attribute describing the Type and Assembly for the
        /// schedule to create.
        /// </remarks>
        /// <param name="scheduleNode">An XmlNode representing a schedule.</param>
        /// <returns>A schedule object.</returns>
        [Obsolete("Deprecated. Use configuration system instead.")]
        public static ISchedule CreateFromXml(XmlNode scheduleNode)
        {
            try
            {
                return Factory.CreateInstance<ISchedule>(scheduleNode);
            }
            catch
            {
                MessageLogger.LogError("Failed to create schedule.");
                throw;
            }
        }
        #endregion

        /// <summary>
        /// Internal holder for the name of this schedule
        /// </summary>
        private string _name;

        /// <summary>
        /// Internal holder for the first occurence of an item
        /// </summary>
        private DateTime _firstOccurrence;

        /// <summary>
        /// Internal holder for the current occurrence of this ISchedule
        /// </summary>
        private DateTime _currentOccurrence = DateTime.MinValue;

        /// <summary>
        /// Internal holder for user defined additional data that follows this object. Can be used to hold an identifier.
        /// </summary>
        private object _additionalData;

        /// <summary>
        /// Fires when schedule goes off.
        /// </summary>
        public event EventHandler<AlarmEventArgs> Alarm;

        /// <summary>
        /// Fires when schedule ends.
        /// </summary>
        public event EventHandler<EventArgs> EndOfSchedule;

        #region accessor properties
        /// <summary>
        /// The name of this <see cref="ISchedule"/>.
        /// </summary>
        public string Name { get { return _name; } protected set { _name = value; } }

        /// <summary>
        /// The first occurence of this <see cref="ISchedule"/>.
        /// </summary>
        public DateTime FirstOccurrence
        {
            get { return _firstOccurrence; }
            protected set
            {

                _firstOccurrence = value;
            }
        }

        /// <summary>
        /// The current occurrence of this <see cref="ISchedule"/>.
        /// </summary>
        public DateTime CurrentOccurrence { get { return _currentOccurrence; } protected set { _currentOccurrence = value; } }

        /// <summary>
        /// User-defined data associated with the object.
        /// </summary>
        public object AdditionalData { get { return _additionalData; } set { _additionalData = value; } }
        #endregion

        #region ctors

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <remarks>
        /// This constructor will set FirstOccurrence to <see cref="DateTime.MinValue"/> and AdditionalData to null.
        /// </remarks>
        /// <param name="name">The name of this Schedule.</param>
        protected ScheduleBase(string name)
            : this(name, DateTime.MinValue, null)
        {
        }

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <remarks>
        /// This constructor will set FirstOccurrence to <see cref="DateTime.MinValue"/>.
        /// </remarks>
        /// <param name="name">The name of this Schedule.</param>
        /// <param name="additionalData">User-defined data associated with the object.</param>
        protected ScheduleBase(string name, object additionalData)
            : this(name, DateTime.MinValue, additionalData)
        {
        }

        /// <summary>
        /// Abstract base class constructor.
        /// </summary>
        /// <remarks>
        /// This constructor sets the Name, FirstOccurrence and AdditionalData properties of the created schedule.
        /// </remarks>
        /// <param name="name">The name of this Schedule.</param>
        /// <param name="firstOccurrence">First scheduled occurrence for this Schedule.</param>
        /// <param name="additionalData">User-defined data associated with the object.</param>
        protected ScheduleBase(string name, DateTime firstOccurrence, object additionalData)
        {
            _name = name;
            _firstOccurrence = firstOccurrence;
            _additionalData = additionalData;
        }

        /// <summary>
        /// Abstract base class constructor for accepting XML configuration. 
        /// </summary>
        /// <remarks>
        /// This constructor sets the <see cref="Name"/> property of the instance. All other
        /// initialization must be done in the created class' constructors. 
        /// </remarks>
        /// <example>
        /// When overriding this constructor, make sure that the base constructor is called
        /// <code>
        /// public Schedule(XmlNode config) : base(config)
        /// {
        ///  ....
        /// }
        /// </code>
        /// </example>
        /// <param name="config">The XML node to get configuration from.</param>
        protected ScheduleBase(XmlNode config)
        {
            if (config == null)
                throw new ArgumentNullException("config", "Config parameter cannot be null");

            XmlAttribute name = config.Attributes["name"];
            if (name != null)
            {
                _name = name.Value;
            }
        }
        #endregion

        #region ctor helpers
        /// <summary>
        /// Delegate for TryParse methods.
        /// 
        /// </summary>
        /// <typeparam name="T">The type of the output value</typeparam>
        /// <param name="inValue">Value to parse</param>
        /// <param name="outValue">Out parameter to put the parsed data into</param>
        /// <returns>True if succeeded, false if data can't be parsed to the appropriate type.</returns>
        internal delegate bool TryParseFunction<T>(string inValue, out T outValue);

        /// <summary>
        /// Tests and sets outValue to the value of the elementName element in inNode
        /// 
        /// </summary>
        /// <remarks>
        /// Checks for the presense and checks the validity of the element data, and throws the
        /// appropriate exceptions if data is not present or valid.
        /// </remarks>
        /// <exception cref="System.ArgumentNullException">Element either doesn't exist.</exception>
        /// <exception cref="System.ArgumentException">element contains invalid data.</exception>
        /// <typeparam name="T">Type of value for outValue</typeparam>
        /// <param name="inNode">The config containing the element to test and get.</param>
        /// <param name="elementName">The name of the element containing the data to get.</param>
        /// <param name="tryParseFunction">The function used to parse the data to the requested type. <seealso cref="DemiCode.Scheduling.Schedules.ScheduleBase.TryParseFunction&lt;T&gt;"/></param>
        /// <returns>The parsed element value.</returns>
        static internal T ParseElementValue<T>(IXPathNavigable inNode, string elementName, TryParseFunction<T> tryParseFunction)
        {
            T val;
            TestAndSetElementValue(inNode, elementName, out val, tryParseFunction);
            return val;
        }

        /// <summary>
        /// Tests and sets outValue to the value of the elementName element in inNode
        /// 
        /// </summary>
        /// <remarks>
        /// Checks for the presense and checks the validity of the element data, and throws the
        /// appropriate exceptions if data is not present or valid.
        /// </remarks>
        /// <exception cref="System.ArgumentNullException">Element doesn't exist.</exception>
        /// <exception cref="System.ArgumentException">element contains invalid data.</exception>
        /// <typeparam name="T">Type of value for outValue</typeparam>
        /// <param name="inNode">The config containing the element to test and get.</param>
        /// <param name="elementName">The name of the element containing the data to get.</param>
        /// <param name="outValue">Out parameter to put the outData into.</param>
        /// <param name="tryParseFunction">The function used to parse the data to the requested type. <seealso cref="DemiCode.Scheduling.Schedules.ScheduleBase.TryParseFunction&lt;T&gt;"/></param>
        static internal void TestAndSetElementValue<T>(IXPathNavigable inNode, string elementName, out T outValue, TryParseFunction<T> tryParseFunction)
        {
            XPathNavigator element = inNode.CreateNavigator().SelectSingleNode(elementName);
            if (element == null)
                throw new ArgumentNullException("inNode", String.Format(CultureInfo.InvariantCulture, "The '{0}' config doesn't exist.", elementName));

            AssignElementValue(element, elementName, tryParseFunction, out outValue);
        }

        private static void AssignElementValue<T>(XPathNavigator element, string elementName, TryParseFunction<T> tryParseFunction, out T outValue)
        {
            string inValue = element.Value;
            if ((inValue == null) || (!tryParseFunction(inValue, out outValue)))
                throw new ArgumentException(String.Format(CultureInfo.InvariantCulture, "The '{0}' config contains invalid data.", elementName), "element");
        }

        /// <summary>
        /// Tests and sets outValue to the value of the elementName element in inNode
        /// 
        /// </summary>
        /// <remarks>
        /// Checks for the presense and checks the validity of the element data, and throws the
        /// appropriate exceptions if data is not present or valid.
        /// </remarks>
        /// <exception cref="System.ArgumentNullException">Element doesn't exist.</exception>
        /// <exception cref="System.ArgumentException">element contains invalid data.</exception>
        /// <typeparam name="T">Type of value for outValue</typeparam>
        /// <param name="inNode">The config containing the element to test and get.</param>
        /// <param name="elementName">The name of the element containing the data to get.</param>
        /// <param name="outValue">Out parameter to put the outData into.</param>
        /// <param name="tryParseFunction">The function used to parse the data to the requested type. <seealso cref="DemiCode.Scheduling.Schedules.ScheduleBase.TryParseFunction&lt;T&gt;"/></param>
        /// <param name="defaultValue">The default value to use if the setting is missing</param>
        static internal void TestAndSetElementValue<T>(IXPathNavigable inNode, string elementName, out T outValue, TryParseFunction<T> tryParseFunction, T defaultValue)
        {
            XPathNavigator element = inNode.CreateNavigator().SelectSingleNode(elementName);
            if (element == null)
            {
                outValue = defaultValue;
                return;
            }

            AssignElementValue(element, elementName, tryParseFunction, out outValue);
        }
        #endregion

        /// <summary>
        /// Changes <see cref="CurrentOccurrence"/> to the next occurence
        /// </summary>
        /// <returns>true if there is a next occurence, false if not</returns>
        public virtual bool MoveToNext()
        {
            return MoveToFirstFrom(_currentOccurrence + TimeSpan.FromTicks(1L));
        }

        /// <summary>
        /// Changes <see cref="CurrentOccurrence"/> to the first possible occurence from the supplied time.
        /// </summary>
        /// <param name="time">The time to calculate from.</param>
        /// <returns>True if new time found, false if schedule is ended.</returns>
        public abstract bool MoveToFirstFrom(DateTime time);

        /// <summary>
        /// Compares the currentOccurrence of two Schedules (or subclasses of Schedule)
        /// </summary>
        /// <param name="value">A Schedule to compare with this one</param>
        /// <returns>A signed number indicating the relative values of this instance and the <paramref name="value"/> parameter.
        /// <list type="table">
        /// <listheader><term>Value</term><description>Description</description></listheader>
        /// <item><term>Less than zero</term><description>This instance is less than <paramref name="value"/>.</description></item>
        /// <item><term>Zero</term><description>This instance is equal to <paramref name="value"/>.</description></item>
        /// <item><term>Greater than zero</term><description>This instance is greater than <paramref name="value"/>.</description></item>
        /// </list>
        /// </returns>
        public int CompareTo(ISchedule value)
        {
            if (value == null)
                throw new ArgumentNullException("value", "Parameter can't be null.");
            return _currentOccurrence.CompareTo(value.CurrentOccurrence);
        }

        /// <summary>
        /// Called by <see cref="Scheduler"/> to signal the event scheduled at <see cref="CurrentOccurrence"/>.
        /// Raises the Alarm event.
        /// </summary>
        /// <param name="actualSignalTime">The actual time the system event was raised.</param>
        public void ItsTime(DateTime actualSignalTime)
        {
            EventHelper.RaiseEvents(this, Alarm, new AlarmEventArgs(_currentOccurrence, actualSignalTime, _additionalData));
        }

        /// <summary>
        /// Called by <see cref="Scheduler"/> to signal the schedule is finished.
        /// Raises the EndOfSchedule event
        /// </summary>
        public void ItsOver()
        {
            EventHelper.RaiseEvents(this, EndOfSchedule, null);
        }

        /// <summary>
        /// Creates a string representation of the current object.
        /// </summary>
        /// <returns>A string</returns>
        public override string ToString()
        {
            return String.Format(CultureInfo.InvariantCulture, "{0}(\"{2}\") CurrentOccurrence = {1:yyyyMMdd hh:mm:ss.fff}", GetType(), CurrentOccurrence, Name);
        }
    }
}
