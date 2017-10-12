using System;
using System.Collections.Generic;
using GeoGen.AnalyticalGeometry;
using GeoGen.Analyzer.Objects;
using Circle = GeoGen.AnalyticalGeometry.Circle;
using Line = GeoGen.AnalyticalGeometry.Line;
using Point = GeoGen.AnalyticalGeometry.Point;

namespace GeoGen.Analyzer.Constructing
{
    internal class RandomObjectsProvider : IRandomObjectsProvider
    {
        private const double MaximalRandomValue = 10.0;

        private readonly Dictionary<Type, HashSet<AnalyticalObject>> _objects;

        private readonly Random _random;

        public RandomObjectsProvider()
        {
            _random = new Random(DateTime.Now.Millisecond);
            _objects = new Dictionary<Type, HashSet<AnalyticalObject>>();
        }

        public AnalyticalObject NextRandomObject<T>() where T : AnalyticalObject
        {
            var type = typeof(T);

            HashSet<AnalyticalObject> set;

            if (!_objects.ContainsKey(type))
            {
                set = new HashSet<AnalyticalObject>();
                _objects.Add(type, set);
            }
            else
                set = _objects[type];

            AnalyticalObject result = null;

            while (true)
            {
                if (type == typeof(Point))
                    result = RandomPoint();

                if (type == typeof(Line))
                    result = RandomLine();

                if (type == typeof(Circle))
                    result = RandomCircle();

                if (result == null)
                    throw new AnalyzerException("Unhandled case");

                if (set.Add(result))
                    break;
            }

            return result;
        }

        private Point RandomPoint()
        {
            var x = RandomDoubleInRange();
            var y = RandomDoubleInRange();

            return new Point(x, y);
        }

        private Circle RandomCircle()
        {
            var center = RandomPoint();
            // This construct makes sure that we won't get a zero radius
            var radius = (1 - _random.NextDouble()) * MaximalRandomValue;

            return new Circle(center, radius);
        }

        private Line RandomLine()
        {
            var point1 = RandomPoint();
            var point2 = RandomPoint();

            return new Line(point1, point2);
        }

        private double RandomDoubleInRange()
        {
            return _random.NextDouble() * MaximalRandomValue;
        }
    }
}