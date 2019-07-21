using GeoGen.Core;
using System;

namespace GeoGen.TheoremsFinder
{
    /// <summary>
    /// Represents a type of a <see cref="GeoGenException"/> that is thrown
    /// when something incorrect happens in the theorems finder module.
    /// </summary>
    public class TheoremsFinderException : GeoGenException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TheoremsFinderException"/> class.
        /// </summary>
        public TheoremsFinderException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TheoremsFinderException"/> class
        /// with a custom message about what happened.
        /// </summary>
        /// <param name="message">The message about what happened.</param>
        public TheoremsFinderException(string message)
                : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeoGenException"/> class
        /// with a custom message about what happened, and the inner exception that caused this one.
        /// </summary>
        /// <param name="message">The message about what happened.</param>
        /// <param name="innerException">The inner exception that caused this one.</param>
        public TheoremsFinderException(string message, Exception innerException)
                : base(message, innerException)
        {
        }
    }
}