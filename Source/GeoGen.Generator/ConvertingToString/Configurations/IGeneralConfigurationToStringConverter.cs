using GeoGen.Core;
using GeoGen.Utilities;

namespace GeoGen.Generator
{
    /// <summary>
    /// Represents a converter of <see cref="GeneratedConfiguration"/> to a string that uses a custom
    /// <see cref="IToStringConverter{T}"/>, where 'T' is <see cref="ConfigurationObject"/>.
    /// </summary>
    public interface IGeneralConfigurationToStringConverter
    {
        /// <summary>
        /// Converts a given configuration to a string, using a given configuration object to string converter.
        /// </summary>
        /// <param name="configuration">The configuration to be converted.</param>
        /// <param name="objectToString">The configuration object to string converter.</param>
        /// <returns>A string representation of the configuration.</returns>
        string ConvertToString(GeneratedConfiguration configuration, IToStringConverter<ConfigurationObject> objectToString);
    }
}