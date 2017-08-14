using System.Collections.Generic;
using GeoGen.Core.Constructions.Arguments;

namespace GeoGen.Generator.Constructor.Arguments.Container
{
    internal interface IArgumentsContainer : IEnumerable<IReadOnlyList<ConstructionArgument>>
    {
        void Add(IReadOnlyList<ConstructionArgument> arguments);

        void Clear();
    }
}