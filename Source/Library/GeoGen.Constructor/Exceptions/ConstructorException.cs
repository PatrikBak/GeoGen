using GeoGen.Core;

namespace GeoGen.Constructor
{
    /// <summary>
    /// Represents a type of a <see cref="GeoGenException"/> that is thrown
    /// when something incorrect happens in the constructor module.
    /// </summary>
    public class ConstructorException : GeoGenException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConstructorException"/> class.
        /// </summary>
        public ConstructorException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstructorException"/> class
        /// with a custom message about what happened.
        /// </summary>
        /// <inheritdoc/>
        public ConstructorException(string message)
                : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConstructorException"/> class
        /// with a custom message about what happened, and the inner exception that caused this one.
        /// </summary>
        /// <inheritdoc/>
        public ConstructorException(string message, Exception innerException)
                : base(message, innerException)
        {
        }
    }
}