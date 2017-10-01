using GeoGen.Analyzer.Geometry;

namespace GeoGen.Analyzer
{
    internal interface IRandomObjectsProvider
    {
        GeometricalObject NextRandomObject<T>() where T : GeometricalObject;
    }
}