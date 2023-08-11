namespace GeoGen.TheoremRanker.RankedTheoremIO
{
    /// <summary>
    /// Represents an exception that is thrown when something incorrect happens in the ranked theorem IO module.
    /// </summary>
    public class RankedTheoremIOException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RankedTheoremIOException"/> class.
        /// </summary>
        public RankedTheoremIOException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RankedTheoremIOException"/> class
        /// with a custom message about what happened.
        /// </summary>
        /// <inheritdoc/>
        public RankedTheoremIOException(string message)
                : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RankedTheoremIOException"/> class
        /// with a custom message about what happened, and the inner exception that caused this one.
        /// </summary>
        /// <inheritdoc/>
        public RankedTheoremIOException(string message, Exception innerException)
                : base(message, innerException)
        {
        }
    }
}
