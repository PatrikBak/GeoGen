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
        /// Initializes a new instance of the <see cref="GeoGenException"/> class.
        /// </summary>
        /// <param name="message">The message containing further information about the fault.</param>
        public GeoGenException(string message)
            :  base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeoGenException"/> class.
        /// </summary>
        /// <param name="message">The message containing further information about the fault.</param>
        /// <param name="innerException">The inner exception that caused this fault.</param>
        public GeoGenException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
