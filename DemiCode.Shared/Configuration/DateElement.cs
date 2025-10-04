using System;

namespace DemiCode.Configuration
{
    /// <summary>
    /// A configuration element for representing <see cref="DateTime"/> in XML elements.
    /// </summary>
    /// <remarks>Only the date part will be preserved with this element.</remarks>
    public class DateElement : DateTimeElement
    {
        /// <summary>
        /// Initialize a new instance of <see cref="DateElement"/>.
        /// </summary>
        public DateElement()
        : base("yyyy-MM-dd")
        { }

        /// <summary>
        /// Initialize a new instance of <see cref="DateElement"/> with <see cref="DateTimeElement.Value"/> set to <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The value to use.</param>
        public DateElement(DateTime value)
            : this()
        {
            Value = value;
        }
    }
}
