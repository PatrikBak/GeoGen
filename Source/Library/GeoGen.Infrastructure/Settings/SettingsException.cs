using System;

namespace GeoGen.Infrastructure
{
    /// <summary>
    /// Represents the base type of exception thrown when there is a problem with settings.
    /// </summary>
    public class SettingsException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsException"/> class.
        /// </summary>
        public SettingsException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsException"/> class
        /// with a custom message about what happened.
        /// </summary>
        /// <param name="message">The message about what happened.</param>
        public SettingsException(string message)
                : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsException"/> class
        /// with a custom message about what happened, and the inner exception that caused this one.
        /// </summary>
        /// <param name="message">The message about what happened.</param>
        /// <param name="innerException">The inner exception that caused this one.</param>
        public SettingsException(string message, Exception innerException)
                : base(message, innerException)
        {
        }
    }
}
