
#if INCLUDE_LOGGING_EXTENSIONS

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using DemiCode.Logging.log4net.TypeConverters;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Util.TypeConverters;

namespace DemiCode.Logging.log4net.Appenders
{
    /// <summary>
    /// Appender that logs to a web resource.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <see cref="WebAppender"/> appends logging events to a web resource. 
    /// The appender can be configured to specify the resource by setting the <see cref="Url"/> property. 
    /// </para>
    /// 
    /// <para>
    /// Log events are posted to the web resource using the POST method. 
    /// The POST can take a number of parameters. Parameters are added using the <see cref="AddParameter"/>
    /// method. This adds a single <see cref="WebAppenderParameter"/> to the
    /// ordered list of parameters. The <see cref="WebAppenderParameter"/> specifies
    /// the parameter name and how the value should
    /// be generated using a <see cref="ILayout"/>.
    /// </para>
    /// 
    /// <para>
    /// The <see cref="WebAppender"/> inherits the <see cref="ForwardingAppender"/>. After logging an event to the web resource
    /// the event is forwarded to any referenced appender. The event will have an extra property "webappender:Result" set with the result string 
    /// from the web resource. 
    /// <code lang="C#" escaped="true">
    ///   string result = loggingEvent.Properties["webappender:Result"] as string;
    /// </code>
    /// </para>
    /// </remarks>
    /// <example>
    /// An example configuration to log to a FogBugz Bugscout url with result forwarding to a <see cref="MemoryAppender"/>:
    /// <code lang="XML" escaped="true">
    /// <appender name="WebAppender" type="DemiCode.Logging.log4net.Appenders.WebAppender, DemiCode.Logging.log4net" >
    ///     <url value="https://localhost/FogBugz/ScoutSubmit.asp" />
    ///     <method value="GET" />
    ///     <parameter>
    /// 	    <parameterName value="ScoutUsername" />
    /// 		<layout type="log4net.Layout.PatternLayout" value="scoutuser" />
    /// 	</parameter>
    ///     <parameter>
    ///         <parameterName value="ScoutProject" />
    /// 		<layout type="log4net.Layout.PatternLayout" value="Test Project" />
    ///     </parameter>
    ///     <parameter>
    ///     <parameterName value="ScoutArea" />
    ///         <layout type="log4net.Layout.PatternLayout" value="Misc" />
    ///     </parameter>
    ///     <parameter>
    /// 	    <parameterName value="Description" />
    /// 	    <layout type="log4net.Layout.ExceptionLayout" />
    ///     </parameter>
    ///     <parameter>
    /// 	    <parameterName value="Extra" />
    ///         <layout type="log4net.Layout.ExceptionLayout" />
    ///     </parameter>
    ///     <parameter>
    ///         <parameterName value="ForceNewBug" />
    /// 		<!-- 1 to force FogBUGZ to create a new bug for this entry, 0 to append to bug with matching description -->
    /// 	    <layout type="log4net.Layout.PatternLayout" value="0" />
    ///     </parameter>
    /// 	<parameter>
    /// 	    <parameterName value="ScoutDefaultMessage" />
    /// 		<!-- The default message to return to the user -->
    ///         <layout type="log4net.Layout.PatternLayout" value="Thanks for your submission" />
    /// 	</parameter>
    /// 	<parameter>
    /// 	    <parameterName value="FriendlyResponse" />
    /// 	    <!-- 1 to respond in HTML, 0 as XML -->
    /// 		<layout type="log4net.Layout.PatternLayout" value="0" />
    /// 	</parameter>
    /// 
    ///     <!-- The result from posting to our url will be forwarded to the specified appender(s) -->
    /// 	<appender-ref ref="WebResultAppender" />
    /// </appender>    
    /// 
    /// <appender name="WebResultAppender" type="log4net.Appender.MemoryAppender, log4net" >
	///     <fix>Partial</fix>
    /// </appender>
    /// </code>
    /// </example>
    public sealed class WebAppender : ForwardingAppender
    {
        private readonly List<WebAppenderParameter> _parameters = new List<WebAppenderParameter>(4);

        ///<summary>
        /// The web resource to post log events to.
        ///</summary>
        public Uri Url { get; set; }

