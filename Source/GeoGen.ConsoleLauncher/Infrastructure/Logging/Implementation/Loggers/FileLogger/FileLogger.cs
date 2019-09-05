using System;
using System.Diagnostics;
using System.IO;

namespace GeoGen.ConsoleLauncher
{
    /// <summary>
    /// Represents the <see cref="ILogger"/> that logs to a specific file.
    /// </summary>
    public class FileLogger : BaseLogger<FileLoggerSettings>
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="FileLogger"/> class.
        /// </summary>
        /// <param name="settings">The file logger settings.</param>
        public FileLogger(FileLoggerSettings settings)
            : base(settings)
        {
            // Erase the file first
            File.Delete(settings.FileLogPath);
        }

        #endregion

        #region BaseLogger Log method implementation

        /// <summary>
        /// Handles the logged message being passed in.
        /// </summary>
        /// <param name="message">The message to log</param>
        /// <param name="level">The level of the message being logged</param>
        /// <param name="time">The time when the logging happened.</param>
        /// <param name="origin">The method/function this message was logged in</param>
        /// <param name="filePath">The code filename that this message was logged from</param>
        /// <param name="lineNumber">The line of code in the filename this message was logged from</param>
        public override void Log(string message, LogLevel level, DateTimeOffset time, string origin, string filePath, int lineNumber)
        {
            // Compose the final message
            var finalMessage = ComposeFinalMessage(message, level, time, origin, filePath, lineNumber);

            try
            {
                // Open the stream writer for the file
                using var streamWriter = new StreamWriter(_settings.FileLogPath, append: true);

                // Write the message to the file
                streamWriter.WriteLine(finalMessage);
            }
            // If there is any exception
            catch (Exception e)
            {
                // Let the developer know
                Debug.WriteLine($"Unable to write the log message to the log file. The exception: {Environment.NewLine}{e.Message}");
            }
        }

        #endregion
    }
}
