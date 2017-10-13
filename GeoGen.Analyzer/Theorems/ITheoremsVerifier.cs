using System.Collections.Generic;
using GeoGen.Core.Theorems;
using GeoGen.Core.Utilities;

namespace GeoGen.Analyzer.Theorems
{
    internal interface ITheoremsVerifier
    {
        IEnumerable<Theorem> FindTheorems(ConfigurationObjectsMap oldObjects, ConfigurationObjectsMap newObjects);
    }
}