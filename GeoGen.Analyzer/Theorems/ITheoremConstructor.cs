using System.Collections.Generic;
using GeoGen.Core.Theorems;
using GeoGen.Core.Utilities;

namespace GeoGen.Analyzer
{
    internal interface ITheoremConstructor
    {
        Theorem Construct(List<GeometricalObject> involvedObjects, ConfigurationObjectsMap allObjects, TheoremType type);
    }
}