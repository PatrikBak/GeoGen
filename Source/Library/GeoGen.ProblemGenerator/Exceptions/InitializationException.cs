using GeoGen.Core;

namespace GeoGen.ProblemGenerator
{
    /// <summary>
    /// Represents a type of a <see cref="GeoGenException"/> that is thrown when 
    /// <see cref="IProblemGenerator"/> couldn't be performed because of an incorrect input.
    /// </summary>
    public class InitializationException : GeoGenException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InitializationException"/> class.
        /// </summary>
        public InitializationException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InitializationException"/> class
        /// with a custom message about what happened.
        /// </summary>
        /// <inheritdoc/>
        public InitializationException(string message)
                : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InitializationException"/> class
        /// with a custom message about what happened, and the inner exception that caused this one.
        /// </summary>
        /// <inheritdoc/>
        public InitializationException(string message, Exception innerException)
                : base(message, innerException)
        {
        }
    }
}