        /// <summary>
        /// The method to use when posting.
        /// </summary>
        /// <value>Defaults to POST. Valid values are POST and GET</value>
        public string Method { get; set; }

        ///<summary>
        ///            Tests if this appender requires a <see cref="P:log4net.Appender.AppenderSkeleton.Layout" /> to be set.
        ///</summary>
        ///<remarks>
        ///<para>
        ///            In the rather exceptional case, where the appender 
        ///            implementation admits a layout but can also work without it, 
        ///            then the appender should return 
        ///<c>true</c>.
        ///</para>
        ///
        ///<para>
        ///            <see cref="WebAppender"/> does not require a layout.
        ///</para>
        ///
        ///</remarks>
        ///<returns>
        ///<c>false</c>.
        ///</returns>
        protected override bool RequiresLayout { get { return false; } }

        ///<summary>
        /// Construct a new <see cref="WebAppender"/>.
        ///</summary>
        public WebAppender()
        {
            if (ConverterRegistry.GetConvertFrom(typeof(Uri)) == null)
            {
                ConverterRegistry.AddConverter(typeof(Uri), typeof(UriConverter));
            }
        }

        ///<summary>
        /// Checks that the <see cref="Url"/> is set.
        ///</summary>
        ///<remarks>
        ///<para>
        ///            This method is called by <see cref="M:log4net.Appender.AppenderSkeleton.DoAppend(log4net.Core.LoggingEvent)" />
        ///            before the call to the abstract <see cref="M:log4net.Appender.AppenderSkeleton.Append(log4net.Core.LoggingEvent)" /> method.
        ///</para>
        ///</remarks>
        ///<returns>
        ///<c>true</c> if the call to <see cref="M:log4net.Appender.AppenderSkeleton.Append(log4net.Core.LoggingEvent)" /> should proceed.
        ///</returns>
        protected override bool PreAppendCheck()
        {
            if (Url == null)
            {
                ErrorHandler.Error("WebAppenderSkeleton: No url set for the appender named [" + Name + "].");
                return false;
            }

            if (String.IsNullOrEmpty(Method))
                Method = "POST";
            else if (Method != "POST" && Method != "GET")
            {
                ErrorHandler.Error("WebAppenderSkeleton: Method set the appender named [" + Name + "] must be either POST or GET.");
                return false;
            }

            return base.PreAppendCheck();
        }

        /// <summary>
        ///             Subclasses of <see cref="T:log4net.Appender.AppenderSkeleton" /> should implement this method 
        ///             to perform actual logging.
        /// </summary>
        /// <param name="loggingEvent">The event to append.</param>
        /// <remarks>
        /// <para>
        ///             A subclass must implement this method to perform
        ///             logging of the <paramref name="loggingEvent" />.
        /// </para>
        /// <para>
        /// This method will be called by <see cref="M:log4net.Appender.AppenderSkeleton.DoAppend(log4net.Core.LoggingEvent)" />
        ///             if all the conditions listed for that method are met.
        /// </para>
        /// <para>
        ///             To restrict the logging of events in the appender
        ///             override the <see cref="M:log4net.Appender.AppenderSkeleton.PreAppendCheck" /> method.
        /// </para>
        /// </remarks>
        protected override void Append(LoggingEvent loggingEvent)
        {
            byte[] response;
            using (var client = new WebClient())
            {
                switch (Method)
                {
                    case "POST":
                        response = UploadUsingPOST(client, loggingEvent);
                        break;
                    case "GET":
                        response = UploadUsingGET(client, loggingEvent);
                        break;
                    default:
                        throw new InvalidOperationException("Unsupported method " + Method);
                }
            }

            // ...and parse request, assume UTF-8 encoding
            var responseText = Encoding.UTF8.GetString(response);

            // Fix Properties collection, otherwise result will be lost when forward-appending 
            loggingEvent.Fix = FixFlags.Properties;
            loggingEvent.Properties["webappender:Result"] = responseText;
            base.Append(loggingEvent);
        }

