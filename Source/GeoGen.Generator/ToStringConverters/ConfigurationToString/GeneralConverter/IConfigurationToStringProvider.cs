namespace GeoGen.Generator
{
    /// <summary>
    /// Represents a generic configuration to string converter that uses 
    /// a custom <see cref="IObjectToStringConverter"/> converter. 
    /// </summary>
    internal interface IConfigurationToStringProvider
    {
        /// <summary>
        /// Converts a given configuration to string, using a given 
        /// configuration object to string converter.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="objectToString">The configuration object to string converter.</param>
        /// <returns>The string representation of the configuration.</returns>
        string ConvertToString(ConfigurationWrapper configuration, IObjectToStringConverter objectToString);
    }
}