using GeoGen.Core.Configurations;

namespace GeoGen.Generator.ConfigurationHandling.ConfigurationObjectToString
{
    /// <summary>
    /// Represents a configuration object to string converter.
    /// </summary>
    internal interface IObjectToStringProvider
    {
        /// <summary>
        /// Gets the id of the provider.
        /// </summary>
        int Id { get; }

        /// <summary>
        /// Converts a given configuration object to string. 
        /// </summary>
        /// <param name="configurationObject">The configuration object.</param>
        /// <returns>The string representation of the object.</returns>
        string ConvertToString(ConfigurationObject configurationObject);
    }
}