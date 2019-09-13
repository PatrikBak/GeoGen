using GeoGen.Core;
using System;

namespace GeoGen.TheoremProver
{
    /// <summary>
    /// Represents a type of a <see cref="GeoGenException"/> that is thrown
    /// when something incorrect happens in the theorem prover module.
    /// </summary>
    public class TheoremProverException : GeoGenException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TheoremProverException"/> class.
        /// </summary>
        public TheoremProverException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TheoremProverException"/> class
        /// with a custom message about what happened.
        /// </summary>
        /// <param name="message">The message about what happened.</param>
        public TheoremProverException(string message)
                : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TheoremProverException"/> class
        /// with a custom message about what happened, and the inner exception that caused this one.
        /// </summary>
        /// <param name="message">The message about what happened.</param>
        /// <param name="innerException">The inner exception that caused this one.</param>
        public TheoremProverException(string message, Exception innerException)
                : base(message, innerException)
        {
        }
    }
}