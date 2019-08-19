using GeoGen.Core;
using GeoGen.Utilities;

namespace GeoGen.Generator
{
    /// <summary>
    /// Represents a converter of <see cref="Arguments"/> to a string that uses a custom 
    /// <see cref="IToStringConverter{T}"/>, where 'T' is <see cref="ConfigurationObject"/>,
    /// to convert the internal arguments' objects.
    /// </summary>
    public interface IGeneralArgumentsToStringConverter
    {
        /// <summary>
        /// Converts given arguments to a string, using a given configuration object to string converter.
        /// </summary>
        /// <param name="arguments">The arguments to be converted.</param>
        /// <param name="objectToString">The configuration object to string converter.</param>
        /// <returns>A string representation of the arguments.</returns>
        string ConvertToString(Arguments arguments, IToStringConverter<ConfigurationObject> objectToString);
    }
}