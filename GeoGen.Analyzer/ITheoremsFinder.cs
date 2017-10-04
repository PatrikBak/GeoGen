using System.Collections.Generic;
using GeoGen.Core.Theorems;
using GeoGen.Core.Utilities;

namespace GeoGen.Analyzer
{
    internal interface ITheoremsFinder
    {
        IEnumerable<Theorem> Find(ConfigurationObjectsMap oldObjects, ConfigurationObjectsMap newObjects);
    }
}