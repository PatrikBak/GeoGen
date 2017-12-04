using GeoGen.Core.Theorems;
using System.Collections.Generic;
using GeoGen.Analyzer.Objects.GeometricalObjects;
using GeoGen.Core.Utilities;

namespace GeoGen.Analyzer.Theorems
{
    internal interface ITheoremConstructor
    {
        Theorem Construct(List<GeometricalObject> involvedObjects, ConfigurationObjectsMap allObjects, TheoremType type);
    }
}