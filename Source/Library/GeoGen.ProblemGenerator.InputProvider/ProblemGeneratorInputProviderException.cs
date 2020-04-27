using System;

namespace GeoGen.ProblemGenerator.InputProvider
{
    /// <summary>
    /// Represents an exception that is thrown when something incorrect happens in the problem generator input provider module.
    /// </summary>
    public class ProblemGeneratorInputProviderException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProblemGeneratorInputProviderException"/> class.
        /// </summary>
        public ProblemGeneratorInputProviderException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProblemGeneratorInputProviderException"/> class
        /// with a custom message about what happened.
        /// </summary>
        /// <inheritdoc/>
        public ProblemGeneratorInputProviderException(string message)
                : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProblemGeneratorInputProviderException"/> class
        /// with a custom message about what happened, and the inner exception that caused this one.
        /// </summary>
        /// <inheritdoc/>
        public ProblemGeneratorInputProviderException(string message, Exception innerException)
                : base(message, innerException)
        {
        }
    }
}
