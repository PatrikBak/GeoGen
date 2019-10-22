namespace GeoGen.ConsoleLauncher
{
    /// <summary>
    /// A static class that gives access to the current <see cref="ILoggingManager"/> 
    /// for the whole application.
    /// </summary>
    public static class Log
    {
        #region Lock

        /// <summary>
        /// The lock to lock the getter and the setter of the logger.
        /// </summary>
        private static readonly object _lock = new object();

        #endregion

        #region Backing field

        /// <summary>
        /// The current manager, initialized with the default manager (so it's never null)
        /// </summary>
        private static ILoggingManager _manager = new DefaultLoggingManager();

        #endregion

        #region LoggerManager

        /// <summary>
        /// Gets or sets the logging manager for the application.
        /// </summary>
        public static ILoggingManager LoggingManager
        {
            get
            {
                // Lock the getter
                lock (_lock)
                {
                    // Return the manager
                    return _manager;
                }
            }
            set
            {
                // Lock the setter
                lock (_lock)
                {
                    // Set the value, if it's not all. Otherwise fall-back to the logging manager
                    _manager = value ?? new DefaultLoggingManager();
                }
            }
        }

        #endregion
    }
}
