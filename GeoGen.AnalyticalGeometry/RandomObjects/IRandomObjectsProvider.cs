using GeoGen.AnalyticalGeometry.Objects;

namespace GeoGen.AnalyticalGeometry.RandomObjects
{
    public interface IRandomObjectsProvider
    {
        AnalyticalObject NextRandomObject<T>() where T : AnalyticalObject;
    }
}