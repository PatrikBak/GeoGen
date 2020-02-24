namespace GeoGen.Infrastructure
{
    /// <summary>
    /// The base class for settings of implementations of <see cref="ILogger"/>.
    /// </summary>
    public abstract class BaseLoggerSettings
    {
        #region Public properties

        /// <summary>
        /// The level of the messages being logged.
        /// </summary>
        public LogOutputLevel LogOutputLevel { get; }

        /// <summary>
        /// Indicates if we should include logging origin details such as file path, line number and the method that the message was logged in.
        /// </summary>
        public bool IncludeLoggingOrigin { get; }

        /// <summary>
        /// Indicates, if we should include the time when the message being logged was produced.
        /// </summary>
        public bool IncludeTime { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseLoggerSettings"/> class.
        /// </summary>
        /// <param name="logOutputLevel">The level of the messages being logged.</param>
        /// <param name="includeLoggingOrigin">Indicates if we should include logging origin details such as file path, line number and the method that the message was logged in.</param>
        /// <param name="includeTime">Indicates, if we should include the time when the message being logged was produced.</param>
        protected BaseLoggerSettings(LogOutputLevel logOutputLevel, bool includeLoggingOrigin, bool includeTime)
        {
            LogOutputLevel = logOutputLevel;
            IncludeLoggingOrigin = includeLoggingOrigin;
            IncludeTime = includeTime;
        }

        #endregion
    }
}
