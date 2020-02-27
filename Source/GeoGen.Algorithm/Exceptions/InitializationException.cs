using GeoGen.Core;
using System;

namespace GeoGen.Algorithm
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
        /// <param name="message">The message about what happened.</param>
        public InitializationException(string message)
                : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InitializationException"/> class
        /// with a custom message about what happened, and the inner exception that caused this one.
        /// </summary>
        /// <param name="message">The message about what happened.</param>
        /// <param name="innerException">The inner exception that caused this one.</param>
        public InitializationException(string message, Exception innerException)
                : base(message, innerException)
        {
        }
    }
}