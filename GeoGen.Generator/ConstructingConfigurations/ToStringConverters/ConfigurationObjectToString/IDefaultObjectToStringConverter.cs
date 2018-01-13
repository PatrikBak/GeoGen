using GeoGen.Core;

namespace GeoGen.Generator
{
    /// <summary>
    /// Represents a converter of <see cref="ConfigurationObject"/>s to string
    /// that simply converts the objects id to string.
    /// </summary>
    internal interface IDefaultObjectToStringConverter : IObjectToStringConverter
    {
    }
}