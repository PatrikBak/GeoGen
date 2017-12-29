using System.Collections.Generic;
using GeoGen.Core.Configurations;
using GeoGen.Core.Theorems;

namespace GeoGen.Analyzer
{
    internal interface ITheoremsVerifier
    {
        IEnumerable<Theorem> FindTheorems(IReadOnlyList<ConfigurationObject> oldObjects, IReadOnlyList<ConstructedConfigurationObject> newObjects);
    }
}