using System.Collections.Generic;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Utilities;

namespace GeoGen.Generator
{
    public interface IDefaultArgumentsListToStringConverter : IToStringConverter<IReadOnlyList<ConstructionArgument>>
    {
    }
}