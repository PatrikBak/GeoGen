using System;
using System.Collections.Generic;

namespace GeoGen.Infrastructure
{
    /// <summary>
    /// The settings for the logging system.
    /// </summary>
    public class LoggingSettings
    {
        #region Public properties

        /// <summary>
        /// The list of the loggers settings.
        /// </summary>
        public IReadOnlyList<BaseLoggerSettings> Loggers { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingSettings"/> class.
        /// </summary>
        /// <param name="loggerSettings">The settings for particular loggers.</param>
        public LoggingSettings(IReadOnlyList<BaseLoggerSettings> loggerSettings)
        {
            Loggers = loggerSettings ?? throw new ArgumentNullException(nameof(loggerSettings));
        }

        #endregion
    }
}
