using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GeoGen.AnalyticalGeometry;
using GeoGen.AnalyticalGeometry.AnalyticalObjects;
using GeoGen.Core.Configurations;
using GeoGen.Core.Utilities;
using GeoGen.Utilities;

namespace GeoGen.Analyzer
{
    internal sealed class ContextualContainer : IContextualContainer
    {
        #region Private fields

        private readonly IObjectsContainersManager _containersManager;

        private readonly IAnalyticalHelper _analyticalHelper;

        private readonly Dictionary<IObjectsContainer, Map<GeometricalObject, IAnalyticalObject>> _objects;

        private readonly Dictionary<int, GeometricalObject> _configurationObjectIdToGeometricalObjects;

        private readonly HashSet<LineObject> _lines;

        private readonly HashSet<CircleObject> _circles;

        private readonly HashSet<PointObject> _points;

        private int _geometricalObjectsNextId;

        #endregion

        #region Constructor

        public ContextualContainer(IObjectsContainersManager containersManager, IAnalyticalHelper analyticalHelper)
        {
            _containersManager = containersManager ?? throw new ArgumentNullException(nameof(containersManager));
            _analyticalHelper = analyticalHelper ?? throw new ArgumentNullException(nameof(analyticalHelper));
            _configurationObjectIdToGeometricalObjects = new Dictionary<int, GeometricalObject>();
            _lines = new HashSet<LineObject>();
            _circles = new HashSet<CircleObject>();
            _points = new HashSet<PointObject>();

            _objects = new Dictionary<IObjectsContainer, Map<GeometricalObject, IAnalyticalObject>>();

            foreach (var objectsContainer in containersManager)
            {
                _objects.Add(objectsContainer, new Map<GeometricalObject, IAnalyticalObject>());
            }
        }

        #endregion

        #region IContextualContainer implementation

        public void Add(ConfigurationObject configurationObject)
        {
            if (configurationObject == null)
                throw new ArgumentNullException(nameof(configurationObject));

            // Pull the id
            var id = configurationObject.Id ?? throw new AnalyzerException("Id must be set");

            // Make sure that this id hasn't been resolved
            if (_configurationObjectIdToGeometricalObjects.ContainsKey(id))
                throw new AnalyzerException("Object with this id has already been added.");

            // Let the helper method find the geometrical object that
            // represents this object
            var geometricalObject = FindObject(configurationObject);

            // If this object is not null
            if (geometricalObject != null)
            {
                // Pull the internal configuration object
                var internalConfigurationObject = geometricalObject.ConfigurationObject;

                // If this object is not null, it means this object has already been added
                if (internalConfigurationObject != null)
                    throw new AnalyzerException("An attempt to add existing configuration object.");

                // Otherwise we can set the internal object to this
                geometricalObject.ConfigurationObject = configurationObject;

                // Register it to the dictionary
                _configurationObjectIdToGeometricalObjects.Add(id, geometricalObject);

                // And terminate
                return;
            }

            // Otherwise this object doesn't exist in the container (explicitly or implicitly).
            // We can let the helper method to create it
            geometricalObject = CreateGeometricalObject(configurationObject);

            // Register it to the dictionary
            _configurationObjectIdToGeometricalObjects.Add(id, geometricalObject);

            // If it's point
            if (configurationObject.ObjectType == ConfigurationObjectType.Point)
            {
                // Let the helper method add it as the point
                AddPoint((PointObject) geometricalObject);

                // And terminate
                return;
            }

            // Otherwise it's line or circle, we let the helper method add it
            AddLineOrCircle(geometricalObject);
        }