        private byte[] UploadUsingGET(WebClient client, LoggingEvent loggingEvent)
        {
            foreach (var param in _parameters)
            {
                var paramValue = param.FormatValue(loggingEvent);

                if (paramValue != null)
                    client.QueryString.Add(param.ParameterName, HttpUtility.UrlEncode(paramValue));
            }

            // GET data from url ...
            return client.DownloadData(Url);
        }

        private byte[] UploadUsingPOST(WebClient client, LoggingEvent loggingEvent)
        {
            var data = new NameValueCollection(_parameters.Count);
            foreach (var param in _parameters)
            {
                var paramValue = param.FormatValue(loggingEvent);

                if (paramValue != null)
                    data.Add(param.ParameterName, paramValue);
            }

            // POST data to url ...
            return client.UploadValues(Url, data);
        }

        #region Helper methods
        // ParseResult: Deciphers the xml result returned by the scout page and throws an exception in the case of a failure notice.
        private string ParseResult(string result)
        {
            // Check for a success result first and just return in that case
            Match ma = Regex.Match(result, "<Success>(?<message>.*)</Success>", RegexOptions.IgnoreCase);
            if (ma.Success) return ma.Groups["message"].Value;

            // Check for a failure result second, and throw an exception in that case
            ma = Regex.Match(result, "<Error>(?<message>.*)</Error>", RegexOptions.IgnoreCase);
            if (ma.Success) throw new ApplicationException(ma.Groups["message"].Value);

            // Unknown format, so throw an InvalidOperationException to note the fact
            throw new InvalidOperationException("Unable to process result from submitting bug report");
        }

        #endregion

        #region Public Instance Methods

        /// <summary>
        /// Adds a parameter to the command.
        /// </summary>
        /// <param name="parameter">The parameter to add to the command.</param>
        /// <remarks>
        /// <para>
        /// Adds a parameter to the ordered list of command parameters.
        /// </para>
        /// </remarks>
        public void AddParameter(WebAppenderParameter parameter)
        {
            _parameters.Add(parameter);
        }


        #endregion // Public Instance Methods

        /// <summary>
        /// Parameter type used by the <see cref="WebAppender"/>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This class provides the basic formatting of values into querystring parameters.
        /// </para>
        /// </remarks>
        public class WebAppenderParameter
        {
            #region Public Instance Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="WebAppenderParameter" /> class.
            /// </summary>
            /// <remarks>
            /// Default constructor for the WebAppenderParameter class.
            /// </remarks>
            public WebAppenderParameter()
            {
            }

            #endregion // Public Instance Constructors

            #region Public Instance Properties

            /// <summary>
            /// Gets or sets the name of this parameter.
            /// </summary>
            /// <value>
            /// The name of this parameter.
            /// </value>
            /// <remarks>
            /// <para>
            /// The name of this parameter. The parameter name
            /// must match up to a named parameter to the SQL stored procedure
            /// or prepared statement.
            /// </para>
            /// </remarks>
            public string ParameterName { get; set; }


            /// <summary>
            /// Gets or sets the <see cref="IRawLayout"/> to use to 
            /// render the logging event into an object for this 
            /// parameter.
            /// </summary>
            /// <value>
            /// The <see cref="IRawLayout"/> used to render the
            /// logging event into an object for this parameter.
            /// </value>
            /// <remarks>
            /// <para>
            /// The <see cref="IRawLayout"/> that renders the value for this
            /// parameter.
            /// </para>
            /// <para>
            /// The <see cref="RawLayoutConverter"/> can be used to adapt
            /// any <see cref="ILayout"/> into a <see cref="IRawLayout"/>
            /// for use in the property.
            /// </para>
            /// </remarks>
            public IRawLayout Layout { get; set; }

            #endregion // Public Instance Properties

            #region Public Instance Methods

            public string FormatValue(LoggingEvent loggingEvent)
            {
                if (Layout == null)
                    return null;

                // Format the value
                var formattedValue = Layout.Format(loggingEvent);

                // If the value is null then convert to a DBNull
                if (formattedValue == null)
                {
                    return null;
                }

                return (string)formattedValue;
            }

            #endregion // Public Instance Methods

            #region Private Instance Fields

            #endregion // Private Instance Fields
        }

    }
}

#endif