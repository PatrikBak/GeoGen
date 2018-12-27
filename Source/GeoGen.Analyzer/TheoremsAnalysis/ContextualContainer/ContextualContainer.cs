using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.AnalyticGeometry;
using GeoGen.Core;
using GeoGen.Utilities;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// A default implementation of <see cref="IContextualContainer"/>.
    /// </summary>
    public class ContextualContainer : IContextualContainer
    {
        #region Private fields

        /// <summary>
        /// The dictionary mapping objects container to the map between geometrical objects
        /// and analytic objects (from that container).
        /// </summary>
        private readonly Dictionary<IObjectsContainer, Map<GeometricalObject, AnalyticObject>> _objects;

        /// <summary>
        /// The set of all old points in the container.
        /// </summary>
        private readonly HashSet<PointObject> _oldPoints;

        /// <summary>
        /// The set of all old lines in the container.
        /// </summary>
        private readonly HashSet<LineObject> _oldLines;

        /// <summary>
        /// The set of all old circles in the container.
        /// </summary>
        private readonly HashSet<CircleObject> _oldCircles;

        /// <summary>
        /// The set of all new points in the container.
        /// </summary>
        private readonly HashSet<PointObject> _newPoints;

        /// <summary>
        /// The set of all new lines in the container.
        /// </summary>
        private readonly HashSet<LineObject> _newLines;

        /// <summary>
        /// The set of all new circles in the container.
        /// </summary>
        private readonly HashSet<CircleObject> _newCircles;

        /// <summary>
        /// The id of the last identified geometrical object by the container.
        /// </summary>
        private int _geometricalObjectsNextId;

        /// <summary>
        /// The set of ids of configuration objects that are present in the container.
        /// </summary>
        private readonly HashSet<int> _ids;

        #endregion
        
        #region Private properties

        /// <summary>
        /// All line objects in the container.
        /// </summary>
        private IEnumerable<LineObject> AllLines => _oldLines.Concat(_newLines);

        /// <summary>
        /// All circle objects in the container.
        /// </summary>
        private IEnumerable<CircleObject> AllCircles => _oldCircles.Concat(_newCircles);

        /// <summary>
        /// All point objects in the container.
        /// </summary>
        private IEnumerable<PointObject> AllPoints => _oldPoints.Concat(_newPoints);

        #endregion

        #region IContectualContainer properties

        /// <summary>
        /// Gets the objects container manager that holds all the  representations of 
        /// the objects inside this container.
        /// </summary>
        public IObjectsContainersManager Manager { get; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor.
        /// </summary>
        /// <param name="configuration">The configuration to be represented by this container.</param>
        /// <param name="manager">The manager holding all objects containers.</param>
        public ContextualContainer(Configuration configuration, IObjectsContainersManager manager)
        {
            Manager = manager ?? throw new ArgumentNullException(nameof(manager));
            _oldLines = new HashSet<LineObject>();
            _newLines = new HashSet<LineObject>();
            _oldPoints = new HashSet<PointObject>();
            _newPoints = new HashSet<PointObject>();
            _oldCircles = new HashSet<CircleObject>();
            _newCircles = new HashSet<CircleObject>();
            _ids = new HashSet<int>();
            _objects = new Dictionary<IObjectsContainer, Map<GeometricalObject, AnalyticObject>>();

            // Initialize the objects dictionary
            foreach (var objectsContainer in manager)
            {
                _objects.Add(objectsContainer, new Map<GeometricalObject, AnalyticObject>());
            }

            // Add all objects
            AddAll(configuration);
        }

        #endregion

        #region IContextualContainer implementation
        
        /// <summary>
        /// Gets the geometrical objects matching a given query and casts them
        /// to a given type.
        /// </summary>
        /// <typeparam name="T">The type of objects.</typeparam>
        /// <param name="query">The contextual container query.</param>
        /// <returns>The objects.</returns>
        public IEnumerable<T> GetGeometricalObjects<T>(ContexualContainerQuery query) where T : GeometricalObject
        {
            // Prepare result
            var result = Enumerable.Empty<GeometricalObject>();

            // If we should include points
            if (query.IncludePoints)
            {
                // Then we decide by the objects type
                switch (query.Type)
                {
                    case ContexualContainerQuery.ObjectsType.New:
                        result = result.Concat(_newPoints);
                        break;
                    case ContexualContainerQuery.ObjectsType.Old:
                        result = result.Concat(_oldPoints);
                        break;
                    case ContexualContainerQuery.ObjectsType.All:
                        result = result.Concat(AllPoints);
                        break;
                }
            }

            // If we should include lines
            if (query.IncludeLines)
            {
                // Then we decide by the objects type
                switch (query.Type)
                {
                    case ContexualContainerQuery.ObjectsType.New:
                        result = result.Concat(_newLines);
                        break;
                    case ContexualContainerQuery.ObjectsType.Old:
                        result = result.Concat(_oldLines);
                        break;
                    case ContexualContainerQuery.ObjectsType.All:
                        result = result.Concat(AllLines);
                        break;
                }
            }

            // If we should include circles
            if (query.IncludeCirces)
            {
                // Then we decide by the objects type
                switch (query.Type)
                {
                    case ContexualContainerQuery.ObjectsType.New:
                        result = result.Concat(_newCircles);
                        break;
                    case ContexualContainerQuery.ObjectsType.Old:
                        result = result.Concat(_oldCircles);
                        break;
                    case ContexualContainerQuery.ObjectsType.All:
                        result = result.Concat(AllCircles);
                        break;
                }
            }

            // Return casted result
            return result.Cast<T>();
        }

        /// <summary>
        /// Gets the analytic representation of a given geometrical object in a given objects container.
        /// </summary>
        /// /// <typeparam name="T">The wanted type of the analytic object.</typeparam>
        /// <param name="geometricalObject">The geometrical object.</param>
        /// <param name="objectsContainer">The objects container.</param>
        /// <returns>The analytic object.</returns>
        public T GetAnalyticObject<T>(GeometricalObject geometricalObject, IObjectsContainer objectsContainer) where T : AnalyticObject
        {
            // Find the right map and pull the analytic object from it
            return (T) _objects[objectsContainer].GetRightValue(geometricalObject);
        }

        #endregion

        #region Adding new objects

        /// <summary>
        /// Adds all objects from a configuration to the container.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        private void AddAll(Configuration configuration)
        {
            // Add loose objects
            configuration.LooseObjectsHolder.LooseObjects.ForEach(obj => Add(obj, isNew: false));

            // Add constructed objects
            configuration.ConstructedObjects.ForEach(obj => Add(obj, isNew: obj == configuration.ConstructedObjects.Last()));
        }

        /// <summary>
        /// Adds a given object and all objects that it implicitly creates (lines / circles)
        /// </summary>
        /// <param name="configurationObject">The object.</param>
        /// <param name="isNew">Indicates if this object should be added to particular new or old objects set.</param>
        private void Add(ConfigurationObject configurationObject, bool isNew)
        {
            // Pull the id
            var id = configurationObject.Id;

            // Add id to the ids set
            _ids.Add(id);

            // Let the helper method find the geometrical object that represents this object
            var geometricalObject = FindObject(configurationObject);

            // If this object is not null (i.e. there already is it's representation)
            if (geometricalObject != null)
            {
                // Pull the internal configuration object
                var internalConfigurationObject = geometricalObject.ConfigurationObject;

                // If this object is not null, it means this object has already been added
                if (internalConfigurationObject != null)
                    throw new AnalyzerException("An attempt to add existing configuration object.");

                // Otherwise we can set the internal object to this one
                geometricalObject.ConfigurationObject = configurationObject;

                // And terminate
                return;
            }

            // If this object isn't null, it means that this object doesn't exist 
            // in the container (explicitly or implicitly). We can let the helper method to a new one
            geometricalObject = CreateGeometricalObject(configurationObject);

            // We add the geometrical object to the objects dictionary
            AddToObjectsDictionary(geometricalObject);

            // If it's point
            if (configurationObject.ObjectType == ConfigurationObjectType.Point)
            {
                // Let the helper method add it as the point
                AddPoint((PointObject) geometricalObject, isNew);

                // And terminate
                return;
            }

            // Otherwise it's line or circle, we let the helper method add it
            AddLineOrCircle(geometricalObject, isNew);
        }

        /// <summary>
        /// Finds the geometrical object that corresponds to a given configuration object.
        /// </summary>
        /// <param name="configurationObject">The configuration object.</param>
        /// <returns>The geometrical object, if there is any; otherwise null.</returns>
        private GeometricalObject FindObject(ConfigurationObject configurationObject)
        {
            // Initialize resulting geometrical object
            GeometricalObject result = null;

            // We loop over containers
            foreach (var container in Manager)
            {
                // Pull the analytic representation of this object. It must exist,
                // which is a part of the contract of this class
                var analyticObject = container.Get(configurationObject);

                // Pull the map for this container
                var map = _objects[container];

                // If the map doesn't contain the analytic object
                if (!map.ContainsRightKey(analyticObject))
                {
                    // And the result is not set yet
                    if (result != null)
                    {
                        // when we clearly have inconsistency between containers
                        throw new InconsistentContainersException();
                    }

                    // Otherwise the object probably doesn't exist and we can continue
                    continue;
                }

                // But if it exists in the container, we pull it's geometrical version.
                var geometricalObject = map.GetLeftValue(analyticObject);

                // If the result has been already set to something else
                if (result != null && !ReferenceEquals(result, geometricalObject))
                {
                    // Then we clearly have inconsistency between containers
                    throw new InconsistentContainersException();
                }

                // If we're fine, we can set the result to the geometrical object
                result = geometricalObject;
            }

            // And finally return the result
            return result;
        }

        /// <summary>
        /// Adds a given geometrical object to the objects dictionary.
        /// </summary>
        /// <param name="geometricalObject">The geometrical object.</param>
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

                // Find the analytic version of this object 
                var analyticObject = container.Get(configurationPoint);

                // Add these objects to the map
                map.Add(geometricalObject, analyticObject);
            }
        }

        /// <summary>
        /// Adds a given point object to the container. It handles creating all lines
        /// that implicitly uses this point and finding all existing lines / circles
        /// that contain this point.
        /// </summary>
        /// <param name="pointObject">The point object.</param>
        /// <param name="isNew">Indicates if this point should be added to the new points set or to the old points set.</param>
        private void AddPoint(PointObject pointObject, bool isNew)
        {
            #region Update existing lines and circles that contain this point

            // Iterate over all lines
            foreach (var lineObject in AllLines)
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
            foreach (var circleObject in AllCircles)
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
            var points = AllPoints.ToList();

            // Now we construct all lines that pass through our point
            // So we iterate over all the points (that don't contain this point yet0
            foreach (var point in points)
            {
                // Resolve line
                ResolveLine(pointObject, point, isNew);
            }

            // Now we construct all circles that pass through our point
            // We iterate once over points to obtain all unique non-ordered
            // pairs of points
            for (var i = 0; i < points.Count; i++)
            {
                for (var j = i + 1; j < points.Count; j++)
                {
                    // Resolve circle
                    ResolveCircle(pointObject, points[i], points[j], isNew);
                }
            }

            #endregion

            // Add the point to the particular points set
            (isNew ? _newPoints : _oldPoints).Add(pointObject);
        }

        /// <summary>
        /// Tries to construct a new geometrical line using given two points. The order
        /// of the points is not important.
        /// </summary>
        /// <param name="point1">The first point.</param>
        /// <param name="point2">The second point.</param>
        /// <param name="isNew">Indicates if a new line should be added to the new lines set or to the old lines set.</param>
        private void ResolveLine(PointObject point1, PointObject point2, bool isNew)
        {
            // Initialize map that caches created analytic representations of this line
            var containersMap = new Dictionary<IObjectsContainer, Line>();

            // Initialize the resulting line object (that is going to be attempted to find)
            LineObject result = null;

            // Iterate over containers
            foreach (var container in Manager)
            {
                // Pull the map between geometrical and analytic objects
                var objects = _objects[container];

                // Find the analytic representations of the points in the map
                var p1 = (Point) objects.GetRightValue(point1);
                var p2 = (Point) objects.GetRightValue(point2);

                // Construct the analytic line
                var analyticLine = new Line(p1, p2);

                // Cache the result
                containersMap.Add(container, analyticLine);

                // If the line is present in the map
                if (objects.ContainsRightKey(analyticLine))
                {
                    // Then pull the geometrical line
                    var newResult = objects.GetLeftValue(analyticLine);

                    // If the current result hasn't been set and is distinct
                    // from the pulled line, then we have an inconsistency
                    if (result != null && !ReferenceEquals(result, newResult))
                        throw new InconsistentContainersException();

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

            // Otherwise we need to create a new line that contains these two points
            result = new LineObject(_geometricalObjectsNextId++, point1, point2);

            // We can immediately add the line to the particular lines set 
            (isNew ? _newLines : _oldLines).Add(result);

            // And register this line to the points (the other registration has been done 
            // by the constructor of LineObject)
            point1.Lines.Add(result);
            point2.Lines.Add(result);

            // And finally we can use the cached analytic versions of the line
            // to update the objects dictionary. We iterate over the cache dictionary
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

        /// <summary>
        /// Tries to construct a new geometrical circle using given three points. The order
        /// of the points is not important.
        /// </summary>
        /// <param name="point1">The first point.</param>
        /// <param name="point2">The second point.</param>
        /// <param name="point3">The third point.</param>
        /// <param name="isNew">Indicates if a new circle should be added to the new circles set or to the old circles set.</param>
        private void ResolveCircle(PointObject point1, PointObject point2, PointObject point3, bool isNew)
        {
            // Initialize map that caches creates analytic representations of this line
            var containersMap = new Dictionary<IObjectsContainer, Circle>();

            // Initialize the resulting circle
            CircleObject result = null;

            // Initialize variable indicating if given points are collinear
            bool? collinear = null;

            // Iterate over containers
            foreach (var container in Manager)
            {
                // Pull the map between geometrical and analytic objects
                var objects = _objects[container];

                var p1 = (Point)objects.GetRightValue(point1);
                var p2 = (Point)objects.GetRightValue(point2);
                var p3 = (Point)objects.GetRightValue(point3);

                // Prepare the analytic circle.
                Circle analyticCircle;

                try
                {
                    // Try to invoke the constructor. It may throw and argument exception
                    // if the provided points are collinear
                    analyticCircle = new Circle(p1, p2, p3);

                    // Otherwise they're not collinear
                    // If it's been marked that they are, then we have inconsistency
                    if (collinear != null && collinear.Value)
                        throw new InconsistentContainersException();

                    // If we're fine, then we mark the points as not collinear
                    collinear = false;
                }
                catch (Exception)
                {
                    // If the are collinear and they have been marked as
                    // not collinear, then we have inconsistency 
                    if (collinear != null && !collinear.Value)
                        throw new InconsistentContainersException();

                    // If we're fine, then we mark the points as collinear
                    collinear = true;

                    // And continue the iterations
                    continue;
                }

                // If we're here, then the points are not collinear and we have their
                // analytic representation. We can cache it.
                containersMap.Add(container, analyticCircle);

                // If the circle is present in the map
                if (objects.ContainsRightKey(analyticCircle))
                {
                    // Then we can pull the geometrical circle 
                    var newResult = objects.GetLeftValue(analyticCircle);

                    // If the current result hasn't been set and is distinct
                    // from the pulled circle, then we have an inconsistency
                    if (result != null && !ReferenceEquals(result, newResult))
                        throw new InconsistentContainersException();

                    // Otherwise we can update the result
                    result = (CircleObject)newResult;
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
            result = new CircleObject(_geometricalObjectsNextId++, point3, point1, point2);

            // We can add it to the particular circles set
            (isNew ? _newCircles : _oldCircles).Add(result);

            // And register this line to the points
            point1.Circles.Add(result);
            point2.Circles.Add(result);
            point3.Circles.Add(result);

            // And finally we can use the cached analytic versions of the circle
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

        /// <summary>
        /// Adds a given geometrical object, that is either a line or a circle, to the container.
        /// </summary>
        /// <param name="geometricalObject">The geometrical object.</param>
        /// <param name="isNew">Indicates if the object should be added to the particular new or old objects set.</param>
        private void AddLineOrCircle(GeometricalObject geometricalObject, bool isNew)
        {
            // Try to cast object to line and circle
            var line = geometricalObject as LineObject;
            var circle = geometricalObject as CircleObject;

            // Iterate over points 
            foreach (var pointObject in AllPoints)
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
                (isNew ? _newLines : _oldLines).Add(line);
            else if (circle != null)
                (isNew ? _newCircles : _oldCircles).Add(circle);
        }

        /// <summary>
        /// Finds out if a given point lies on a given geometrical object.
        /// </summary>
        /// <param name="pointObject">The point object.</param>
        /// <param name="geometricalObject">The geometrical object.</param>
        /// <returns>true, if the point is on the line/circle; false otherwise.</returns>
        private bool IsPointOnLineOrCircle(PointObject pointObject, GeometricalObject geometricalObject)
        {
            // Initialize variable holding result
            bool? result = null;

            // Iterate over containers
            foreach (var container in Manager)
            {
                // Pull analytic representations of the objects
                var point = (Point) _objects[container].GetRightValue(pointObject);
                var analyticObject = _objects[container].GetRightValue(geometricalObject);

                // Let the helper decide if the point lies on the object
                var liesOn = AnalyticHelpers.LiesOn(analyticObject, point);

                // If the result has been set and it differs from the currently 
                // found value, then we have inconsistency
                if (result != null && result.Value != liesOn)
                {
                    throw new InconsistentContainersException();
                }

                // Otherwise we update the result
                result = liesOn;
            }

            // Finally we can return the result (which should not be null)
            return result ?? throw new AnalyzerException("Impossible (we have at least one container).");
        }

        /// <summary>
        /// Creates a new geometrical object representing a given configuration object.
        /// </summary>
        /// <param name="configurationObject">The configuration object.</param>
        /// <returns>The new geometrical object.</returns>
        private GeometricalObject CreateGeometricalObject(ConfigurationObject configurationObject)
        {
            switch (configurationObject.ObjectType)
            {
                case ConfigurationObjectType.Point:
                    return new PointObject(_geometricalObjectsNextId++, configurationObject);
                case ConfigurationObjectType.Line:
                    return new LineObject(_geometricalObjectsNextId++, configurationObject);
                case ConfigurationObjectType.Circle:
                    return new CircleObject(_geometricalObjectsNextId++, configurationObject);
                default:
                    throw new AnalyzerException("Unhandled case.");
            }
        }

        #endregion
    }
}