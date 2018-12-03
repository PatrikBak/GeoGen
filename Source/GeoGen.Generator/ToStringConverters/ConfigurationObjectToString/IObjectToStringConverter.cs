using GeoGen.Core;
using GeoGen.Utilities;

namespace GeoGen.Generator
{
    /// <summary>
    /// Represents a converter of <see cref="ConfigurationObject"/>s to string
    /// using their ids.
    /// </summary>
    internal interface IObjectToStringConverter : IToStringConverter<ConfigurationObject>
    {
        /// <summary>
        /// Gets the object id resolver associated with this converter.
        /// </summary>
        IObjectIdResolver Resolver { get; }
    }
}