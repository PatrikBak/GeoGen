using System;

namespace GeoGen.Infrastructure
{
    /// <summary>
    /// Represents an exception thrown when there are issues with setting up <see cref="JsonConfiguration"/>.
    /// </summary>
    public class ConfigurationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationException"/> class.
        /// </summary>
        public ConfigurationException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationException"/> class
        /// with a custom message about what happened.
        /// </summary>
        /// <inheritdoc/>
        public ConfigurationException(string message)
                : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationException"/> class
        /// with a custom message about what happened, and the inner exception that caused this one.
        /// </summary>
        /// <inheritdoc/>
        public ConfigurationException(string message, Exception innerException)
                : base(message, innerException)
        {
        }
    }
}