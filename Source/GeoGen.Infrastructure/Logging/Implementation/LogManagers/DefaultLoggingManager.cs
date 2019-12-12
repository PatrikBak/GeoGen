namespace GeoGen.Infrastructure
{
    /// <summary>
    /// Represents an <see cref="ILoggingManager"/> that logs only to console.
    /// </summary>
    public class DefaultLoggingManager : CustomLoggingManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultLoggingManager/"/> class.
        /// </summary>
        public DefaultLoggingManager()
            // Pass a simple console logger
            : base(new[]
            {
                new ConsoleLogger(new ConsoleLoggerSettings
                                  (
                                      logOutputLevel: LogOutputLevel.Info,
                                      includeLoggingOrigin: false,
                                      includeTime: false
                                  ))
            })
        {
        }
    }
}