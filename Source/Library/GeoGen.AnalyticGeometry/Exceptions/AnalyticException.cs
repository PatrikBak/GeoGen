using System;

namespace GeoGen.AnalyticGeometry
{
    /// <summary>
    /// Represents an exception that is thrown when geometrically illogical things 
    /// happen in the module (for example, constructing a circle from three collinear points).
    /// </summary>
    public class AnalyticException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AnalyticException"/> class.
        /// </summary>
        public AnalyticException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnalyticException"/> class
        /// with a custom message about what happened.
        /// </summary>
        /// <param name="message">The message about what happened.</param>
        public AnalyticException(string message)
                : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AnalyticException"/> class
        /// with a custom message about what happened, and the inner exception that caused this one.
        /// </summary>
        /// <param name="message">The message about what happened.</param>
        /// <param name="innerException">The inner exception that caused this one.</param>
        public AnalyticException(string message, Exception innerException)
                : base(message, innerException)
        {
        }
    }
}