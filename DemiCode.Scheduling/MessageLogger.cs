using System;
using System.Diagnostics;
using System.Globalization;

namespace DemiCode.Scheduling
{
    [Obsolete("Should use DemiCode.Shared.Logging")]
	static internal class MessageLogger
	{
		#region static stuff
		static public void LogInfo(string format, params object[] p)
		{
			Log(ConsoleColor.White, "info", format, p);
		}

		static public void LogComment(string format, params object[] p)
		{
			Log(ConsoleColor.DarkGray, "comment", format, p);
		}

		static public void LogError(string format, params object[] p)
		{
			Log(ConsoleColor.Red, "error", format, p);
		}

		static public void LogWarning(string format, params object[] p)
		{
			Log(ConsoleColor.Yellow, "warning", format, p);
		}

		static private void Log(ConsoleColor color, string elementName, string format, params object[] p)
		{
            string s = String.Format(CultureInfo.InvariantCulture, format, p);
			//color makes no sence in Debug... 
            //TODO: refactor ConsoleColor into LogLevel
            //Debug.ForegroundColor = color;
            Debug.WriteLine(String.Format(CultureInfo.InvariantCulture, "{0}: {1}", elementName.ToUpper(CultureInfo.InvariantCulture), s));
		}
		#endregion
	}
}
