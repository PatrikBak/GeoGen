using GeoGen.Core.Configurations;
using GeoGen.Generator.ConfigurationsHandling.ConfigurationObjectToString.ConfigurationObjectIdResolving;

namespace GeoGen.Generator.ConfigurationsHandling.ConfigurationObjectToString
{
    /// <summary>
    /// Represents a configuration object to string converter.
    /// </summary>
    internal interface IObjectToStringProvider
    {
        /// <summary>
        /// Gets the object to string resolver that is used by this provider.
        /// </summary>
        IObjectIdResolver Resolver { get; }

        /// <summary>
        /// Converts a given configuration object to string. 
        /// </summary>
        /// <param name="configurationObject">The configuration object.</param>
        /// <returns>The string representation of the object.</returns>
        string ConvertToString(ConfigurationObject configurationObject);
    }
}