using System.Collections.Generic;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Generator.ConfigurationHandling;

namespace GeoGen.Generator.Constructing.Arguments
{
    internal interface IArgumentsGenerator
    {
        IEnumerable<IReadOnlyList<ConstructionArgument>> GenerateArguments(ConfigurationWrapper configuration, ConstructionWrapper construction);
    }
}