using System.Collections.Generic;
using GeoGen.Core;

namespace GeoGen.Analyzer
{
    internal interface ITheoremConstructor
    {
        Theorem Construct(List<GeometricalObject> involvedObjects, ConfigurationObjectsMap allObjects, TheoremType type);
    }
}