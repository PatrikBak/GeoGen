using GeoGen.Core;
using GeoGen.Utilities;

namespace GeoGen.Generator
{
    /// <summary>
    /// Represents a converter of arguments to string that uses a short
    /// type of signature. In this type, the objects are simply converted
    /// to string by converting its id to string. 
    /// </summary>
    public interface IDefaultArgumentsToStringConverter : IToStringConverter<Arguments>
    {
    }
}