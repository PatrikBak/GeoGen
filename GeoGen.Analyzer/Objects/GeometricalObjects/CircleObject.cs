using System.Collections.Generic;
using GeoGen.Core.Configurations;

namespace GeoGen.Analyzer.Objects.GeometricalObjects
{
    public sealed class CircleObject : GeometricalObject
    {
        public HashSet<PointObject> Points { get; set; }

        public CircleObject(ConfigurationObject configurationObject, int id)
            : base(configurationObject, id)
        {
            Points = new HashSet<PointObject>();
        }

        public CircleObject(int id, params PointObject[] points)
            : base(id)
        {
            Points = new HashSet<PointObject>(points);
        }
    }
}