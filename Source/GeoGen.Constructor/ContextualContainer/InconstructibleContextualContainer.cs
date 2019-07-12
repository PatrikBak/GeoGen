using System;

namespace GeoGen.Constructor
{
    /// <summary>
    /// An exception thrown when a reconstruction of a <see cref="ContextualContainer"/> fails.
    /// </summary>
    public class InconstructibleContextualContainer : ConstructorException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InconstructibleContextualContainer"/> class.
        /// </summary>
        public InconstructibleContextualContainer()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InconstructibleContextualContainer"/> class
        /// with a custom message about what happened.
        /// </summary>
        /// <param name="message">The message about what happened.</param>
        public InconstructibleContextualContainer(string message)
                : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InconstructibleContextualContainer"/> class
        /// with a custom message about what happened, and the inner exception that caused this one.
        /// </summary>
        /// <param name="message">The message about what happened.</param>
        /// <param name="innerException">The inner exception that caused this one.</param>
        public InconstructibleContextualContainer(string message, Exception innerException)
                : base(message, innerException)
        {
        }
    }
}
