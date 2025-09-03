#if INCLUDE_LOGGING_EXTENSIONS

using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using log4net.Core;
using log4net.Layout;
using log4net.Layout.Pattern;

namespace DemiCode.Logging.log4net.Layout
{
 
    /// <summary>
    /// A <see cref="PatternLayout"/> class that adds some extra formatting tokens.
    /// </summary>
    /// <remarks>
    /// This layout adds the following token:
    /// 
    /// <list type="table">
    /// <listheader>
    /// <term>token</term>
    /// <description>token value</description>
    /// </listheader>
    /// <item>
    /// <term>exceptiontitle</term>
    /// <description>Formats the description of the exception into a unique identifiable string.</description>
    /// </item>
    /// </list>
    /// </remarks>
    public class ExtendedPatternLayout : PatternLayout
    {
        #region Constructors

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <remarks>
        /// <para>
        /// Constructs a ExtendedPatternLayout.
        /// </para>
        /// </remarks>
        public ExtendedPatternLayout()
        {
            AddConverter("exceptiontitle", typeof(ExceptionTitlePatternConverter));
        }

        #endregion

        #region Override implementation of LayoutSkeleton

        ///// <summary>
        ///// Gets the exception text from the logging event
        ///// </summary>
        ///// <param name="writer">The TextWriter to write the formatted event to</param>
        ///// <param name="loggingEvent">the event being logged</param>
        ///// <remarks>
        ///// <para>
        ///// Write the exception string to the <see cref="TextWriter"/>.
        ///// The exception string is retrieved from <see cref="LoggingEvent.GetExceptionString()"/>.
        ///// </para>
        ///// </remarks>
        //override public void Format(TextWriter writer, LoggingEvent loggingEvent)
        //{
        //    if (writer == null)
        //        throw new ArgumentNullException("writer");

        //    if (loggingEvent == null)
        //        throw new ArgumentNullException("loggingEvent");

        //    if (loggingEvent.ExceptionObject != null)
        //    {
        //        writer.Write(GetExceptionDescription(loggingEvent.ExceptionObject));
        //    }
        //}

        #endregion
    }

    /// <summary>
    /// Formats the description of the exception into a unique identifiable string.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This Layout must be used as a converter with <see cref="PatternLayout"/>.
    /// </para>
    /// </remarks>
    public class ExceptionTitlePatternConverter : PatternLayoutConverter
    {
        ///<summary>
        ///            Derived pattern converters must override this method in order to
        ///            convert conversion specifiers in the correct way.
        ///</summary>
        ///
        ///<param name="writer"><see cref="T:System.IO.TextWriter" /> that will receive the formatted result.</param>
        ///<param name="loggingEvent">The <see cref="T:log4net.Core.LoggingEvent" /> on which the pattern converter should be executed.</param>
        override protected void Convert(TextWriter writer, LoggingEvent loggingEvent)
        {
            if (loggingEvent.ExceptionObject != null)
                writer.Write(GetExceptionDescription(loggingEvent.ExceptionObject));
        }

        /// <summary>
        /// Formats the description of the exception into a unique identifiable string.
        /// </summary>
        /// <remarks>
        /// The reason for this method and not just a simpler way of producing the description is that this
        /// string will be used to find existing bugs in the database to add occurances to, instead of adding
        /// new bugs for each occurance.
        /// </remarks>
        /// <param name="ex">The Exception object to copy information from. This parameter cannot be null or an 
        /// ArgumentNullException exception will be thrown.
        /// </param>
        /// <returns>The formatted description string</returns>
        private static string GetExceptionDescription(Exception ex)
        {
            return GetExceptionDescription(ex, "{0}.{1}.{2}.{3}");
        }

