using System;
using System.Collections.Generic;
using GeoGen.Core;

namespace GeoGen.Analyzer
{
    internal sealed class LineObject : GeometricalObject
    {
        public HashSet<PointObject> Points { get; } = new HashSet<PointObject>();

        public LineObject(ConfigurationObject configurationObject, int id)
            : base(configurationObject, id)
        {
        }

        public LineObject(int id, params PointObject[] points)
            : base(id)
        {
            if (points == null)
                throw new ArgumentNullException(nameof(points));

            foreach (var pointObject in points)
            {
                if (pointObject == null)
                    throw new ArgumentException("Passed null point");

                Points.Add(pointObject);
            }
        }
    }
}