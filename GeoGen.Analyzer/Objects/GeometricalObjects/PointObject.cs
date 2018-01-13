using System;
using System.Collections.Generic;
using GeoGen.Core;

namespace GeoGen.Analyzer
{
    internal sealed class PointObject : GeometricalObject
    {
        public HashSet<LineObject> Lines { get; } = new HashSet<LineObject>();

        public HashSet<CircleObject> Circles { get; } = new HashSet<CircleObject>();

        public PointObject(ConfigurationObject configurationObject, int id)
            : base(configurationObject, id)
        {
        }

        public IEnumerable<GeometricalObject> ObjectsThatContainThisPoint(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (type == typeof(LineObject))
                return Lines;

            if (type == typeof(CircleObject))
                return Circles;

            throw new AnalyzerException($"Can't return objects for type: {type}");
        }
    }
}