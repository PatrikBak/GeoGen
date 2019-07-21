using GeoGen.Utilities;
using System;
using System.Linq;

namespace GeoGen.ConsoleLauncher
{
    /// <summary>
    /// A default <see cref="ILoggingManager"/> that gets its loggers injected through the constructor.
    /// </summary>
    public class DefaultLoggingManager : ILoggingManager
    {
        #region Private members

        /// <summary>
        /// The loggers to which we are going.
        /// </summary>
        private readonly ILogger[] _loggers;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes an instance of the <see cref="DefaultLoggingManager"/> class with the provided loggers.
        /// </summary>
        /// <param name="loggers">The loggers to which we are going.</param>
        public DefaultLoggingManager(ILogger[] loggers)
        {
            _loggers = loggers ?? throw new ArgumentNullException(nameof(loggers));
        }

        #endregion

        #region ILoggingManager implementation

        /// <summary>
        /// Logs the specific debug message to all loggers.
        /// </summary>
        /// <param name="message">The message to log.</param>
        /// <param name="level">The level of the message being logged.</param>
        /// <param name="origin">The method/function this message was logged in.</param>
        /// <param name="filePath">The code filename that this message was logged from.</param>
        /// <param name="lineNumber">The line of code in the filename this message was logged from.</param>
        public void Log(string message, LogLevel level, string origin = "", string filePath = "", int lineNumber = 0)
        {
            // Lock the call so we won't log two messages at the same time
            lock (this)
            {
                // Pick the time when the logging happened
                var time = DateTimeOffset.Now;

                // Take the loggers with the right logging level
                _loggers.Where(logger => (int) logger.LogOutputLevel <= (int) level)
                    // And log the message to them
                    .ForEach(_loggers => _loggers.Log(message, level, time, origin, filePath, lineNumber));
            }
        }

        #endregion
    }
}
