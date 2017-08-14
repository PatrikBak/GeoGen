using System.Collections.Generic;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Core.Constructions.Parameters;

namespace GeoGen.Generator.Constructor.Arguments.SignatureMatching
{
    interface IConstructionSignatureMatcher
    {
        IReadOnlyList<ConstructionArgument> Match(IConfigurationObjectsIterator iterator, IReadOnlyList<ConstructionParameter> parameters);
    }
}