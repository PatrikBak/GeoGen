using System.Collections.Generic;
using GeoGen.Analyzer.Objects;
using GeoGen.Core.Configurations;
using GeoGen.Core.Theorems;
using GeoGen.Core.Utilities;

namespace GeoGen.Analyzer.Theorems
{
    internal interface ITheoremFinder
    {
        IEnumerable<Theorem> Find(ConfigurationObjectsMap oldObjects, ConfigurationObjectsMap newObjects);
    }
}