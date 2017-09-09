using GeoGen.Core.Configurations;

namespace GeoGen.Generator.ConfigurationHandling.ConfigurationObjectToString
{
    /// <summary>
    /// Represents a cofiguration object to string converter.
    /// </summary>
    public interface IConfgurationObjectToStringProvider
    {
        /// <summary>
        /// Converts a given configuration object to string. 
        /// </summary>
        /// <param name="configurationObject">The configuration object.</param>
        /// <returns>The string representation of the list.</returns>
        string ConvertToString(ConfigurationObject configurationObject);
    }
}
