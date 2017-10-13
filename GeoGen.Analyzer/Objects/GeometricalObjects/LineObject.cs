using System.Collections.Generic;
using GeoGen.Core.Configurations;

namespace GeoGen.Analyzer.Objects.GeometricalObjects
{
    public sealed class LineObject : GeometricalObject
    {
        public HashSet<PointObject> Points { get; set; }

        public LineObject(ConfigurationObject configurationObject, int id)
            : base(configurationObject, id)
        {
            Points = new HashSet<PointObject>();
        }

        public LineObject(int id, params PointObject[] points)
            : base(id)
        {
            Points = new HashSet<PointObject>(points);
        }
    }
}