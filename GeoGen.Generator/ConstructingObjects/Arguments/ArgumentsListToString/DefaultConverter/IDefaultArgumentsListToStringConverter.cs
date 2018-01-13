using System.Collections.Generic;
using GeoGen.Core;
using GeoGen.Utilities;

namespace GeoGen.Generator
{
    /// <summary>
    /// Represents a converter of arguments list to string that uses a short
    /// type of signature. In this type, the objects are simply converted
    /// to string by converting its id to string. 
    /// </summary>
    public interface IDefaultArgumentsListToStringConverter : IToStringConverter<IReadOnlyList<ConstructionArgument>>
    {
    }
}