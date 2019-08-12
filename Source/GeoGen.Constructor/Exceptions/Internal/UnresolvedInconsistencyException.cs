using System;

namespace GeoGen.Constructor
{
    /// <summary>
    /// Represents an <see cref="ConstructorException"/> that is thrown when there are inconsistencies
    /// between <see cref="Picture"/>s of a <see cref="Pictures"/> that
    /// couldn't be automatically resolved. For more details about inconsistencies see the documentation
    /// of <see cref="InconsistentPicturesException"/>. This exception should be handled internally.
    /// </summary>
    internal class UnresolvedInconsistencyException : ConstructorException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnresolvedInconsistencyException"/> class.
        /// </summary>
        public UnresolvedInconsistencyException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnresolvedInconsistencyException"/> class
        /// with a custom message about what happened.
        /// </summary>
        /// <param name="message">The message about what happened.</param>
        public UnresolvedInconsistencyException(string message)
                : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnresolvedInconsistencyException"/> class
        /// with a custom message about what happened, and the inner exception that caused this one.
        /// </summary>
        /// <param name="message">The message about what happened.</param>
        /// <param name="innerException">The inner exception that caused this one.</param>
        public UnresolvedInconsistencyException(string message, Exception innerException)
                : base(message, innerException)
        {
        }
    }
}
