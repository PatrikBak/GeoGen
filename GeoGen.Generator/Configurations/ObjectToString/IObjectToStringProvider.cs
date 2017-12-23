using GeoGen.Core.Configurations;
using GeoGen.Generator.ConstructingConfigurations.ObjectToString.ObjectIdResolving;

namespace GeoGen.Generator.ConstructingConfigurations.ObjectToString
{
    /// <summary>
    /// Represents a configuration object to string converter. It is 
    /// dependent on a <see cref="IObjectIdResolver"/>.
    /// </summary>
    internal interface IObjectToStringProvider
    {
        /// <summary>
        /// Converts a given configuration object to string. 
        /// </summary>
        /// <param name="configurationObject">The configuration object.</param>
        /// <returns>The string representation of the object.</returns>
        string ConvertToString(ConfigurationObject configurationObject);
    }
}