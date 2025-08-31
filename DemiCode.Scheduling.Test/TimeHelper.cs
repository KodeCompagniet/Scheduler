using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace DemiCode.Scheduling.Test
{
    public static class TimeHelper
    {
        /// <summary>
        /// Indicates whether system time manipulation is supported on the current platform
        /// </summary>
        public static bool IsTimeManipulationSupported => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        [DllImport("kernel32.dll")]
        private extern static void GetSystemTime(ref SYSTEMTIME lpSystemTime);

        [DllImport("kernel32.dll")]
        private extern static uint SetSystemTime(ref SYSTEMTIME lpSystemTime);


        private struct SYSTEMTIME
        {
            public ushort wYear;
            public ushort wMonth;
            public ushort wDayOfWeek;
            public ushort wDay;
            public ushort wHour;
            public ushort wMinute;
            public ushort wSecond;
            public ushort wMilliseconds;

            public DateTime ToDateTime()
            {
                return new DateTime(wYear, wMonth, wDay, wHour, wMinute, wSecond, wMilliseconds, DateTimeKind.Utc);
            }

            public static SYSTEMTIME FromDateTime(DateTime date)
            {
                if (date.Kind != DateTimeKind.Utc)
                    date = date.ToUniversalTime();

                SYSTEMTIME st = new SYSTEMTIME();
                st.wYear = (ushort)date.Year;
                st.wMonth = (ushort)date.Month;
                st.wDay = (ushort)date.Day;
                st.wDayOfWeek = (ushort)date.DayOfWeek;
                st.wHour = (ushort)date.Hour;
                st.wMinute = (ushort)date.Minute;
                st.wSecond = (ushort)date.Second;
                st.wMilliseconds = (ushort)date.Millisecond;
                return st;
            }
        }

        public static DateTime GetTime()
        {
            if (!IsTimeManipulationSupported)
                throw new PlatformNotSupportedException("System time manipulation is only supported on Windows");

            // Call the native GetSystemTime method
            // with the defined structure.
            SYSTEMTIME stime = new SYSTEMTIME();
            GetSystemTime(ref stime);

            return stime.ToDateTime();
        }

        public static void SetTime(DateTime date)
        {
            if (!IsTimeManipulationSupported)
                throw new PlatformNotSupportedException("System time manipulation is only supported on Windows");

            // Call the native SetSystemTime method
            // with the defined structure.
            SYSTEMTIME systime = SYSTEMTIME.FromDateTime(date);
            SetSystemTime(ref systime);
        }


        /// <summary>
        /// Change system time to the specified date/time and restore system time upon disposing.
        /// </summary>
        public static IDisposable Change(DateTime toDate)
        {
            if (!IsTimeManipulationSupported)
                throw new PlatformNotSupportedException("System time manipulation is only supported on Windows");

            DateTime original = GetTime();
            Stopwatch sw = Stopwatch.StartNew();
            Cargo c = new Cargo(original, sw);
            SetTime(toDate);
            return c;
        }

        private class Cargo : IDisposable
        {
            private bool _disposed;
            private DateTime _originalDate;
            private Stopwatch _stopwatch;

            public Cargo(DateTime originalDate, Stopwatch stopwatch)
            {
                _originalDate = originalDate;
                _stopwatch = stopwatch;
            }

            public void RestoreTime()
            {
                if (!_stopwatch.IsRunning)
                    throw new InvalidOperationException("System time was already restored");

                _stopwatch.Stop();
                SetTime(_originalDate.Add(_stopwatch.Elapsed));
            }

            ~Cargo()
            {
                Dispose(false);
            }

            ///<summary>
            ///Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
            ///</summary>
            ///<filterpriority>2</filterpriority>
            public void Dispose()
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }

            private void Dispose(bool disposing)
            {
                if (!_disposed)
                {
                    // If disposing equals true, dispose all managed 
                    // and unmanaged resources.
                    if (disposing)
                    {
                        // Dispose managed resources.
                        RestoreTime();
                    }

                    // Release unmanaged resources. If disposing is false, 
                    // only the following code is executed.
                }

                _disposed = true;
            }
        }
    }
}
