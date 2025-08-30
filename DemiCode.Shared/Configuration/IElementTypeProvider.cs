using System;
using System.Configuration;

namespace DemiCode.Configuration
{
    /// <summary>
    /// This interface is used in conjunction with <see cref="PolymorphicElementCollection{V}"/>. 
    /// </summary>
    public interface IElementTypeProvider
    {
        /// <summary>
        /// Return the <see cref="ConfigurationElement"/> class that implements the specified element name.
        /// </summary>
        /// <param name="elementName">The element name to get the type for.</param>
        /// <returns>A type implementing the specified element name.</returns>
        Type GetElementType(string elementName);
    }
}
