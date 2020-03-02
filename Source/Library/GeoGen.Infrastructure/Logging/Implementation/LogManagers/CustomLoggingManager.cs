using GeoGen.Utilities;
using System;
using System.Linq;

namespace GeoGen.Infrastructure
{
    /// <summary>
    /// A <see cref="ILoggingManager"/> that logs to particular <see cref="ILogger"/>s.
    /// </summary>
    public class CustomLoggingManager : ILoggingManager
    {
        #region Private members

        /// <summary>
        /// The loggers to which we are going to log.
        /// </summary>
        private readonly ILogger[] _loggers;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes an instance of the <see cref="CustomLoggingManager"/> class with the provided loggers.
        /// </summary>
        /// <param name="loggers">The loggers to which we are going to log</param>
        public CustomLoggingManager(ILogger[] loggers)
        {
            _loggers = loggers ?? throw new ArgumentNullException(nameof(loggers));
        }

        #endregion

        #region ILoggingManager implementation

        /// <inheritdoc/>
        public void Log(string message, LogLevel level, string origin = "", string filePath = "", int lineNumber = 0)
        {
            // Lock the call so we won't log two messages at the same time
            lock (this)
            {
                // Pick the time when the logging happened
                var time = DateTimeOffset.Now;

                // Take the loggers with the right logging level
                _loggers.Where(logger => (int)logger.LogOutputLevel <= (int)level)
                    // And log the message to them
                    .ForEach(_loggers => _loggers.Log(message, level, time, origin, filePath, lineNumber));
            }
        }

        #endregion
    }
}
