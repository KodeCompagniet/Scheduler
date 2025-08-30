using System;

namespace DemiCode.Logging
{
    /// <summary>
    /// A simple Nested Diagnostic Context implementation.
    /// </summary>
    internal class SimpleNDC : IDisposable
    {
        private string _message;
        public SimpleNDC(string message)
        {
            _message = message;
        }

        #region IDisposable pattern

        private bool _disposed = false;
        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Cleanup managed resources
                }
                _disposed = true;
            }
        }

        ///<summary>
        ///Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        ///</summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion
    }
}