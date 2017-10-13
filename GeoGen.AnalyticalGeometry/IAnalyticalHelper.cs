using System.Collections.Generic;
using GeoGen.AnalyticalGeometry.Objects;

namespace GeoGen.AnalyticalGeometry
{
    public interface IAnalyticalHelper
    {
        List<Point> Intersect(IEnumerable<AnalyticalObject> inputObjects);

        bool LiesOn(AnalyticalObject analyticalObject, Point point);
    }
}