        public IEnumerable<T> GetObjects<T>(ConfigurationObjectsMap objects) where T : GeometricalObject
        {
            if (objects == null)
                throw new ArgumentNullException(nameof(objects));

            // If we're asked to get points
            if (typeof(T) == typeof(PointObject))
            {
                // Then we simply cast points from the map to their geometrical
                // objects
                var points = objects[ConfigurationObjectType.Point]
                        .Select(GetGeometricalObject)
                        .Cast<T>();

                // Iterate over them to yield them
                foreach (var point in points)
                {
                    yield return point;
                }

                // And terminate the iterator
                yield break;
            }

            // Otherwise we have line / circle. According to which one we need to
            // decide the number of points needed to construct them
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

            // Not we get all points that are present in the map
            var pointObjects = objects[ConfigurationObjectType.Point]
                    .Select(GetGeometricalObject)
                    .Cast<PointObject>()
                    .ToList();

            // Finally we can prepare enumerator to construct the result
            var resultingObjects = pointObjects
                    // We cast each point to lines / circles that contains that point
                    .SelectMany(point => point.ObjectsThatContainThisPoint(typeof(T)))
                    // Make sure that we have only distinct ones
                    .Distinct()
                    .Cast<T>()
                    // Take only those that are constructible from our points
                    .Where(obj => CanBeConstructedFromPoints(obj, neededPoints, pointObjects))
                    // And union them with lines / circles already present in the map
                    .Union(objects[type].Select(GetGeometricalObject).Cast<T>());

            // Now we just enumerate the result
            foreach (var resultingObject in resultingObjects)
            {
                yield return resultingObject;
            }
        }

        public IEnumerable<T> GetNewObjects<T>(ConfigurationObjectsMap oldObjects, ConfigurationObjectsMap newObjects)
                where T : GeometricalObject
        {
            if (oldObjects == null)
                throw new ArgumentNullException(nameof(oldObjects));

            if (newObjects == null)
                throw new ArgumentNullException(nameof(newObjects));

            // If we're asked to get points
            if (typeof(T) == typeof(PointObject))
            {
                // Then we simply cast points from the map to their geometrical
                // objects
                var points = newObjects[ConfigurationObjectType.Point]
                        .Select(GetGeometricalObject)
                        .Cast<T>();

                // Iterate over them to yield them
                foreach (var point in points)
                {
                    yield return point;
                }

                // And terminate the iterator
                yield break;
            }

            // Otherwise we have line / circle. According to which one we need to
            // decide the number of points needed to construct them
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

            // Now we prepare some objects.
            // We need set of lines / circles that are physically present
            // in old objects
            var oldObjectsOfType = oldObjects[type]
                    .Select(GetGeometricalObject)
                    .ToSet();

            // Then we need all points from old objects
            var oldPoints = oldObjects[ConfigurationObjectType.Point]
                    .Select(GetGeometricalObject)
                    .Cast<PointObject>()
                    .ToList();

            // And all new points from new objects
            var newPoints = newObjects[ConfigurationObjectType.Point]
                    .Select(GetGeometricalObject)
                    .Cast<PointObject>()
                    .ToList();

            // And finally list of all points
            var allPoints = oldPoints.Concat(newPoints).ToList();

            // Finally we can construct the enumerable for the result
            var resultingObjects = newPoints
                    // Cast point to set of lines / circles that contain it 
                    // TODO: This might be inefficient for the big container. 
                    .SelectMany(point => point.ObjectsThatContainThisPoint(typeof(T)))
                    // Take only distinct ones
                    .Distinct()
                    // Take only those that can be constructed from all of our points
                    .Where(obj => CanBeConstructedFromPoints(obj, neededPoints, allPoints))
                    // Union with new objects
                    .Union(newObjects[type].Select(GetGeometricalObject))
                    // Take only those that CAN'T be constructed from old points 
                    .Where(obj => !CanBeConstructedFromPoints(obj, neededPoints, oldPoints))
                    // And are not contained in the old physical lines / circles
                    .Where(obj => !oldObjectsOfType.Contains(obj));

            // And enumerate it
            foreach (var resultingObject in resultingObjects.Cast<T>())
            {
                yield return resultingObject;
            }
        }

        public IAnalyticalObject GetAnalyticalObject(GeometricalObject geometricalObject, IObjectsContainer objectsContainer)
        {
            if (geometricalObject == null)
                throw new ArgumentNullException(nameof(geometricalObject));

            if (objectsContainer == null)
                throw new ArgumentNullException(nameof(objectsContainer));

            return _objects[objectsContainer].GetRight(geometricalObject);
        }

        #endregion

        #region IEnumerable implementation

