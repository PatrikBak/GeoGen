using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.AnalyticalGeometry;
using GeoGen.AnalyticalGeometry.Objects;
using GeoGen.Core.Configurations;
using GeoGen.Core.Utilities;

namespace GeoGen.Analyzer.Objects.GeometricalObjects.Container
{
    internal sealed class ContextualContainer : IContextualContainer
    {
        private readonly IObjectsContainersHolder _containersHolder;

        private readonly IAnalyticalHelper _analyticalHelper;

        private readonly Dictionary<IObjectsContainer, Map<GeometricalObject, IAnalyticalObject>> _objects;

        private readonly Dictionary<int, GeometricalObject> _configurationObjectIdToGeometrical;

        private readonly Dictionary<PointObject, Dictionary<Type, HashSet<GeometricalObject>>> _pointsToObjects;

        private readonly HashSet<LineObject> _lines;

        private readonly HashSet<CircleObject> _circles;

        private int _geometricalObjectsNextId;

        public void Add(ConfigurationObject configurationObject)
        {
            var geometricalObject = FindObject(configurationObject);

            void AddToIdDictionary()
            {
                var configurationObjectId = configurationObject.Id ?? throw new AnalyzerException("Id must be set");

                _configurationObjectIdToGeometrical.Add(configurationObjectId, geometricalObject);
            }

            if (geometricalObject != null)
            {
                geometricalObject.ConfigurationObject = configurationObject;
                AddToIdDictionary();
                return;
            }

            geometricalObject = CreateGeometricalObject(configurationObject);
            AddToIdDictionary();

            if (configurationObject.ObjectType == ConfigurationObjectType.Point)
            {
                AddPoint((PointObject) geometricalObject);
                return;
            }

            AddLineOrCircle(geometricalObject);
        }

        private GeometricalObject FindObject(ConfigurationObject configurationObject)
        {
            GeometricalObject result = null;

            foreach (var container in _containersHolder)
            {
                var analyticalObject = container.Get(configurationObject);

                var map = _objects[container];

                if (!map.ContainsRight(analyticalObject))
                {
                    if (result != null)
                    {
                        throw new AnalyzerException("Inconsistent containers");
                    }

                    continue;
                }

                var mapResult = map.Reverse[analyticalObject];

                if (result != null && mapResult != result)
                {
                    throw new AnalyzerException("Inconsistent containers");
                }

                result = mapResult;
            }

            return result;
        }

        private void AddPoint(PointObject pointObject)
        {
            var lineObjects = new HashSet<LineObject>();
            var circleObjects = new HashSet<CircleObject>();

            foreach (var lineObject in _lines)
            {
                if (IsPointOnLineOrCircle(pointObject, lineObject))
                {
                    lineObjects.Add(lineObject);
                }
            }

            foreach (var circleObject in _circles)
            {
                if (IsPointOnLineOrCircle(pointObject, circleObject))
                {
                    circleObjects.Add(circleObject);
                }
            }

            var points = _pointsToObjects.Keys.ToList();

            foreach (var point in points)
            {
                var line = ResolveLine(pointObject, point);
                lineObjects.Add(line);
            }

            for (var i = 0; i < points.Count; i++)
            {
                for (var j = i + 1; j < points.Count; j++)
                {
                    circleObjects.Add(ResolveCircle(pointObject, points[i], points[j]));
                }
            }

            var dictionary = new Dictionary<Type, HashSet<GeometricalObject>>
            {
                {typeof(LineObject), new HashSet<GeometricalObject>()},
                {typeof(CircleObject), new HashSet<GeometricalObject>()}
            };

            foreach (var lineObject in lineObjects)
            {
                lineObject.Points.Add(pointObject);
                dictionary[typeof(LineObject)].Add(lineObject);
            }

            foreach (var circleObject in circleObjects)
            {
                circleObject.Points.Add(pointObject);
                dictionary[typeof(CircleObject)].Add(circleObject);
            }

            _pointsToObjects.Add(pointObject, dictionary);
        }

