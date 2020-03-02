using System;
using System.IO;

namespace GeoGen.Infrastructure
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

        #region BaseLogger implementation

        /// <inheritdoc/>
        public override void Log(string message, LogLevel level, DateTimeOffset time, string origin, string filePath, int lineNumber)
        {
            // Compose the final message
            var finalMessage = ComposeFinalMessage(message, level, time, origin, filePath, lineNumber);

            // Open the stream writer for the file
            using var streamWriter = new StreamWriter(_settings.FileLogPath, append: true);

            // Write the message to the file
            streamWriter.WriteLine(finalMessage);
        }

        #endregion
    }
}