        public IEnumerator<GeometricalObject> GetEnumerator()
        {
            return _points.Cast<GeometricalObject>()
                    .Concat(_lines)
                    .Concat(_circles)
                    .GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region Private methods

        private GeometricalObject FindObject(ConfigurationObject configurationObject)
        {
            // Initialize resulting geometrical object
            GeometricalObject result = null;

            // We loop over containers
            foreach (var container in _containersManager)
            {
                // Pull the analytical representation of this object. It must exist,
                // which is a part of the contract of this class
                var analyticalObject = container.Get(configurationObject);

                // Pull the map for this container
                var map = _objects[container];

                // If the map doesn't contain the analytical object
                if (!map.ContainsRight(analyticalObject))
                {
                    // And the result is not 
                    if (result != null)
                    {
                        // Then we clearly have inconsistency between containers
                        throw new InconsistentContainersException(container);
                    }

                    // Otherwise the object probably doesn't exist and we can continue
                    continue;
                }

                // But if it exists in the container, we pull it's geometrical version.
                var geometricalObject = map.GetLeft(analyticalObject);

                // If the result has been already set to something else
                if (result != null && geometricalObject != result)
                {
                    // Then we clearly have inconsistency between containers
                    throw new InconsistentContainersException(container);
                }

                // If we're fine, we can set the result to the geometrical object
                result = geometricalObject;
            }

            // And finally return the result
            return result;
        }

        private void AddPoint(PointObject pointObject)
        {
            // Add the point to the objects dictionary
            AddToObjectsDictionary(pointObject);

            #region Update existing lines and circles that contain this point

            // Iterate over all lines
            foreach (var lineObject in _lines)
            {
                // If this point lies on it
                if (IsPointOnLineOrCircle(pointObject, lineObject))
                {
                    // Register the point to the line
                    lineObject.Points.Add(pointObject);

                    // Register the line to the point
                    pointObject.Lines.Add(lineObject);
                }
            }

            // Iterate over all circles
            foreach (var circleObject in _circles)
            {
                // If this point lies on it
                if (IsPointOnLineOrCircle(pointObject, circleObject))
                {
                    // Register the point to the circle
                    circleObject.Points.Add(pointObject);

                    // Register the circle to the point
                    pointObject.Circles.Add(circleObject);
                }
            }

            #endregion

            #region Create new lines and circles from this point and others

            // Enumerate all the points
            var points = _points.ToList();

            // Now we construct all lines that pass through our point
            // So we iterate over all the points (that don't contain this point yet0
            foreach (var point in points)
            {
                // Resolve line
                ResolveLine(pointObject, point);
            }

            // Now we construct all circles that pass through our point
            // We iterate once over points to obtain all unique non-ordered
            // pairs of points
            for (var i = 0; i < points.Count; i++)
            {
                for (var j = i + 1; j < points.Count; j++)
                {
                    // Resolve circle
                    ResolveCircle(pointObject, points[i], points[j]);
                }
            }

            #endregion

            // Add the point to the points set
            _points.Add(pointObject);
        }

        private void AddToObjectsDictionary(GeometricalObject geometricalObject)
        {
            // Loop over pairs [container, map]
            foreach (var pair in _objects)
            {
                // Pull the container
                var container = pair.Key;

                // Pull the map
                var map = pair.Value;

                // Pull the configuration object representing this object
                var configurationPoint = geometricalObject.ConfigurationObject;

                // Find the analytical version of this object 
                var analyticalObject = container.Get(configurationPoint);

                // Add these objects to the map
                map.Add(geometricalObject, analyticalObject);
            }
        }

        private void ResolveLine(PointObject proccesedPoint, PointObject pointFromEnumerator)
        {
            // Initialize map that caches creates analytical representations of this line
            var containersMap = new Dictionary<IObjectsContainer, Line>();

            // Initialize the resulting line (that is going to be attempted to find)
            LineObject result = null;

            // Iterate over containers
            foreach (var container in _containersManager)
            {
                // Pull the map between geometrical and analytical objects
                var objects = _objects[container];

                // Find the analytical representations of the points in the map
                var p1 = (Point) objects.GetRight(proccesedPoint);
                var p2 = (Point) objects.GetRight(pointFromEnumerator);

                // Construct the analytical line
                var analyticalLine = new Line(p1, p2);

                // Cache the result
                containersMap.Add(container, analyticalLine);

                // If the line is present in the map
                if (objects.ContainsRight(analyticalLine))
                {
                    // Then pull the geometrical line
                    var newResult = objects.GetLeft(analyticalLine);

                    // If the current result hasn't been set and is distinct
                    // from the pulled line, then we have an inconsistency
                    if (result != null && result != newResult)
                        throw new InconsistentContainersException(container);

                    // Otherwise we can update the result
                    result = (LineObject) newResult;
                }
            }

            // If the result is not null (i.e. the line physically exist) 
            if (result != null)
            {
                // We can simply terminate
                return;
            }

            // Otherwise we need to create a new line that contains these two points initially
            result = new LineObject(_geometricalObjectsNextId++, proccesedPoint, pointFromEnumerator);

            // We can immediately add the line to the lines set 
            _lines.Add(result);

            // And register this line to the points (the other registration has been done 
            // by the constructor of LineObject)
            proccesedPoint.Lines.Add(result);
            pointFromEnumerator.Lines.Add(result);

            // And finally we can use the cached analytical versions of the line
            // So we iterate over the cache dictionary
            foreach (var pair in containersMap)
            {
                // Pull the container
                var container = pair.Key;

                // Pull the line
                var line = pair.Value;

                // Update the real objects dictionary
                _objects[container].Add(result, line);
            }
        }

        private void ResolveCircle(PointObject proccesedPoint, PointObject point1, PointObject point2)
        {
            // Initialize map that caches creates analytical representations of this line
            var containersMap = new Dictionary<IObjectsContainer, Circle>();

            // Initialize the resulting circle
            CircleObject result = null;

            // Initialize variable indicating if given points are collinear
            bool? collinear = null;

            // Iterate over containers
            foreach (var container in _containersManager)
            {
                // Pull the map between geometrical and analytical objects
                var objects = _objects[container];

                var p1 = (Point) objects.GetRight(point1);
                var p2 = (Point) objects.GetRight(point2);
                var p3 = (Point) objects.GetRight(proccesedPoint);

                // Prepare the analytical circle.
                Circle analyticalCircle;

                try
                {
                    // Try to invoke the constructor. It may throw and argument exception
                    // if the provided points are collinear
                    analyticalCircle = new Circle(p1, p2, p3);

                    // Otherwise they're not collinear
                    // If it's been marked that they are, then we have inconsistency

                    if (collinear != null && collinear.Value)
                        throw new InconsistentContainersException(container);

                    // If we're fine, then we mark the points as not collinear
                    collinear = false;
                }
                catch (ArgumentException)
                {
                    // If the are collinear and they have been marked as
                    // not collinear, then we have inconsistency 

                    if (collinear != null && !collinear.Value)
                        throw new InconsistentContainersException(container);

                    // If we're fine, then we mark the points as collinear
                    collinear = true;

                    // And continue the iterations
                    continue;
                }

                // If we're here, then the points are not collinear and we have their
                // analytical representation. We can cache it.
                containersMap.Add(container, analyticalCircle);

                // If the circle is present in the map
                if (objects.ContainsRight(analyticalCircle))
                {
                    // Then we can pull the geometrical circle 
                    var newResult = objects.GetLeft(analyticalCircle);

                    // If the current result hasn't been set and is distinct
                    // from the pulled circle, then we have an inconsistency
                    if (result != null && result != newResult)
                        throw new InconsistentContainersException(container);

                    // Otherwise we can update the result
                    result = (CircleObject) newResult;
                }
            }

            // If the result is not null (i.e. the circle physically exist) 
            if (result != null)
            {
                // We can simply terminate
                return;
            }

            // If we have collinear points
            if (collinear ?? throw new Exception("Impossible (at least one container exists)"))
            {
                // Then we can terminate as well (we hardly can construct a circle...)
                return;
            }

            // Otherwise we can create a new circle object with these points
            result = new CircleObject(_geometricalObjectsNextId++, proccesedPoint, point1, point2);

            // We can add it to the circles set
            _circles.Add(result);

            // And register this line to the points (the other registration has been done 
            // by the constructor of CircleObject)
            proccesedPoint.Circles.Add(result);
            point1.Circles.Add(result);
            point2.Circles.Add(result);

            // And finally we can use the cached analytical versions of the circle
            // So we iterate over the cache dictionary
            foreach (var pair in containersMap)
            {
                // Pull the container
                var container = pair.Key;

                // Pull the circle
                var circle = pair.Value;

                // Update the real objects dictionary
                _objects[container].Add(result, circle);
            }
        }

        private void AddLineOrCircle(GeometricalObject geometricalObject)
        {
            // Let the helper method add this objects to objects dictionary
            AddToObjectsDictionary(geometricalObject);

            // Try to cast object to line and circle
            var line = geometricalObject as LineObject;
            var circle = geometricalObject as CircleObject;

            // Iterate over points 
            foreach (var pointObject in _points)
            {
                // If the given points lies on the line / circle, we want
                // to mark it
                if (IsPointOnLineOrCircle(pointObject, geometricalObject))
                {
                    // If it's line
                    if (line != null)
                    {
                        // Register the point to the line
                        line.Points.Add(pointObject);

                        // Register the line to the line
                        pointObject.Lines.Add(line);
                    }
                    // Or if it's circle
                    else if (circle != null)
                    {
                        // Register the point to the line
                        pointObject.Circles.Add(circle);

                        // Register the line to the line
                        circle.Points.Add(pointObject);
                    }
                }
            }

            // Finally we add objects to the particular set.
            if (line != null)
                _lines.Add(line);
            else if (circle != null)
                _circles.Add(circle);
        }

        private bool IsPointOnLineOrCircle(PointObject pointObject, GeometricalObject geometricalObject)
        {
            // Initialize variable holding result
            bool? result = null;

            // Iterate over containers
            foreach (var container in _containersManager)
            {
                // Pull analytical representations of the objects
                var point = (Point) _objects[container].GetRight(pointObject);
                var analyticalObject = _objects[container].GetRight(geometricalObject);

                // Let the helper decide if the point lies on the object
                var liesOn = _analyticalHelper.LiesOn(analyticalObject, point);

                // If the result has been set and it differs from the currently 
                // found value, then we have inconsistency
                if (result != null && result.Value != liesOn)
                {
                    throw new InconsistentContainersException(container);
                }

                // Otherwise we update the result
                result = liesOn;
            }

            // Finally we can return the result (which should not be null)
            return result ?? throw new AnalyzerException("Impossible (we have at least one container).");
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
                    throw new AnalyzerException("Unhandled case.");
            }
        }

