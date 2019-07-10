using System;

namespace GeoGen.Constructor
{
    /// <summary>
    /// Represents an <see cref="ConstructorException"/> that is thrown when there are inconsistencies
    /// between <see cref="IObjectsContainer"/>s of a <see cref="IObjectsContainersManager"/> that
    /// couldn't be automatically resolved. For more details about inconsistencies see the documentation
    /// of <see cref="InconsistentContainersException"/>. This exception should be handled internally.
    /// </summary>
    public class UnresolvableInconsistencyException : ConstructorException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnresolvableInconsistencyException"/> class.
        /// </summary>
        public UnresolvableInconsistencyException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnresolvableInconsistencyException"/> class
        /// with a custom message about what happened.
        /// </summary>
        /// <param name="message">The message about what happened.</param>
        public UnresolvableInconsistencyException(string message)
                : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnresolvableInconsistencyException"/> class
        /// with a custom message about what happened, and the inner exception that caused this one.
        /// </summary>
        /// <param name="message">The message about what happened.</param>
        /// <param name="innerException">The inner exception that caused this one.</param>
        public UnresolvableInconsistencyException(string message, Exception innerException)
                : base(message, innerException)
        {
        }
    }
}
