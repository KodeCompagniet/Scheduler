#if INCLUDE_LOGGING_EXTENSIONS

using System;
using System.Collections.Generic;
using System.Text;
using log4net.Core;
using log4net.Filter;

namespace DemiCode.Logging.log4net.Filters
{
    /// <summary>
    /// Simple filter to match a string in the name of an event exception.
    /// </summary>
    public class ExceptionFilter : StringMatchFilter
    {
        /// <summary>
        /// Check if this filter should allow the event to be logged
        /// </summary>
        /// <param name="loggingEvent">the event being logged</param>
        /// <returns>see remarks</returns>
        /// <remarks>
        /// <para>
        /// The event exception is matched against 
        /// the <see cref="StringMatchFilter.StringToMatch"/>.
        /// If the <see cref="StringMatchFilter.StringToMatch"/> occurs as a substring within
        /// the exception then a match will have occurred. If no match occurs, or the event have no exception,
        /// this function will return <see cref="FilterDecision.Neutral"/>
        /// allowing other filters to check the event. If a match occurs then
        /// the value of <see cref="StringMatchFilter.AcceptOnMatch"/> is checked. If it is
        /// true then <see cref="FilterDecision.Accept"/> is returned otherwise
        /// <see cref="FilterDecision.Deny"/> is returned.
        /// </para>
        /// </remarks>
        override public FilterDecision Decide(LoggingEvent loggingEvent)
        {
            if (loggingEvent == null)
            {
                throw new ArgumentNullException("loggingEvent");
            }

            var exception = loggingEvent.GetExceptionString();

            // Check if we have been setup to filter
            if (String.IsNullOrEmpty(exception) || (m_stringToMatch == null && m_regexToMatch == null))
            {
                // We cannot filter so allow the filter chain
                // to continue processing
                return FilterDecision.Neutral;
            }

            // Firstly check if we are matching using a regex
            if (m_regexToMatch != null)
            {
                // Check the regex
                if (m_regexToMatch.Match(exception).Success == false)
                {
                    // No match, continue processing
                    return FilterDecision.Neutral;
                }

                // we've got a match
                return m_acceptOnMatch ? FilterDecision.Accept : FilterDecision.Deny;
            }
            
            if (m_stringToMatch != null)
            {
                // Check substring match
                if (exception.IndexOf(m_stringToMatch) == -1)
                {
                    // No match, continue processing
                    return FilterDecision.Neutral;
                }

                // we've got a match
                return m_acceptOnMatch ? FilterDecision.Accept : FilterDecision.Deny;
            }

            return FilterDecision.Neutral;
        }
    }
}

#endif