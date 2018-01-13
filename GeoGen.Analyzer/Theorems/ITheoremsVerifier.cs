using System.Collections.Generic;
using GeoGen.Core;

namespace GeoGen.Analyzer
{
    internal interface ITheoremsVerifier
    {
        IEnumerable<Theorem> FindTheorems(IReadOnlyList<ConfigurationObject> oldObjects, IReadOnlyList<ConstructedConfigurationObject> newObjects);
    }
}