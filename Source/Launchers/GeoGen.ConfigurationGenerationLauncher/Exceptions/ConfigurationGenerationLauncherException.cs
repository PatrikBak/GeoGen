using System;

namespace GeoGen.ConfigurationGenerationLauncher
{
    /// <summary>
    /// Represents an exception that is thrown when something incorrect happens in the configuration generation launcher.
    /// </summary>
    public class ConfigurationGenerationLauncherException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationGenerationLauncherException"/> class.
        /// </summary>
        public ConfigurationGenerationLauncherException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationGenerationLauncherException"/> class
        /// with a custom message about what happened.
        /// </summary>
        /// <inheritdoc/>
        public ConfigurationGenerationLauncherException(string message)
                : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationGenerationLauncherException"/> class
        /// with a custom message about what happened, and the inner exception that caused this one.
        /// </summary>
        /// <inheritdoc/>
        public ConfigurationGenerationLauncherException(string message, Exception innerException)
                : base(message, innerException)
        {
        }
    }
}
