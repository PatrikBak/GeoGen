using System.Collections.Generic;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Generator.Wrappers;

namespace GeoGen.Generator.Constructor.Arguments
{
    internal interface IArgumentsGenerator
    {
        IEnumerable<IReadOnlyList<ConstructionArgument>> GenerateArguments(ConfigurationWrapper configuration, ConstructionWrapper construction);
    }
}