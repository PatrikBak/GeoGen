using GeoGen.Core;

namespace GeoGen.TheoremFinder
{
    /// <summary>
    /// Represents a type of a <see cref="GeoGenException"/> that is thrown
    /// when something incorrect happens in the theorem finder module.
    /// </summary>
    public class TheoremFinderException : GeoGenException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TheoremFinderException"/> class.
        /// </summary>
        public TheoremFinderException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TheoremFinderException"/> class
        /// with a custom message about what happened.
        /// </summary>
        /// <inheritdoc/>
        public TheoremFinderException(string message)
                : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TheoremFinderException"/> class
        /// with a custom message about what happened, and the inner exception that caused this one.
        /// </summary>
        /// <inheritdoc/>
        public TheoremFinderException(string message, Exception innerException)
                : base(message, innerException)
        {
        }
    }
}