        /// <summary>
        /// Formats the description of the exception into a unique identifiable string.
        /// </summary>
        /// <remarks>
        /// The reason for this method and not just a simpler way of producing the description is that this
        /// string will be used to find existing bugs in the database to add occurances to, instead of adding
        /// new bugs for each occurance.
        /// </remarks>
        /// <param name="ex">The Exception object to copy information from. This parameter cannot be null or an 
        /// ArgumentNullException exception will be thrown.
        /// </param>
        /// <param name="versionFormat">A string used to format the version number. This string will be passed to String.Format,
        /// and the four parameters given will be major, minor, build, and revision version numbers. This parameter
        /// cannot be null or an ArgumentNullException exception will be thrown.
        /// </param>
        /// <returns>The formatted description string</returns>
        private static string GetExceptionDescription(Exception ex, string versionFormat)
        {
            if (ex == null) throw new ArgumentNullException(nameof(ex));
            if (String.IsNullOrEmpty(versionFormat)) throw new ArgumentNullException(nameof(versionFormat));

            var desc = new StringBuilder();

            // We first want the class name of the exception that occured
            desc.Append(ex.GetType().Name);

            // If the exception has a property called ErrorCode, add the value of it to the desc
            var rePropertyName = new Regex("^(ErrorCode|HResult)$", RegexOptions.IgnoreCase);
            foreach (var property in ex.GetType().GetProperties())
            {
                if (!rePropertyName.Match(property.Name).Success) continue;

                // Only deal with readable properties
                if (!property.CanRead) continue;

                // Only deal with properties that aren't indexed
                if (property.GetIndexParameters().Length != 0) continue;

                // Only add property values that are not null
                var propertyValue = property.GetValue(ex, new Object[] { });
                if (propertyValue == null) continue;

                // If the property value converted to a string yields the same name as the class
                // name of the value, it is uninteresting
                var propertyValueString = propertyValue.ToString();
                if (propertyValueString != propertyValue.GetType().FullName)
                    desc.AppendFormat(" {0}={1}", property.Name, propertyValueString);
            }

            // Work out the first source code reference in the stacktrace and add the unique value for it
            var reSourceReference = new Regex("at\\s+.+\\.(?<methodname>[^)]+)\\(.*\\)\\s+in\\s+.+\\\\(?<filename>[^:\\\\]+):line\\s+(?<linenumber>[0-9]+)", RegexOptions.IgnoreCase);
            var gotReference = false;
            if (ex.StackTrace != null)
            {
                foreach (var line in ex.StackTrace.Split('\n', '\r'))
                {
                    var ma = reSourceReference.Match(line);
                    if (!ma.Success) continue;

                    desc.AppendFormat(" ({0}:{1}:{2})",
                                      ma.Groups["filename"].Value,
                                      ma.Groups["methodname"].Value,
                                      ma.Groups["linenumber"].Value);
                    gotReference = true;
                    break;
                }
            }

            // If we didn't get a source reference (release compile ?), try to find a non-System.* reference
            if (!gotReference)
            {
                var reMethodReference = new Regex("at\\s+(?<methodname>[^(]+)\\(.*\\)", RegexOptions.IgnoreCase);
                if (ex.StackTrace != null)
                {
                    foreach (var line in ex.StackTrace.Split('\n', '\r'))
                    {
                        var ma = reMethodReference.Match(line);
                        if (!ma.Success) continue;
                        if (ma.Groups["methodname"].Value.ToUpper().StartsWith("SYSTEM.")) continue;

                        desc.AppendFormat(" ({0})", ma.Groups["methodname"].Value);
                        break;
                    }
                }
            }

            // If we can get the entry assembly, add the version number of it
            var entryAssembly = Assembly.GetEntryAssembly();
            if (entryAssembly != null)
            {
                var entryAssemblyName = entryAssembly.GetName();
                if (entryAssemblyName.Version != null)
                {
                    var version = String.Format(versionFormat,
                                                entryAssemblyName.Version.Major,
                                                entryAssemblyName.Version.Minor,
                                                entryAssemblyName.Version.Build,
                                                entryAssemblyName.Version.Revision);
                    desc.AppendFormat(" v{0}", version);
                }
            }

            // Return result
            return desc.ToString();
        }
    }
}

#endif