        private CircleObject ResolveCircle(PointObject proccesedPoint, PointObject point1, PointObject point2)
        {
            var containersMap = new Dictionary<IObjectsContainer, Circle>();
            CircleObject result = null;

            foreach (var container in _containersHolder)
            {
                var objects = _objects[container];

                var p1 = (Point) objects.Forward[point1];
                var p2 = (Point) objects.Forward[point2];
                var p3 = (Point) objects.Forward[proccesedPoint];

                var analyticalCircle = new Circle(p1, p2, p3);
                containersMap.Add(container, analyticalCircle);

                if (objects.ContainsRight(analyticalCircle))
                {
                    var newResult = objects.Reverse[analyticalCircle];

                    if (result != null && result != newResult)
                        throw new AnalyzerException("Inconsistent containers");

                    result = (CircleObject) newResult;
                }
            }

            if (result != null)
                return result;

            result = new CircleObject(_geometricalObjectsNextId++, proccesedPoint, point1, point2);

            _circles.Add(result);

            _pointsToObjects[proccesedPoint][typeof(LineObject)].Add(result);

            foreach (var pair in containersMap)
            {
                var container = pair.Key;
                var line = pair.Value;

                _objects[container].Add(result, line);
            }

            return result;
        }

        private LineObject ResolveLine(PointObject proccesedPoint, PointObject pointFromEnumerator)
        {
            var containersMap = new Dictionary<IObjectsContainer, Line>();
            LineObject result = null;

            foreach (var container in _containersHolder)
            {
                var objects = _objects[container];

                var p1 = (Point) objects.Forward[proccesedPoint];
                var p2 = (Point) objects.Forward[pointFromEnumerator];

                var analyticalLine = new Line(p1, p2);
                containersMap.Add(container, analyticalLine);

                if (objects.ContainsRight(analyticalLine))
                {
                    var newResult = objects.Reverse[analyticalLine];

                    if (result != null && result != newResult)
                        throw new AnalyzerException("Inconsistent containers");

                    result = (LineObject) newResult;
                }
            }

            if (result != null)
                return result;

            result = new LineObject(_geometricalObjectsNextId++, proccesedPoint, pointFromEnumerator);

            _lines.Add(result);

            _pointsToObjects[pointFromEnumerator][typeof(LineObject)].Add(result);

            foreach (var pair in containersMap)
            {
                var container = pair.Key;
                var line = pair.Value;

                _objects[container].Add(result, line);
            }

            return result;
        }

        private void AddLineOrCircle(GeometricalObject geometricalObject)
        {
            var line = geometricalObject as LineObject;
            var circle = geometricalObject as CircleObject;

            foreach (var keyValuePair in _pointsToObjects)
            {
                var pointObject = keyValuePair.Key;

                if (IsPointOnLineOrCircle(pointObject, geometricalObject))
                {
                    line?.Points.Add(pointObject);
                    circle?.Points.Add(pointObject);
                }

                keyValuePair.Value[geometricalObject.GetType()].Add(geometricalObject);
            }

            if (line != null)
                _lines.Add(line);

            if (circle != null)
                _circles.Add(circle);
        }

        private bool IsPointOnLineOrCircle(PointObject pointObject, GeometricalObject geometricalObject)
        {
            bool? result = null;

            foreach (var container in _containersHolder)
            {
                var point = (Point) _objects[container].Forward[pointObject];
                var analyticalObject = _objects[container].Forward[geometricalObject];

                var liesOn = _analyticalHelper.LiesOn(analyticalObject, point);

                if (result.HasValue && result.Value != liesOn)
                {
                    throw new AnalyzerException("Inconsistent containers");
                }

                result = liesOn;
            }

            return result ?? throw new AnalyzerException("Impossible");
        }

