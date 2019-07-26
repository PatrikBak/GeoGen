using GeoGen.Core;
using System;

namespace GeoGen.ConsoleLauncher
{
    /// <summary>
    /// Represents an exception that occurred while parsing GeoGen objects from strings.
    /// </summary>
    public class ParserException : GeoGenException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ParserException"/> class.
        /// </summary>
        public ParserException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParserException"/> class
        /// with a custom message about what happened.
        /// </summary>
        /// <param name="message">The message about what happened.</param>
        public ParserException(string message)
                : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParserException"/> class
        /// with a custom message about what happened, and the inner exception that caused this one.
        /// </summary>
        /// <param name="message">The message about what happened.</param>
        /// <param name="innerException">The inner exception that caused this one.</param>
        public ParserException(string message, Exception innerException)
                : base(message, innerException)
        {
        }
    }
}
