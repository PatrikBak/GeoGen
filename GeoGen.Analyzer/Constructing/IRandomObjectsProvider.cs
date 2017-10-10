using GeoGen.AnalyticalGeometry;
using GeoGen.Analyzer.Objects;

namespace GeoGen.Analyzer.Constructing
{
    internal interface IRandomObjectsProvider
    {
        AnalyticalObject NextRandomObject<T>() where T : AnalyticalObject;
    }
}