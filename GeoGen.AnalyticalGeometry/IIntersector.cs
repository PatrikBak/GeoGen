using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoGen.AnalyticalGeometry
{
    public interface IIntersector
    {
        List<Point> Intersect(HashSet<AnalyticalObject> inputObjects);

        bool LiesOn(AnalyticalObject analyticalObject, Point point);

        Line CreateLine(Point p1, Point p2);
        Circle CreateCircle(Point p1, Point p2, Point p3);
    }
}