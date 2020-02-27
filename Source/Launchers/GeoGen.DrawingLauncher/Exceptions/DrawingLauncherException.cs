using System;

namespace GeoGen.DrawingLauncher
{
    /// <summary>
    /// Represents an exception that is thrown when something incorrect happens in the drawer.
    /// </summary>
    public class DrawingLauncherException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DrawingLauncherException"/> class.
        /// </summary>
        public DrawingLauncherException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DrawingLauncherException"/> class
        /// with a custom message about what happened.
        /// </summary>
        /// <param name="message">The message about what happened.</param>
        public DrawingLauncherException(string message)
                : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DrawingLauncherException"/> class
        /// with a custom message about what happened, and the inner exception that caused this one.
        /// </summary>
        /// <param name="message">The message about what happened.</param>
        /// <param name="innerException">The inner exception that caused this one.</param>
        public DrawingLauncherException(string message, Exception innerException)
                : base(message, innerException)
        {
        }
    }
}
