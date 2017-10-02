using GeoGen.Analyzer.Objects;

namespace GeoGen.Analyzer.Constructing
{
    internal interface IRandomObjectsProvider
    {
        GeometricalObject NextRandomObject<T>() where T : GeometricalObject;
    }
}