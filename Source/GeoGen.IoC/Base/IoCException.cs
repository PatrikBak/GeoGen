using System;

namespace GeoGen.IoC
{
    /// <summary>
    /// Represents the exception thrown where there is an error concerning IoC bindings.
    /// </summary>
    public class IoCException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IoCException"/> class.
        /// </summary>
        public IoCException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IoCException"/> class
        /// with a custom message about what happened.
        /// </summary>
        /// <param name="message">The message about what happened.</param>
        public IoCException(string message)
                : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IoCException"/> class
        /// with a custom message about what happened, and the inner exception that caused this one.
        /// </summary>
        /// <param name="message">The message about what happened.</param>
        /// <param name="innerException">The inner exception that caused this one.</param>
        public IoCException(string message, Exception innerException)
                : base(message, innerException)
        {
        }
    }
}
