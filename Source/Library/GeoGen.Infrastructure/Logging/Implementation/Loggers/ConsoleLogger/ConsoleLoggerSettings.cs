namespace GeoGen.Infrastructure
{
    /// <summary>
    /// Settings of the <see cref="ConsoleLogger"/>.
    /// </summary>
    public class ConsoleLoggerSettings : BaseLoggerSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConsoleLoggerSettings"/> class.
        /// </summary>
        /// <param name="logOutputLevel">The level of the messages being logged.</param>
        /// <param name="includeLoggingOrigin">Indicates if we should include logging origin details such as file path, line number and the method that the message was logged in.</param>
        /// <param name="includeTime">Indicates, if we should include the time when the message being logged was produced.</param>
        public ConsoleLoggerSettings(LogOutputLevel logOutputLevel, bool includeLoggingOrigin, bool includeTime)
            : base(logOutputLevel, includeLoggingOrigin, includeTime)
        {
        }
    }
}
