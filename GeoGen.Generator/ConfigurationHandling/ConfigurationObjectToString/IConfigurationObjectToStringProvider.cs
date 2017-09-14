using GeoGen.Core.Configurations;

namespace GeoGen.Generator.ConfigurationHandling.ConfigurationObjectToString
{
    /// <summary>
    /// Represents a cofiguration object to string converter. It is supposed to cache 
    /// string versions of already pocessed objects. Therefore it won't convert an object
    /// until it's all internal objects have already been converted and cached.
    /// </summary>
    internal interface IConfigurationObjectToStringProvider
    {
        /// <summary>
        /// Converts a given configuration object to string. 
        /// </summary>
        /// <param name="configurationObject">The configuration object.</param>
        /// <returns>The string representation of the object..</returns>
        string ConvertToString(ConfigurationObject configurationObject);
    }
}
