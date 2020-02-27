using GeoGen.Core;
using System;

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
        /// <param name="message">The message about what happened.</param>
        public TheoremFinderException(string message)
                : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TheoremFinderException"/> class
        /// with a custom message about what happened, and the inner exception that caused this one.
        /// </summary>
        /// <param name="message">The message about what happened.</param>
        /// <param name="innerException">The inner exception that caused this one.</param>
        public TheoremFinderException(string message, Exception innerException)
                : base(message, innerException)
        {
        }
    }
}