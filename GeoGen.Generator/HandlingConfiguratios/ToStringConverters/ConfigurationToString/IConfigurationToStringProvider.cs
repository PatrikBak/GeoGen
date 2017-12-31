namespace GeoGen.Generator
{
    /// <summary>
    /// Represents a converter of configuration to string. It uses
    /// configuration object to string provider, which is injected
    /// via a method injection, because of the thread safety.
    /// </summary>
    internal interface IConfigurationToStringProvider
    {
        /// <summary>
        /// Converts a given configuration to string, using a given 
        /// configuration object to string provider.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="objectToString">The configuration object to string provider.</param>
        /// <returns>The string representation of the configuration.</returns>
        string ConvertToString(ConfigurationWrapper configuration, IObjectToStringConverter objectToString);
    }
}