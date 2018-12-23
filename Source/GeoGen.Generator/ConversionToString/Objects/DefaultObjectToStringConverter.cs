using GeoGen.Core;
using GeoGen.Utilities;

namespace GeoGen.Generator
{
    /// <summary>
    /// Represents a simple <see cref="IToStringConverter{T}"/> for <see cref="ConfigurationObject"/>s 
    /// that simply converts an object to the string representation of its id.
    /// </summary>
    public class DefaultObjectToStringConverter : IToStringConverter<ConfigurationObject>
    {
        /// <summary>
        /// Converts a given configuration object to string.
        /// </summary>
        /// <param name="configurationObject">The configuration object.</param>
        /// <returns>The string representation.</returns>
        public string ConvertToString(ConfigurationObject configurationObject)
        {
            return configurationObject.Id.ToString();
        }
    }
}