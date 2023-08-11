using GeoGen.Core;

namespace GeoGen.TheoremRanker
{
    /// <summary>
    /// Represents a type of <see cref="GeoGenException"/> that is thrown
    /// when something incorrect happens in the theorem ranker module.
    /// </summary>
    public class TheoremRankerException : GeoGenException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TheoremRankerException"/> class.
        /// </summary>
        public TheoremRankerException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TheoremRankerException"/> class
        /// with a custom message about what happened.
        /// </summary>
        /// <inheritdoc/>
        public TheoremRankerException(string message)
                : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TheoremRankerException"/> class
        /// with a custom message about what happened, and the inner exception that caused this one.
        /// </summary>
        /// <inheritdoc/>
        public TheoremRankerException(string message, Exception innerException)
                : base(message, innerException)
        {
        }
    }
}