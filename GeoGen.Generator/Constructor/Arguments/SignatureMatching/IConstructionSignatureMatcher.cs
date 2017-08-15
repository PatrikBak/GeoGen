using System.Collections.Generic;
using GeoGen.Core.Configurations;
using GeoGen.Core.Constructions.Arguments;
using GeoGen.Core.Constructions.Parameters;

namespace GeoGen.Generator.Constructor.Arguments.SignatureMatching
{
    internal interface IConstructionSignatureMatcher
    {
        void Initialize(IReadOnlyDictionary<ConfigurationObjectType, IEnumerator<ConfigurationObject>> objectTypeToObjects);

        IReadOnlyList<ConstructionArgument> Match(IReadOnlyList<ConstructionParameter> parameters);
    }
}