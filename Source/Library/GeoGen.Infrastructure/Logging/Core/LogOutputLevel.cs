namespace GeoGen.Infrastructure
{
    /// <summary>
    /// The level of details to output for a logger.
    /// </summary>
    public enum LogOutputLevel
    {
        /// <summary>
        /// Logs everything.
        /// </summary>
        Debug = 1,

        /// <summary>
        /// Logs all messages except for debug ones.
        /// </summary>
        Info = 2,

        /// <summary>
        /// Log only warning, errors and fatal messages.
        /// </summary>
        Warning = 3,

        /// <summary>
        /// Log only error and fatal messages.
        /// </summary>
        Error = 4,

        /// <summary>
        /// Log only fatal messages.
        /// </summary>
        Fatal = 5
    }
}
