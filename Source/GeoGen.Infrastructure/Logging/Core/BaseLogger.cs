using System;
using System.IO;

namespace GeoGen.Infrastructure
{
    /// <summary>
    /// A base class implementing the <see cref="ILogger"/> interface.
    /// </summary>
    /// <typeparam name="TSettings">The type of the settings used for this logger.</typeparam>
    public abstract class BaseLogger<TSettings> : ILogger where TSettings : BaseLoggerSettings
    {
        #region ILogger properties

        /// <summary>
        /// Gets the output level for the logger.
        /// </summary>
        public LogOutputLevel LogOutputLevel => _settings.LogOutputLevel;

        #endregion

        #region Protected fields

        /// <summary>
        /// The settings for the logger.
        /// </summary>
        protected readonly TSettings _settings;

        #endregion

        #region Protected constructor

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="settings">The settings for the logger.</param>
        protected BaseLogger(TSettings settings)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
        }

        #endregion

        #region Protected helper methods

        /// <summary>
        /// A helper function that composes the final message, according to the settings.
        /// </summary>
        /// <param name="message">The message to log</param>
        /// <param name="level">The level of the message being logged</param>
        /// <param name="time">The time when the logging happened.</param>
        /// <param name="origin">The method/function this message was logged in</param>
        /// <param name="filePath">The code filename that this message was logged from</param>
        /// <param name="lineNumber">The line of code in the filename this message was logged from</param>
        protected string ComposeFinalMessage(string message, LogLevel level, DateTimeOffset time, string origin, string filePath, int lineNumber)
        {
            // Prepare the variable holding the result
            var finalMessage = default(string);

            // If we request the time
            if (_settings.IncludeTime)
                // Start with it
                finalMessage = $"[{time.ToString("yyyy-MM-dd hh:mm:ss")}]";

            // Append the level (trim in the case that we didn't include the time -- then the string starts with the space)
            finalMessage = $"{finalMessage} [{ level}]".Trim();

            // Append the message
            finalMessage = $"{finalMessage} {message}";

            // If we request the origin...
            if (_settings.IncludeLoggingOrigin)
                // Prepend it
                finalMessage = $"[{Path.GetFileName(filePath)} > {origin}() > Line {lineNumber}] {finalMessage}";

            // Return the composed message
            return finalMessage;
        }

        #endregion

        #region ILogger abstract implementation

        /// <summary>
        /// Handles the logged message being passed in.
        /// </summary>
        /// <param name="message">The message to log</param>
        /// <param name="level">The level of the message being logged</param>
        /// <param name="time">The time when the logging happened.</param>
        /// <param name="origin">The method/function this message was logged in</param>
        /// <param name="filePath">The code filename that this message was logged from</param>
        /// <param name="lineNumber">The line of code in the filename this message was logged from</param>
        public abstract void Log(string message, LogLevel level, DateTimeOffset time, string origin, string filePath, int lineNumber);

        #endregion
    }
}