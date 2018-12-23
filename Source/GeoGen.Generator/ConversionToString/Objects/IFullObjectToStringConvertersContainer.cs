using GeoGen.Core;
using GeoGen.Utilities;
using System.Collections.Generic;

namespace GeoGen.Generator
{
    public interface IFullObjectToStringConvertersContainer : IEnumerable<IToStringConverter<ConfigurationObject>>
    {
        IFullObjectToStringConverter DefaultFullConverter { get; }
    }
}
