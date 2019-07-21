namespace GeoGen.ConsoleLauncher
{
    /// <summary>
    /// The severity of the log message.
    /// </summary>
    public enum LogLevel
    {
        /// <summary>
        /// A developer-specific message used to diagnose problems.
        /// </summary>
        Debug = 1,

        /// <summary>
        /// A message indicating that something expected and noteworthy happened. 
        /// </summary>
        Info = 2,

        /// <summary>
        /// A message indicating that something unexpected, but potentially interesting happened.
        /// </summary>
        Warning = 3,

        /// <summary>
        /// An error message that doesn't cause the fall of the application.
        /// </summary>
        Error = 4,

        /// <summary>
        /// A serious message from which there is no recovery.
        /// </summary>
        Fatal = 5
    }
}