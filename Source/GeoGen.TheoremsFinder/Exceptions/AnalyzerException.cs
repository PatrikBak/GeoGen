using GeoGen.Core;
using System;

namespace GeoGen.TheoremsFinder
{
    /// <summary>
    /// Represents a type of a <see cref="GeoGenException"/> that is thrown
    /// when something incorrect happens in the analyzer module.
    /// </summary>
    public class AnalyzerException : GeoGenException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnalyzerException"/> class.
        /// </summary>
        public AnalyzerException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnalyzerException"/> class
        /// with a custom message about what happened.
        /// </summary>
        /// <param name="message">The message about what happened.</param>
        public AnalyzerException(string message)
                : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeoGenException"/> class
        /// with a custom message about what happened, and the inner exception that caused this one.
        /// </summary>
        /// <param name="message">The message about what happened.</param>
        /// <param name="innerException">The inner exception that caused this one.</param>
        public AnalyzerException(string message, Exception innerException)
                : base(message, innerException)
        {
        }
    }
}