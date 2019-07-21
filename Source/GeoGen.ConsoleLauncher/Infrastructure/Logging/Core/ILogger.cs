using System;

namespace GeoGen.ConsoleLauncher
{
    /// <summary>
    /// A logger that will handle log messages from a <see cref="ILoggingManager"/>
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Gets the output level for the logger.
        /// </summary>
        LogOutputLevel LogOutputLevel { get; }

        /// <summary>
        /// Handles the logged message being passed in.
        /// </summary>
        /// <param name="message">The message to log</param>
        /// <param name="level">The level of the message being logged</param>
        /// <param name="time">The time when the logging happened.</param>
        /// <param name="origin">The method/function this message was logged in</param>
        /// <param name="filePath">The code filename that this message was logged from</param>
        /// <param name="lineNumber">The line of code in the filename this message was logged from</param>
        void Log(string message, LogLevel level, DateTimeOffset time, string origin, string filePath, int lineNumber);
    }
}