        private GeometricalObject CreateGeometricalObject(ConfigurationObject configurationObject)
        {
            switch (configurationObject.ObjectType)
            {
                case ConfigurationObjectType.Point:
                    return new PointObject(configurationObject, _geometricalObjectsNextId++);
                case ConfigurationObjectType.Line:
                    return new LineObject(configurationObject, _geometricalObjectsNextId++);
                case ConfigurationObjectType.Circle:
                    return new CircleObject(configurationObject, _geometricalObjectsNextId++);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public IEnumerable<T> GetNewObjects<T>(ConfigurationObjectsMap oldObjects, ConfigurationObjectsMap newObjects)
            where T : GeometricalObject
        {
            if (typeof(T) == typeof(PointObject))
            {
                var points = newObjects[ConfigurationObjectType.Point]
                        .Select(GetGeometricalObject)
                        .Cast<T>();

                foreach (var point in points)
                {
                    yield return point;
                }

                yield break;
            }

            ConfigurationObjectType type;
            int neededPoints;

            if (typeof(T) == typeof(LineObject))
            {
                type = ConfigurationObjectType.Line;
                neededPoints = 2;
            }
            else
            {
                type = ConfigurationObjectType.Circle;
                neededPoints = 3;
            }

            var oldPoints = oldObjects[ConfigurationObjectType.Point]
                    .Select(GetGeometricalObject)
                    .Cast<PointObject>()
                    .ToList();

            var resultingObjects = newObjects[ConfigurationObjectType.Point]
                    .Select(GetGeometricalObject)
                    .Cast<PointObject>()
                    .SelectMany(point => _pointsToObjects[point][typeof(T)])
                    .Distinct()
                    .Union(newObjects[type].Select(GetGeometricalObject))
                    .Where(obj => !CanBeConstructedFromPoints(obj, type, neededPoints, oldPoints))
                    .Cast<T>();

            foreach (var resultingObject in resultingObjects)
            {
                yield return resultingObject;
            }
        }

        public IEnumerable<T> GetObjects<T>(ConfigurationObjectsMap objects) where T : GeometricalObject
        {
            if (typeof(T) == typeof(PointObject))
            {
                var points = objects[ConfigurationObjectType.Point]
                        .Select(GetGeometricalObject)
                        .Cast<T>();

                foreach (var point in points)
                {
                    yield return point;
                }

                yield break;
            }

            ConfigurationObjectType type;
            int neededPoints;

            if (typeof(T) == typeof(LineObject))
            {
                type = ConfigurationObjectType.Line;
                neededPoints = 2;
            }
            else
            {
                type = ConfigurationObjectType.Circle;
                neededPoints = 3;
            }

            var pointObjects = objects[ConfigurationObjectType.Point]
                    .Select(GetGeometricalObject)
                    .Cast<PointObject>()
                    .ToList();

            var resultingObjects = pointObjects
                    .SelectMany(point => _pointsToObjects[point][typeof(T)])
                    .Distinct()
                    .Where(obj => CanBeConstructedFromPoints(obj, type, neededPoints, pointObjects))
                    .Union(objects[type].Select(GetGeometricalObject))
                    .Cast<T>();

            foreach (var resultingObject in resultingObjects)
            {
                yield return resultingObject;
            }
        }

        public IAnalyticalObject GetAnalyticalObject(GeometricalObject geometricalObject, IObjectsContainer objectsContainer)
        {
            return _objects[objectsContainer].Forward[geometricalObject];
        }

        private static bool CanBeConstructedFromPoints
        (
            GeometricalObject geometricalObject,
            ConfigurationObjectType type,
            int neededPoints,
            ICollection<PointObject> points)
        {
            var interiorPoints = type == ConfigurationObjectType.Line
                ? ((LineObject) geometricalObject).Points
                : ((CircleObject) geometricalObject).Points;

            var counter = 0;

            foreach (var pointObject in interiorPoints)
            {
                if (!points.Contains(pointObject))
                    continue;

                counter++;

                if (counter == neededPoints)
                    return true;
            }

            return false;
        }

        private GeometricalObject GetGeometricalObject(ConfigurationObject configurationObject)
        {
            var id = configurationObject.Id ?? throw new AnalyzerException("Id must be set.");

            return _configurationObjectIdToGeometrical[id];
        }
    }
}