using System.Collections.Generic;
using GeoGen.Core.Configurations;
using GeoGen.Core.Theorems;

namespace GeoGen.Analyzer.Theorems
{
    internal interface ITheoremsVerifier
    {
        IEnumerable<Theorem> FindTheorems(List<ConfigurationObject> oldObjects, List<ConstructedConfigurationObject> newObjects);
    }
}