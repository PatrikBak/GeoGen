using System;

namespace GeoGen.Constructor
{
    /// <summary>
    /// Represents an exception that is thrown when there is an inconsistency between 
    /// <see cref="IPicture"/>s. Two pictures are inconsistent for example
    /// if there is an object that is constructible in one of them, and not in the
    /// other. The other cases of inconsistency might be: two objects are not duplicates
    /// in all pictures, three points are not collinear in all pictures, etc. 
    /// Inconsistencies are a pretty natural, though rare thing to happen (because we 
    /// are using a limited precision model) and they should by handled internally.
    /// </summary>
    public class InconsistentPicturesException : ConstructorException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InconsistentPicturesException"/> class.
        /// </summary>
        public InconsistentPicturesException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InconsistentPicturesException"/> class
        /// with a custom message about what happened.
        /// </summary>
        /// <param name="message">The message about what happened.</param>
        public InconsistentPicturesException(string message)
                : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InconsistentPicturesException"/> class
        /// with a custom message about what happened, and the inner exception that caused this one.
        /// </summary>
        /// <param name="message">The message about what happened.</param>
        /// <param name="innerException">The inner exception that caused this one.</param>
        public InconsistentPicturesException(string message, Exception innerException)
                : base(message, innerException)
        {
        }
    }
}