        private bool CanBeConstructedFromPoints(GeometricalObject obj, int neededPoints, IEnumerable<PointObject> points)
        {
            // Initialize interior points
            HashSet<PointObject> interiorPoints;

            // Try to cast the object to line and circle
            var lineObject = obj as LineObject;
            var circleObject = obj as CircleObject;

            // Pull the points from the successfully casted type
            if (lineObject != null)
                interiorPoints = lineObject.Points;
            else if (circleObject != null)
                interiorPoints = circleObject.Points;
            else
                throw new AnalyzerException("Unhandled case");


            // Initialize the counter indicating the number of points
            // that are interior points of our line / circle and are also 
            // contained in the given points collection
            var counter = 0;

            // We iterate over provided points (their count should be
            // usually less than the number of interior points)
            foreach (var pointObject in points)
            {
                // If the current point is not in the interior, we can skip it
                if (!interiorPoints.Contains(pointObject))
                    continue;

                // Otherwise we increment the counter
                counter++;

                // And if we've found the needed number of points, we can terminate
                if (counter == neededPoints)
                    return true;
            }

            // If we got here, then there are not enough points, so we can return false
            return false;
        }

        private GeometricalObject GetGeometricalObject(ConfigurationObject configurationObject)
        {
            // Pull id
            var id = configurationObject.Id ?? throw new AnalyzerException("Id must be set.");

            // Look up the object in the dictionary
            return _configurationObjectIdToGeometricalObjects[id];
        }

        #endregion
    }
}