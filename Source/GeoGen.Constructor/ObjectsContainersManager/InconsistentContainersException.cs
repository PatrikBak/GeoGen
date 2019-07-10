using System;

namespace GeoGen.Constructor
{
    /// <summary>
    /// Represents an exception that is thrown when there is an inconsistency between 
    /// <see cref="IObjectsContainer"/>s. Two containers are inconsistent for example
    /// if there is an object that is constructible in one of them, and not in the
    /// other. The other cases of inconsistency might be: two objects are not duplicates
    /// in all containers, three points are not collinear in all containers, etc. 
    /// Inconsistencies are a pretty natural, though rare thing to happen (because we 
    /// are using a limited precision model) and they should by handled internally.
    /// </summary>
    public class InconsistentContainersException : ConstructorException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InconsistentContainersException"/> class.
        /// </summary>
        public InconsistentContainersException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InconsistentContainersException"/> class
        /// with a custom message about what happened.
        /// </summary>
        /// <param name="message">The message about what happened.</param>
        public InconsistentContainersException(string message)
                : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InconsistentContainersException"/> class
        /// with a custom message about what happened, and the inner exception that caused this one.
        /// </summary>
        /// <param name="message">The message about what happened.</param>
        /// <param name="innerException">The inner exception that caused this one.</param>
        public InconsistentContainersException(string message, Exception innerException)
                : base(message, innerException)
        {
        }
    }
}