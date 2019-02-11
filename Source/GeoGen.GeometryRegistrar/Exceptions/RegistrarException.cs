using GeoGen.Core;
using System;

namespace GeoGen.GeometryRegistrar
{
    /// <summary>
    /// Represents a type of a <see cref="GeoGenException"/> that is thrown
    /// when something incorrect happens in the registrar module.
    /// </summary>
    public class RegistrarException : GeoGenException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RegistrarException"/> class.
        /// </summary>
        public RegistrarException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RegistrarException"/> class
        /// with a custom message about what happened.
        /// </summary>
        /// <param name="message">The message about what happened.</param>
        public RegistrarException(string message)
                : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="GeoGenException"/> class
        /// with a custom message about what happened, and the inner exception that caused this one.
        /// </summary>
        /// <param name="message">The message about what happened.</param>
        /// <param name="innerException">The inner exception that caused this one.</param>
        public RegistrarException(string message, Exception innerException)
                : base(message, innerException)
        {
        }
    }
}