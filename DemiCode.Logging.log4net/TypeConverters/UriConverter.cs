#if INCLUDE_LOGGING_EXTENSIONS

using System;
using log4net.Util.TypeConverters;

namespace DemiCode.Logging.log4net.TypeConverters
{
    public class UriConverter: IConvertFrom
    {
        /// <summary>
        ///             Can the source type be converted to the type supported by this object
        /// </summary>
        /// <param name="sourceType">the type to convert</param>
        /// <returns>
        /// true if the conversion is possible
        /// </returns>
        /// <remarks>
        /// <para>
        ///             Test if the <paramref name="sourceType" /> can be converted to the
        ///             type supported by this converter.
        /// </para>
        /// </remarks>
        public bool CanConvertFrom(Type sourceType)
        {
            return (sourceType == typeof(string) || sourceType == typeof(Uri)) ;
        }

        /// <summary>
        ///             Convert the source object to the type supported by this object
        /// </summary>
        /// <param name="source">the object to convert</param>
        /// <returns>
        /// the converted object
        /// </returns>
        /// <remarks>
        /// <para>
        ///             Converts the <paramref name="source" /> to the type supported
        ///             by this converter.
        /// </para>
        /// </remarks>
        public object ConvertFrom(object source)
        {
            if (source == null)
                return null;
            
            var val = source as string;
            if (val != null)
            {
                return new Uri(val);
            }

            var uri = source as Uri;
            if (uri != null)
                return uri;

            throw ConversionNotSupportedException.Create(typeof(Uri), source);
        }
    }
}

#endif