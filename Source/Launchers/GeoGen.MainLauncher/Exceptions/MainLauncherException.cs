using System;

namespace GeoGen.MainLauncher
{
    /// <summary>
    /// Represents an exception that is thrown when something incorrect happens in the main launcher.
    /// </summary>
    public class MainLauncherException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainLauncherException"/> class.
        /// </summary>
        public MainLauncherException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MainLauncherException"/> class
        /// with a custom message about what happened.
        /// </summary>
        /// <inheritdoc/>
        public MainLauncherException(string message)
                : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MainLauncherException"/> class
        /// with a custom message about what happened, and the inner exception that caused this one.
        /// </summary>
        /// <inheritdoc/>
        public MainLauncherException(string message, Exception innerException)
                : base(message, innerException)
        {
        }
    }
}
