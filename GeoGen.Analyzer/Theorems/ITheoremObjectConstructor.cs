using GeoGen.Analyzer.Objects.GeometricalObjects;
using GeoGen.Core.Theorems;
using GeoGen.Core.Utilities;

namespace GeoGen.Analyzer.Theorems
{
    internal interface ITheoremObjectConstructor
    {
        TheoremObject Construct(ConfigurationObjectsMap objects, GeometricalObject geometricalObject);
    }
}