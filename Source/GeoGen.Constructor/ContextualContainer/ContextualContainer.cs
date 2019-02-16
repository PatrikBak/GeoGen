using GeoGen.AnalyticGeometry;
using GeoGen.Core;
using GeoGen.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.Constructor
{
    /// <summary>
    /// The default implementation of <see cref="IContextualContainer"/>.
    /// </summary>
    public class ContextualContainer : IContextualContainer
    {
        #region Private fields

        /// <summary>
        /// The dictionary mapping objects container to the maps between geometrical objects
        /// and analytic objects (that are retrieved from the particular container).
        /// </summary>
        private Dictionary<IObjectsContainer, Map<GeometricalObject, IAnalyticObject>> _objects = new Dictionary<IObjectsContainer, Map<GeometricalObject, IAnalyticObject>>();

        /// <summary>
        /// The set of all the old points in the container.
        /// </summary>
        private readonly HashSet<PointObject> _oldPoints = new HashSet<PointObject>();

        /// <summary>
        /// The set of all the old lines in the container.
        /// </summary>
        private readonly HashSet<LineObject> _oldLines = new HashSet<LineObject>();

        /// <summary>
        /// The set of all the old circles in the container.
        /// </summary>
        private readonly HashSet<CircleObject> _oldCircles = new HashSet<CircleObject>();

        /// <summary>
        /// The set of all the new points in the container.
        /// </summary>
        private readonly HashSet<PointObject> _newPoints = new HashSet<PointObject>();

        /// <summary>
        /// The set of all the new lines in the container.
        /// </summary>
        private readonly HashSet<LineObject> _newLines = new HashSet<LineObject>();

        /// <summary>
        /// The set of all the new circles in the container.
        /// </summary>
        private readonly HashSet<CircleObject> _newCircles = new HashSet<CircleObject>();

        /// <summary>
        /// The configuration objects represented in this container.
        /// </summary>
        private readonly IReadOnlyList<ConfigurationObject> _configurationObjects;

        /// <summary>
        /// The objects container manager that holds all the representations of the objects inside this container.
        /// </summary>
        public readonly IObjectsContainersManager _manager;

        /// <summary>
        /// The tracer of unsuccessful attempts to reconstruct the container.
        /// </summary>
        private readonly IUnsuccessfulReconstructionsTracer _tracer;

        /// <summary>
        /// The settings of the container.
        /// </summary>
        private readonly ContextualContainerSettings _settings;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ContextualContainer"/> class.
        /// </summary>
        /// <param name="settings">The settings of the container.</param>
        /// <param name="objects">The objects to be present in the container.</param>
        /// <param name="manager">The manager of all the containers where our objects are supposed to be drawn.</param>
        /// <param name="tracer">The tracer of unsuccessful attempts to reconstruct the container.</param>[
        public ContextualContainer(ContextualContainerSettings settings, IReadOnlyList<ConfigurationObject> objects, IObjectsContainersManager manager, IUnsuccessfulReconstructionsTracer tracer = null)
        {
            _settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _manager = manager ?? throw new ArgumentNullException(nameof(manager));
            _configurationObjects = objects;
            _tracer = tracer;

            // Initialize the dictionary mapping containers to the object maps
            _manager.ForEach(container => _objects.Add(container, new Map<GeometricalObject, IAnalyticObject>()));

            try
            {
                // Add all objects, where only the last one is new
                _configurationObjects.ForEach((obj, index) => Add(obj, isNew: index == _configurationObjects.Count - 1));
            }
            catch (AnalyticException)
            {
                // Just in case, if there is an analytic exception, which it normally shouldn't, we will 
                // resolve it as an inconsistency. These exception are possible, because it would be very
                // hard to predict every possible outcome of the fact that the numerical system is not 
                // precise. Other exceptions would be a bigger deal and we won't hide them
                throw new InconsistentContainersException();
            }
        }

        #endregion

        #region IContextualContainer implementation

        /// <summary>
        /// Gets the geometrical objects matching a given query and casts them to a given type.
        /// </summary>
        /// <typeparam name="T">The type of objects.</typeparam>
        /// <param name="query">The query that we want to perform.</param>
        /// <returns>The queried objects.</returns>
        public IEnumerable<T> GetGeometricalObjects<T>(ContexualContainerQuery query) where T : GeometricalObject
        {
            // Prepare the result
            var result = Enumerable.Empty<GeometricalObject>();

            // If we should include points
            if (query.IncludePoints)
            {
                // Then we decide based on the objects type
                switch (query.Type)
                {
                    case ContexualContainerQuery.ObjectsType.New:
                        result = result.Concat(_newPoints);
                        break;
                    case ContexualContainerQuery.ObjectsType.Old:
                        result = result.Concat(_oldPoints);
                        break;
                    case ContexualContainerQuery.ObjectsType.All:
                        result = result.Concat(_oldPoints).Concat(_newPoints);
                        break;
                }
            }

            // If we should include lines
            if (query.IncludeLines)
            {
                // Then we decide based on the objects type
                switch (query.Type)
                {
                    case ContexualContainerQuery.ObjectsType.New:
                        result = result.Concat(_newLines);
                        break;
                    case ContexualContainerQuery.ObjectsType.Old:
                        result = result.Concat(_oldLines);
                        break;
                    case ContexualContainerQuery.ObjectsType.All:
                        result = result.Concat(_oldLines).Concat(_newLines);
                        break;
                }
            }

            // If we should include circles
            if (query.IncludeCirces)
            {
                // Then we decide based on the objects type
                switch (query.Type)
                {
                    case ContexualContainerQuery.ObjectsType.New:
                        result = result.Concat(_newCircles);
                        break;
                    case ContexualContainerQuery.ObjectsType.Old:
                        result = result.Concat(_oldCircles);
                        break;
                    case ContexualContainerQuery.ObjectsType.All:
                        result = result.Concat(_oldCircles).Concat(_newCircles);
                        break;
                }
            }

            // Return the result casted to the wanted type
            return result.Cast<T>();
        }

        /// <summary>
        /// Gets the analytic representation of a given geometrical object in a given objects container.
        /// </summary>
        /// /// <typeparam name="T">The wanted type of the analytic object.</typeparam>
        /// <param name="geometricalObject">The geometrical object.</param>
        /// <param name="objectsContainer">The objects container.</param>
        /// <returns>The analytic object represented by the given geometrical object in the given container.</returns>
        public T GetAnalyticObject<T>(GeometricalObject geometricalObject, IObjectsContainer objectsContainer) where T : IAnalyticObject
        {
            // Find the right map, pull the analytic object from it, and cast it
            return (T) _objects[objectsContainer].GetRightValue(geometricalObject);
        }

        /// <summary>
        /// Recreates the underlying analytic objects that this container maps to <see cref="GeometricalObject"/>s.
        /// This method doesn't get delete the <see cref="GeometricalObject"/>s that container already created.
        /// </summary>
        /// <param name="successful">true, if the reconstruction was successful; false otherwise.</param>
        public void TryReconstruct(out bool successful)
        {
            // Prepare a variable holding the current number of attempts
            var numberOfAttempts = 0;

            // While we are supposed to try...
            while (numberOfAttempts < _settings.MaximalNumberOfAttemptsToReconstruct)
            {
                // Mark an attempt
                numberOfAttempts++;

                try
                {
                    // Perform the reconstruction
                    Reconstruct();

                    // If we got here, we're happy
                    break;
                }
                catch (ConstructorException)
                {
                    // It might happen that it failed. This is a very rare case, 
                    // but due to the imprecision of the analytic geometry it is possible
                }
            }

            // We did it if and only if we didn't reach the maximal number of attempts
            successful = numberOfAttempts != _settings.MaximalNumberOfAttemptsToReconstruct;

            // If we made it and needed more than one attempt, trace it
            if (successful && numberOfAttempts > 1)
                _tracer?.TraceUnsucessfullAttemptsToReconstruct(this, _manager, numberOfAttempts);

            // If we weren't successful at all, trace that as well
            if (!successful)
                _tracer?.TraceReachingMaximalNumberOfAttemptsToReconstruct(this, _manager);
        }


        #endregion

        #region Adding new objects

        /// <summary>
        /// Adds a given object and all the objects it implicitly creates (lines / circles).
        /// </summary>
        /// <param name="configurationObject">The object that should be added.</param>
        /// <param name="isNew">Indicates if the object should be considered as a new one (which affects queries).</param>
        private void Add(ConfigurationObject configurationObject, bool isNew)
        {
            // Let the helper method get the geometrical object that represents this object
            var geometricalObject = GetGeometricalObject(configurationObject);

            // If this object is not null (i.e. there already is it's representation)...
            if (geometricalObject != null)
            {
                // Pull the internal configuration object
                var internalConfigurationObject = geometricalObject.ConfigurationObject;

                // If this object is not null, it means this object has already been added
                if (internalConfigurationObject != null)
                    throw new ConstructorException("An attempt to add an existing configuration object.");

                // Otherwise we can set the internal object to this one
                geometricalObject.ConfigurationObject = configurationObject;

                // And terminate, since the implicit objects this might create have already been added
                return;
            }

            // If this object isn't null, it means that this object doesn't exist 
            // Let's create it based on the configuration object's type
            switch (configurationObject.ObjectType)
            {
                case ConfigurationObjectType.Point:
                    geometricalObject = new PointObject(configurationObject);
                    break;
                case ConfigurationObjectType.Line:
                    geometricalObject = new LineObject(configurationObject);
                    break;
                case ConfigurationObjectType.Circle:
                    geometricalObject = new CircleObject(configurationObject);
                    break;
                default:
                    throw new ConstructorException("Unknown type of configuration object.");
            }

            // We add the geometrical object to all the dictionaries
            _objects.Keys.ForEach(container => _objects[container].Add(geometricalObject, container.Get(configurationObject)));

            // If it's a point
            if (configurationObject.ObjectType == ConfigurationObjectType.Point)
            {
                // Let the helper method add it as a point
                AddPoint((PointObject) geometricalObject, isNew);

                // And terminate
                return;
            }

            // Otherwise it's a line or a circle, we let another helper method add it
            AddLineOrCircle(geometricalObject, isNew);
        }

        /// <summary>
        /// Gets the geometrical object that corresponds to a given configuration object. 
        /// </summary>
        /// <param name="configurationObject">The configuration object.</param>
        /// <returns>The corresponding geometrical object, if there is any; otherwise null.</returns>
        private GeometricalObject GetGeometricalObject(ConfigurationObject configurationObject)
        {
            // NOTE: We could have just taken one container and look for the object
            // in it, but we didn't. There is a chance of inconsistency, that is 
            // extremely rare. Assume we have 2 containers, in each we have 2 points,
            // A,B. So we have the line AB in the container implicitly. Now assume
            // we should add its physical version, which was constructed in a different
            // way than the line AB. It might happen that this slight imprecision will
            // cause that the line will be found in one container, but won't be found 
            // in the other. I genuinely hope it won't happen, because that would mean 
            // our numerical system is very imprecise for our calculations. In any case
            // if it does happen, we can find it. What we wouldn't be able to find it 
            // is it happening incorrectly in all the containers. But even once it is
            // very improbable :)

            // Declare the result
            GeometricalObject result = null;

            // We're gonna check all the containers
            foreach (var container in _manager)
            {
                // Pull the analytic representation of this object. It must exist,
                // which is part of the contract of this class
                var analyticObject = container.Get(configurationObject);

                // If the analytic version of this object is not present...
                if (!_objects[container].ContainsRightKey(analyticObject))
                {
                    // And the result is already set, then we have an inconsistency
                    if (result != null)
                        throw new InconsistentContainersException();

                    // Otherwise we'll try another container
                    continue;
                }

                // But if it exists in the container, we pull its geometrical version.
                var geometricalObject = _objects[container].GetLeftValue(analyticObject);

                // If the result has been already set to something else, then we have an inconsistency
                if (result != null && !ReferenceEquals(result, geometricalObject))
                    throw new InconsistentContainersException();

                // If we're fine, we can set the result and test the next container
                result = geometricalObject;
            }

            // If we got here, then there are no inconsistencies and we can return the result
            return result;
        }

        /// <summary>
        /// Adds a given point object to the container. It handles creating all the lines
        /// that implicitly use this point and finding all the existing lines / circles
        /// that contain this point.
        /// </summary>
        /// <param name="pointObject">The point object.</param>
        /// <param name="isNew">Indicates if this point should be added to the new points set.</param>
        private void AddPoint(PointObject pointObject, bool isNew)
        {
            #region Update existing lines and circles that contain this point

            // Iterate over all the lines
            foreach (var lineObject in _oldLines.Concat(_newLines))
            {
                // If this point lies on it
                if (IsPointOnLineOrCircle(pointObject, lineObject))
                {
                    // Make sure the line knows about the point
                    lineObject.Points.Add(pointObject);

                    // Make sure the point knows about the line
                    pointObject.Lines.Add(lineObject);
                }
            }

            // Iterate over all the circles
            foreach (var circleObject in _oldCircles.Concat(_newCircles))
            {
                // If this point lies on it
                if (IsPointOnLineOrCircle(pointObject, circleObject))
                {
                    // Make sure the circle knows about the point
                    circleObject.Points.Add(pointObject);

                    // Make sure the point knows about the circle
                    pointObject.Circles.Add(circleObject);
                }
            }

            #endregion

            #region Create new lines and circles from this point and the other ones

            // Enumerate all the points so we get all the pairs of distinct ones
            var points = _oldPoints.Concat(_newPoints).ToArray();

            // Now we construct all lines that pass through our point
            // We use our helper method to do the hard work
            foreach (var point in points)
                ResolveLine(pointObject, point, isNew);


            // Now we construct all circles that pass through our point
            // We iterate twice over to obtain all the non-ordered pairs of them
            // and use our helper method to do the hard work
            for (var i = 0; i < points.Length; i++)
                for (var j = i + 1; j < points.Length; j++)
                    ResolveCircle(pointObject, points[i], points[j], isNew);

            #endregion

            // Add the point to the particular points set
            (isNew ? _newPoints : _oldPoints).Add(pointObject);
        }

        /// <summary>
        /// Tries to construct a new geometrical line using given two points. 
        /// If it doesn't exist yet, it is properly initialized.
        /// </summary>
        /// <param name="point1">The first point.</param>
        /// <param name="point2">The second point.</param>
        /// <param name="isNew">Indicates if a new line should be added to the new lines set.</param>
        private void ResolveLine(PointObject point1, PointObject point2, bool isNew)
        {
            // Initialize a map that caches created analytic representations of this line
            var containersMap = new Dictionary<IObjectsContainer, Line>();

            // Initialize a resulting line object 
            LineObject result = null;

            // Try to find or construct the line in every container
            foreach (var container in _manager)
            {
                // Pull the map between geometrical and analytic objects for this container
                var map = _objects[container];

                // Construct the analytic line from the analytic representations of the points in the map
                var analyticLine = new Line((Point) map.GetRightValue(point1), (Point) map.GetRightValue(point2));

                // Cache the result
                containersMap.Add(container, analyticLine);

                // If the line is present in the map...
                if (map.ContainsRightKey(analyticLine))
                {
                    // Then pull the corresponding geometrical line
                    var newResult = map.GetLeftValue(analyticLine);

                    // If the current result has been set and is distinct
                    // from the pulled line, then we have an inconsistency
                    if (result != null && !ReferenceEquals(result, newResult))
                        throw new InconsistentContainersException();

                    // Otherwise we can update the result
                    result = (LineObject) newResult;
                }
            }

            // If the result is not null, i.e. the line already exists, we won't do anything else
            if (result != null)
                return;

            // Otherwise we need to create a new line object that contains these two points
            // The constructor will make sure the line knows about the points
            result = new LineObject(point1, point2);

            // Make sure the points know about the line as well
            point1.Lines.Add(result);
            point2.Lines.Add(result);

            // Add the line to the particular lines set 
            (isNew ? _newLines : _oldLines).Add(result);

            // And finally we can use the cached analytic versions of the line
            // to update the objects dictionary.
            containersMap.ForEach(pair => _objects[pair.Key].Add(result, pair.Value));
        }

        /// <summary>
        /// Tries to construct a new geometrical circle using given three points. The order
        /// of the points is not important.
        /// </summary>
        /// <param name="point1">The first point.</param>
        /// <param name="point2">The second point.</param>
        /// <param name="point3">The third point.</param>
        /// <param name="isNew">Indicates if a new circle should be added to the new circles set.</param>
        private void ResolveCircle(PointObject point1, PointObject point2, PointObject point3, bool isNew)
        {
            // Initialize a map that caches created analytic representations of this circle
            var containersMap = new Dictionary<IObjectsContainer, Circle>();

            // Initialize a resulting circle
            CircleObject result = null;

            // Initialize variable indicating if the given points are collinear
            bool? collinear = null;

            // Iterate over containers
            foreach (var container in _manager)
            {
                // Pull the map between geometrical and analytic objects for this container
                var map = _objects[container];

                // Get the analytic versions of the points from the map
                var analyticPoint1 = (Point) map.GetRightValue(point1);
                var analyticPoint2 = (Point) map.GetRightValue(point2);
                var analyticPoint3 = (Point) map.GetRightValue(point3);

                // Check if they are collinear...
                var areCollinear = AnalyticHelpers.AreCollinear(analyticPoint1, analyticPoint2, analyticPoint3);

                // If we got a different result than the one set, then we have an inconsistency 
                if (collinear != null && collinear.Value != areCollinear)
                    throw new InconsistentContainersException();

                // We we're fine, we can set the collinearity result
                collinear = areCollinear;

                // If they are collinear, we can't do more
                if (areCollinear)
                    continue;

                // Otherwise we can construct the analytic circle from the points
                var analyticCircle = new Circle(analyticPoint1, analyticPoint2, analyticPoint3);

                // Cache the result
                containersMap.Add(container, analyticCircle);

                // If the circle is present in the map...
                if (map.ContainsRightKey(analyticCircle))
                {
                    // Then pull the corresponding geometrical circle
                    var newResult = map.GetLeftValue(analyticCircle);

                    // If the current result has been set and is distinct
                    // from the pulled circle, then we have an inconsistency
                    if (result != null && !ReferenceEquals(result, newResult))
                        throw new InconsistentContainersException();

                    // Otherwise we can update the result
                    result = (CircleObject) newResult;
                }
            }

            // If the result is not null, i.e. the circle already exists, we won't do anything else
            if (result != null)
                return;

            // Similarly if the points are collinear...
            if (collinear.Value)
                return;

            // Otherwise we need to create a new circle object that contains these three points
            // The constructor will make sure the circle knows about the points
            result = new CircleObject(point1, point2, point3);

            // Make sure the points know about the circle as well
            point1.Circles.Add(result);
            point2.Circles.Add(result);
            point3.Circles.Add(result);

            // Add the circle to the particular circles set 
            (isNew ? _newCircles : _oldCircles).Add(result);

            // And finally we can use the cached analytic versions of the circles
            // to update the objects dictionary.
            containersMap.ForEach(pair => _objects[pair.Key].Add(result, pair.Value));
        }

        /// <summary>
        /// Adds a given geometrical object, that is either a line or a circle, to the container.
        /// </summary>
        /// <param name="geometricalObject">The geometrical object to be added.</param>
        /// <param name="isNew">Indicates if the object should be added to the particular new set.</param>
        private void AddLineOrCircle(GeometricalObject geometricalObject, bool isNew)
        {
            // Safely cast the object to a line and a circle
            var line = geometricalObject as LineObject;
            var circle = geometricalObject as CircleObject;

            // Iterate over all the points 
            foreach (var pointObject in _oldPoints.Concat(_newPoints))
            {
                // If the current point lies on the line / circle, we want to mark it
                if (IsPointOnLineOrCircle(pointObject, geometricalObject))
                {
                    // If it's a line
                    if (line != null)
                    {
                        // Make sure the line knows about the point
                        line.Points.Add(pointObject);

                        // Make sure the point knows about the line
                        pointObject.Lines.Add(line);
                    }
                    // Or if it's a circle
                    else if (circle != null)
                    {
                        // Make sure the circle knows about the point
                        circle.Points.Add(pointObject);

                        // Make sure the point knows about the circle
                        pointObject.Circles.Add(circle);
                    }
                }
            }

            // Finally we add the object to the particular set.
            if (line != null)
                (isNew ? _newLines : _oldLines).Add(line);
            else if (circle != null)
                (isNew ? _newCircles : _oldCircles).Add(circle);
        }

        /// <summary>
        /// Finds out if a given point lies on a given geometrical object, that is either a line or a circle.
        /// </summary>
        /// <param name="pointObject">The point object to be examined.</param>
        /// <param name="geometricalObject">The geometrical line or circle.</param>
        /// <returns>true, if the point lies on the line/circle; false otherwise.</returns>
        private bool IsPointOnLineOrCircle(PointObject pointObject, GeometricalObject geometricalObject)
        {
            // Initialize a variable holding the result
            bool? result = null;

            // Iterate over containers
            foreach (var container in _manager)
            {
                // Pull the analytic representations of the point and the line/circle
                var point = (Point) _objects[container].GetRightValue(pointObject);
                var lineOrCircle = _objects[container].GetRightValue(geometricalObject);

                // Let the helper decide if the point lies on the object
                var liesOn = AnalyticHelpers.LiesOn(lineOrCircle, point);

                // If the result has been set and it differs from the currently 
                // found value, then we have an inconsistency
                if (result != null && result.Value != liesOn)
                    throw new InconsistentContainersException();

                // Otherwise we update the result
                result = liesOn;
            }

            // Finally we can return the result
            return result.Value;
        }

        #endregion

        #region Reconstructing container

        /// <summary>
        /// Performs the actual reconstruction of the container and throws an <see cref="AnalyticException"/>,
        /// if it's not successful.
        /// </summary>
        private void Reconstruct()
        {
            // Let the manager reconstruct its containers
            _manager.TryReconstructContainers();

            // Prepare a new objects map
            var newMap = new Dictionary<IObjectsContainer, Map<GeometricalObject, IAnalyticObject>>();

            // Add the containers to it
            _manager.ForEach(container => newMap.Add(container, new Map<GeometricalObject, IAnalyticObject>()));

            // Go through all the original pairs of [container, objects] map...
            _objects.ForEach(pair =>
            {
                // Get the current container
                var container = pair.Key;

                // Go through all the geometrical objects in this container
                pair.Value.Select(tuple => tuple.item1).ForEach(geometricalObject =>
                {
                    // We're going to find the current analytic representation of it
                    IAnalyticObject analyticObject;

                    // If the configuration object is set, then we can ask the container directly
                    if (geometricalObject.ConfigurationObject != null)
                        analyticObject = container.Get(geometricalObject.ConfigurationObject);
                    else
                    {
                        // Otherwise we need to reconstruct the object from points
                        var definableByPoints = (DefinableByPoints) geometricalObject;

                        // We get the needed numbers of points (which is 2 for line, 3 for circle)
                        // We could have tried get all pair and triples to be sure they make the same
                        // line/circle, but 
                        var points = definableByPoints.Points.Take(definableByPoints.NumberOfNeededPoints)
                                        // Get their analytic representations in the container
                                        .Select(point => container.Get(point.ConfigurationObject))
                                        // Cast to the right type
                                        .Cast<Point>()
                                        // Enumerate to an array
                                        .ToArray();

                        try
                        {
                            // Decide according to the object
                            switch (geometricalObject)
                            {
                                // Line case
                                case LineObject line:
                                    analyticObject = new Line(points[0], points[1]);
                                    break;

                                // Circle case
                                case CircleObject circle:
                                    analyticObject = new Circle(points[0], points[1], points[2]);
                                    break;

                                // There shouldn't be any other
                                default:
                                    throw new ConstructorException("Unknown type of geometrical object");
                            }
                        }
                        catch (AnalyticException)
                        {
                            // Since we didn't care about inconsistencies, it might happen that
                            // in the new container the points can't make a line or a circle anymore
                            // because of some very slight imprecision. This is a very rare case though.
                            throw new ConstructorException("Reconstruction of the container failed.");
                        }
                    }

                    try
                    {
                        // Finally we can add a new mapping between the objects
                        newMap[container].Add(geometricalObject, analyticObject);
                    }
                    catch (ArgumentException)
                    {
                        // Since we didn't care about inconsistencies, it might happen that
                        // an almost equal version of this object is already added  because
                        // of some very slight imprecision. This is a very rare case though.
                        throw new ConstructorException("Reconstruction of the container failed.");
                    }
                });
            });

            // Reset the objects map to the new one
            _objects = newMap;
        }

        #endregion
    }
}