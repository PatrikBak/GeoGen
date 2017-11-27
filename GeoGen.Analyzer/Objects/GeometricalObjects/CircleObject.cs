using System;
using System.Collections.Generic;
using GeoGen.Core.Configurations;

namespace GeoGen.Analyzer.Objects.GeometricalObjects
{
    internal sealed class CircleObject : GeometricalObject
    {
        public HashSet<PointObject> Points { get; } = new HashSet<PointObject>();

        public CircleObject(ConfigurationObject configurationObject, int id)
            : base(configurationObject, id)
        {
        }

        public CircleObject(int id, params PointObject[] points)
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