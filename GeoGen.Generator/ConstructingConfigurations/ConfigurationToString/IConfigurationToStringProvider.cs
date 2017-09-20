using GeoGen.Core.Configurations;
using GeoGen.Generator.ConstructingConfigurations.ObjectToString;

namespace GeoGen.Generator.ConstructingConfigurations.ConfigurationToString
{
    /// <summary>
    /// Represents a converter of configurations to string. It uses
    /// configuration object to string provider, which is injection
    /// via a method injection, because of the thread safety.
    /// </summary>
    internal interface IConfigurationToStringProvider
    {
        /// <summary>
        /// Converts a givenn configuration to string, using a given 
        /// configuration object to string provider.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="objectToString">The configuration object to string provider.</param>
        /// <returns>The string representation of the configuration.</returns>
        string ConvertToString(Configuration configuration, IObjectToStringProvider objectToString);
    }
}