using GeoGen.Core;
using System;

namespace GeoGen.TheoremsAnalyzer
{
    /// <summary>
    /// Represents a type of a <see cref="GeoGenException"/> that is thrown
    /// when something incorrect happens in the theorems analyzer module.
    /// </summary>
    public class TheoremsAnalyzerException : GeoGenException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TheoremsAnalyzerException"/> class.
        /// </summary>
        public TheoremsAnalyzerException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TheoremsAnalyzerException"/> class
        /// with a custom message about what happened.
        /// </summary>
        /// <param name="message">The message about what happened.</param>
        public TheoremsAnalyzerException(string message)
                : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TheoremsAnalyzerException"/> class
        /// with a custom message about what happened, and the inner exception that caused this one.
        /// </summary>
        /// <param name="message">The message about what happened.</param>
        /// <param name="innerException">The inner exception that caused this one.</param>
        public TheoremsAnalyzerException(string message, Exception innerException)
                : base(message, innerException)
        {
        }
    }
}