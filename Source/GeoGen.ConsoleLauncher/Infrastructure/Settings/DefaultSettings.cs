using System.Collections.Generic;

namespace GeoGen.ConsoleLauncher
{
    /// <summary>
    /// A default settings for the application.
    /// </summary>
    public class DefaultSettings : Settings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultSettings"/> class.
        /// </summary>
        public DefaultSettings()
        {
            // Create the loggers
            Loggers = new List<BaseLoggerSettings>
            {
                // Console logger
                new ConsoleLoggerSettings
                {
                    IncludeLoggingOrigin = false,
                    IncludeTime = true,
                    LogOutputLevel = LogOutputLevel.Info
                },

                // File logger
                new FileLoggerSettings
                {
                    IncludeLoggingOrigin = true,
                    IncludeTime = false,
                    LogOutputLevel = LogOutputLevel.Debug,
                    FileLogPath = "log.txt"
                }
            };
        }
    }
}
