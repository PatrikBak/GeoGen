using GeoGen.Core;

namespace GeoGen.ConfigurationGenerator
{
    /// <summary>
    /// Represents a type of a <see cref="GeoGenException"/> that is thrown when something incorrect happens 
    /// in the configuration generator module.
    /// </summary>
    public class ConfigurationGeneratorException : GeoGenException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationGeneratorException"/> class.
        /// </summary>
        public ConfigurationGeneratorException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationGeneratorException"/> class
        /// with a custom message about what happened.
        /// </summary>
        /// <param name="message">The message about what happened.</param>
        public ConfigurationGeneratorException(string message)
                : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationGeneratorException"/> class
        /// with a custom message about what happened, and the inner exception that caused this one.
        /// </summary>
        /// <param name="message">The message about what happened.</param>
        /// <param name="innerException">The inner exception that caused this one.</param>
        public ConfigurationGeneratorException(string message, Exception innerException)
                : base(message, innerException)
        {
        }
    }
}