using System;

namespace GeoGen.TheoremSimplifier
{
    /// <summary>
    /// Represents an exception that is thrown when something incorrect happens in the simplifier module.
    /// </summary>
    public class TheoremSimplifierException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TheoremSimplifierException"/> class.
        /// </summary>
        public TheoremSimplifierException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TheoremSimplifierException"/> class
        /// with a custom message about what happened.
        /// </summary>
        /// <inheritdoc/>
        public TheoremSimplifierException(string message)
                : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TheoremSimplifierException"/> class
        /// with a custom message about what happened, and the inner exception that caused this one.
        /// </summary>
        /// <inheritdoc/>
        public TheoremSimplifierException(string message, Exception innerException)
                : base(message, innerException)
        {
        }
    }
}
