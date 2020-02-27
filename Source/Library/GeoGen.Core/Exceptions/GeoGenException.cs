using System;

namespace GeoGen.Core
{
    /// <summary>
    /// Represents the base type of exception for the GeoGen algorithms.
    /// </summary>
    public class GeoGenException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GeoGenException"/> class.
        /// </summary>
        public GeoGenException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeoGenException"/> class
        /// with a custom message about what happened.
        /// </summary>
        /// <param name="message">The message about what happened.</param>
        public GeoGenException(string message)
                : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeoGenException"/> class
        /// with a custom message about what happened, and the inner exception that caused this one.
        /// </summary>
        /// <param name="message">The message about what happened.</param>
        /// <param name="innerException">The inner exception that caused this one.</param>
        public GeoGenException(string message, Exception innerException)
                : base(message, innerException)
        {
        }
    }
}
