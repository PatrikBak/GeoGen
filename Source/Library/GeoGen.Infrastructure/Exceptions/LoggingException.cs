namespace GeoGen.Infrastructure
{
    /// <summary>
    /// Represents an exception thrown when there are issues with setting up logging.
    /// </summary>
    public class LoggingException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingException"/> class.
        /// </summary>
        public LoggingException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingException"/> class
        /// with a custom message about what happened.
        /// </summary>
        /// <inheritdoc/>
        public LoggingException(string message)
                : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoggingException"/> class
        /// with a custom message about what happened, and the inner exception that caused this one.
        /// </summary>
        /// <inheritdoc/>
        public LoggingException(string message, Exception innerException)
                : base(message, innerException)
        {
        }
    }
}