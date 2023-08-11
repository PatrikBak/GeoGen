namespace GeoGen.Core
{
    /// <summary>
    /// Represents an exception that occurred while parsing GeoGen objects from strings.
    /// </summary>
    public class ParsingException : GeoGenException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ParsingException"/> class.
        /// </summary>
        public ParsingException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParsingException"/> class
        /// with a custom message about what happened.
        /// </summary>
        /// <inheritdoc/>
        public ParsingException(string message)
                : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParsingException"/> class
        /// with a custom message about what happened, and the inner exception that caused this one.
        /// </summary>
        /// <inheritdoc/>
        public ParsingException(string message, Exception innerException)
                : base(message, innerException)
        {
        }
    }
}
