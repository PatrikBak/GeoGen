using System.Collections.Generic;
using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions;
using GeoGen.Core.Constructions.Arguments;

namespace GeoGen.Generator.Constructor
{
    internal interface IArgumentsGenerator
    {
        IEnumerable<IReadOnlyList<ConstructionArgument>> GenerateArguments(Configuration configuration, Construction constructionId);
    }
}