using System;

namespace GeoGen.Constructor
{
    /// <summary>
    /// An exception thrown when a reconstruction of a <see cref="ContextualPicture"/> fails.
    /// </summary>
    public class InconstructibleContextualPicture : ConstructorException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InconstructibleContextualPicture"/> class.
        /// </summary>
        public InconstructibleContextualPicture()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InconstructibleContextualPicture"/> class
        /// with a custom message about what happened.
        /// </summary>
        /// <param name="message">The message about what happened.</param>
        public InconstructibleContextualPicture(string message)
                : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InconstructibleContextualPicture"/> class
        /// with a custom message about what happened, and the inner exception that caused this one.
        /// </summary>
        /// <param name="message">The message about what happened.</param>
        /// <param name="innerException">The inner exception that caused this one.</param>
        public InconstructibleContextualPicture(string message, Exception innerException)
                : base(message, innerException)
        {
        }
    }
}
