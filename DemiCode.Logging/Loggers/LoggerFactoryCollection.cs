using System;
using System.Collections;
using System.Collections.Generic;

namespace DemiCode.Logging.Loggers
{
    /// <summary>
    /// A collection of logger factories.
    /// </summary>
    public class LoggerFactoryCollection : IList<ILoggerFactory>
    {
        private readonly List<ILoggerFactory> _loggerFactories = new List<ILoggerFactory>();
        private const bool isReadOnly = false;

        /// <summary>
        /// Get an <see cref="ILoggerFactory"/> instance for the default configured logger factory.
        /// </summary>
        public ILoggerFactory GetDefaultLoggerFactory()
        {
            if (Count == 0)
            {
                // No factories! Construct a "nil" logger.
                return new NilLoggerFactory();
            }

            //TODO: implement "default logger factory"
            return this[0];
        }

        int IList<ILoggerFactory>.IndexOf(ILoggerFactory loggerFactory)
        {
            return _loggerFactories.IndexOf(loggerFactory);
        }

        void IList<ILoggerFactory>.Insert(int index, ILoggerFactory loggerFactory)
        {
            _loggerFactories.Insert(index, loggerFactory);
        }

        void IList<ILoggerFactory>.RemoveAt(int index)
        {
            _loggerFactories.RemoveAt(index);
        }

        /// <summary>
        /// Return the task at the specified index.
        /// </summary>
        public ILoggerFactory this[int index]
        {
            get 
            {
                return _loggerFactories[index];
            }
            set
            {
                _loggerFactories[index] = value;
            }
        }

        void ICollection<ILoggerFactory>.Clear()
        {
            _loggerFactories.Clear();
        }

        bool ICollection<ILoggerFactory>.Contains(ILoggerFactory task)
        {
            return _loggerFactories.Contains(task);
        }

        void ICollection<ILoggerFactory>.CopyTo(ILoggerFactory[] array, int arrayIndex)
        {
            _loggerFactories.CopyTo(array, arrayIndex);
        }

        bool ICollection<ILoggerFactory>.Remove(ILoggerFactory item)
        {
            return _loggerFactories.Remove(item);
        }

        /// <summary>
        /// Number of logger factories in the collection.
        /// </summary>
        public int Count
        {
            get { return _loggerFactories.Count; }
        }

        bool ICollection<ILoggerFactory>.IsReadOnly
        {
            get { return isReadOnly; }
        }

        IEnumerator<ILoggerFactory> IEnumerable<ILoggerFactory>.GetEnumerator()
        {
            return _loggerFactories.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _loggerFactories.GetEnumerator();
        }

        /// <summary>
        /// Add a logger factory.
        /// </summary>
        /// <param name="loggerFactory">The logger factory</param>
        public void Add(ILoggerFactory loggerFactory)
        {
            if (loggerFactory == null)
                throw new ArgumentNullException(nameof(loggerFactory));

            _loggerFactories.Add(loggerFactory);
        }

    }
}
