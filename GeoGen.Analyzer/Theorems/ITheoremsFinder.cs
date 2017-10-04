using System.Collections.Generic;
using GeoGen.Core.Theorems;
using GeoGen.Core.Utilities;

namespace GeoGen.Analyzer.Theorems
{
    internal interface ITheoremsFinder
    {
        IEnumerable<Theorem> Find(ConfigurationObjectsMap oldObjects, ConfigurationObjectsMap newObjects);
    }
}