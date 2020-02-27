using System;

namespace GeoGen.Infrastructure
{
    /// <summary>
    /// Settings of the <see cref="FileLogger"/>
    /// </summary>
    public class FileLoggerSettings : BaseLoggerSettings
    {
        #region Public properties

        /// <summary>
        /// The path to the file to which we log to.
        /// </summary>
        public string FileLogPath { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="FileLoggerSettings"/> class.
        /// </summary>
        /// <param name="fileLogPath">The path to the file to which we log to.</param>
        /// <param name="logOutputLevel">The level of the messages being logged.</param>
        /// <param name="includeLoggingOrigin">Indicates if we should include logging origin details such as file path, line number and the method that the message was logged in.</param>
        /// <param name="includeTime">Indicates, if we should include the time when the message being logged was produced.</param>

        public FileLoggerSettings(string fileLogPath, LogOutputLevel logOutputLevel, bool includeLoggingOrigin, bool includeTime)
            : base(logOutputLevel, includeLoggingOrigin, includeTime)
        {
            FileLogPath = fileLogPath ?? throw new ArgumentNullException(nameof(fileLogPath));
        }

        #endregion
    }
}
