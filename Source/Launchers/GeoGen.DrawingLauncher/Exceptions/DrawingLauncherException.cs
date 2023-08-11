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
        /// <inheritdoc/>
        public DrawingLauncherException(string message)
                : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DrawingLauncherException"/> class
        /// with a custom message about what happened, and the inner exception that caused this one.
        /// </summary>
        /// <inheritdoc/>
        public DrawingLauncherException(string message, Exception innerException)
                : base(message, innerException)
        {
        }
    }
}
