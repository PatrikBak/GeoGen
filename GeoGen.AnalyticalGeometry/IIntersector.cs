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
    }
}