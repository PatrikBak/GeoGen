namespace GeoGen.ConsoleLauncher
{
    /// <summary>
    /// The base class for settings of <see cref="ILogger"/>.
    /// </summary>
    public abstract class BaseLoggerSettings
    {
        /// <summary>
        /// The level of the messages being logged.
        /// </summary>
        public LogOutputLevel LogOutputLevel { get; set; }

        /// <summary>
        /// Indicates, if we should include logging origin details such as file path, line number and the method that the message was logged in.
        /// </summary>
        public bool IncludeLoggingOrigin { get; set; }

        /// <summary>
        /// Indicates, if we should include the time when the message being logged was produced.
        /// </summary>
        public bool IncludeTime { get; set; }
    }
}
