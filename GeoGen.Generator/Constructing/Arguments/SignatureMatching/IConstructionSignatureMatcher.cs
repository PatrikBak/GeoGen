using System.Collections.Generic;
using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Core.Constructions.Parameters;

namespace GeoGen.Generator.Constructing.Arguments.SignatureMatching
{
    internal interface IConstructionSignatureMatcher
    {
        void Initialize(IReadOnlyDictionary<ConfigurationObjectType, List<ConfigurationObject>> objectTypeToObjects);

        IReadOnlyList<ConstructionArgument> Match(IReadOnlyList<ConstructionParameter> parameters);
    }
}