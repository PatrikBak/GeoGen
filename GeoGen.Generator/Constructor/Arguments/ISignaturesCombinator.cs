using System.Collections.Generic;
using GeoGen.Core.Configurations;
using GeoGen.Generator.Wrappers;

namespace GeoGen.Generator.Constructor.Arguments
{
    internal interface ISignaturesCombinator
    {
        IEnumerable<IReadOnlyDictionary<ConfigurationObjectType, IEnumerable<ConfigurationObject>>> Combine(
            ConfigurationWrapper configuration, ConstructionWrapper construction);
    }
}
