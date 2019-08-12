using GeoGen.AnalyticGeometry;
using GeoGen.Core;
using GeoGen.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.Constructor
{
    /// <summary>
    /// Represents a picture that holds <see cref="GeometricObject"/>s. This picture
    /// is responsible for creating them and mapping them between <see cref="IAnalyticObject"/>s
    /// with respect to <see cref="Picture"/>s.
    /// </summary>
    public class ContextualPicture
    {
        #region Private fields

        /// <summary>
        /// The dictionary mapping pictures to the maps between geometric objects
        /// and analytic objects (that are retrieved from the particular picture).
        /// </summary>
        private readonly Dictionary<Picture, Map<GeometricObject, IAnalyticObject>> _objects = new Dictionary<Picture, Map<GeometricObject, IAnalyticObject>>();

        /// <summary>
        /// The set of all old points (i.e. discovered before the last object was added) of the configuration.
        /// </summary>
        private readonly HashSet<PointObject> _oldPoints = new HashSet<PointObject>();

        /// <summary>
        /// The set of all old lines (i.e. discovered before the last object was added) of the configuration.
        /// </summary>
        private readonly HashSet<LineObject> _oldLines = new HashSet<LineObject>();

        /// <summary>
        /// The set of all old circles (i.e. discovered before the last object was added) of the configuration.
        /// </summary>
        private readonly HashSet<CircleObject> _oldCircles = new HashSet<CircleObject>();

        /// <summary>
        /// The set of all new points (i.e. discovered while adding the last object) of the configuration.
        /// </summary>
        private readonly HashSet<PointObject> _newPoints = new HashSet<PointObject>();

        /// <summary>
        /// The set of all new lines (i.e. discovered while adding the last object) of the configuration.
        /// </summary>
        private readonly HashSet<LineObject> _newLines = new HashSet<LineObject>();

        /// <summary>
        /// The set of all new circles (i.e. discovered while adding the last object) of the configuration.
        /// </summary>
        private readonly HashSet<CircleObject> _newCircles = new HashSet<CircleObject>();

        /// <summary>
        /// The map of configuration objects represented in this picture to their corresponding geometric objects.
        /// </summary>
        private readonly Dictionary<ConfigurationObject, GeometricObject> _configurationObjectsMap = new Dictionary<ConfigurationObject, GeometricObject>();

        /// <summary>
        /// The pictures that hold all the representations of the configuration.
        /// </summary>
        private Pictures _pictures;

        /// <summary>
        /// The tracer of unsuccessful attempts to reconstruct the contextual picture.
        /// </summary>
        private IContexualPictureConstructionFailureTracer _tracer;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ContextualPicture"/> class. Throws
        /// an <see cref="InconstructibleContextualPicture"/> if it cannot be done.
        /// </summary>
        /// <param name="pictures">The pictures that hold all the representations of the configuration.</param>
        /// <param name="tracer">The tracer of unsuccessful attempts to reconstruct the contextual picture.</param>
        public ContextualPicture(Pictures pictures, IContexualPictureConstructionFailureTracer tracer = null)
        {
            _pictures = pictures ?? throw new ArgumentNullException(nameof(pictures));
            _tracer = tracer;

            // Initialize the dictionary mapping pictures to the object maps
            _pictures.ForEach(picture => _objects.Add(picture, new Map<GeometricObject, IAnalyticObject>()));

            // Add all objects
            AddObjects(pictures.Configuration.AllObjects);
        }

        /// <summary>
        /// Initializes a new empty instance of the <see cref="ContextualPicture"/> class.
        /// </summary>
        private ContextualPicture() { }

        #endregion

        #region Public methods

        /// <summary>
        /// Clones the contextual picture by cloning the current one and adding only 
        /// the last object of the configuration represented in the pictures. Throws
        /// an <see cref="InconstructibleContextualPicture"/> if it cannot be done.
        /// </summary>
        /// <param name="newPictures">The pictures that should be used to construct the contextual picture.</param>
        /// <returns>The cloned contextual </returns>
        public ContextualPicture ConstructByCloning(Pictures newPictures)
        {
            // Create an empty picture
            var newPicture = new ContextualPicture
            {
                _pictures = newPictures,
                _tracer = _tracer
            };

            // We need to have geometric objects cloned
            // Thus we prepare a dictionary mapping old to newly created ones
            // To get all the geometric objects we take some map from the _objects
            var geometricObjectsMap = _objects.First().Value.Select(pair => pair.item1).Select(geometricObject => geometricObject switch
            {
                // Point
                PointObject point => (oldObject: geometricObject, newObject: new PointObject(point.ConfigurationObject) as GeometricObject),

                // Point
                LineObject line => (oldObject: geometricObject, newObject: new LineObject(line.ConfigurationObject, line.Points)),

                // Circle
                CircleObject circle => (oldObject: geometricObject, newObject: new CircleObject(circle.ConfigurationObject, circle.Points)),

                // Default case
                _ => throw new ConstructorException($"Unhandled type of geometric object: {geometricObject.GetType()}")
            })
            // Wrap them in a dictionary
            .ToDictionary(pair => pair.oldObject, pair => pair.newObject);

            // Clone the configuration objects map
            _configurationObjectsMap.ForEach(pair => newPicture._configurationObjectsMap.Add(pair.Key, geometricObjectsMap[pair.Value]));

            // Prepare a map that maps old picture instances to corresponding new ones
            var picturesMap = _pictures.ZipToDictionary(newPictures);

            // Clone the objects
            _objects.ForEach(pair =>
            {
                // Create a new map
                var newMap = new Map<GeometricObject, IAnalyticObject>();

                // Add the new map corresponding to the picture
                newPicture._objects.Add(picturesMap[pair.Key], newMap);

                // Fill the map
                pair.Value.ForEach(tuple => newMap.Add(geometricObjectsMap[tuple.item1], tuple.item2));
            });

            // Clone all objects while making them all old
            _oldPoints.Concat(_newPoints).ForEach(point => newPicture._oldPoints.Add((PointObject)geometricObjectsMap[point]));
            _oldLines.Concat(_newLines).ForEach(line => newPicture._oldLines.Add((LineObject)geometricObjectsMap[line]));
            _oldCircles.Concat(_newCircles).ForEach(circle => newPicture._oldCircles.Add((CircleObject)geometricObjectsMap[circle]));

            // Add the last object of the configuration to the new picture
            newPicture.AddObjects(new[] { newPictures.Configuration.LastConstructedObject });

            // Return the picture
            return newPicture;
        }

        /// <summary>
        /// Gets the geometric objects matching a given query and casts them to a given type.
        /// </summary>
        /// <typeparam name="T">The type of objects.</typeparam>
        /// <param name="query">The query that we want to perform.</param>
        /// <returns>The queried objects.</returns>
        public IEnumerable<T> GetGeometricObjects<T>(ContextualPictureQuery query) where T : GeometricObject
        {
            // Prepare the result
            var result = Enumerable.Empty<GeometricObject>();

            // If we should include points
            if (query.IncludePoints)
            {
                // Then we decide based on the objects type
                switch (query.Type)
                {
                    case ContextualPictureQuery.ObjectsType.New:
                        result = result.Concat(_newPoints);
                        break;
                    case ContextualPictureQuery.ObjectsType.Old:
                        result = result.Concat(_oldPoints);
                        break;
                    case ContextualPictureQuery.ObjectsType.All:
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
                    case ContextualPictureQuery.ObjectsType.New:
                        result = result.Concat(_newLines);
                        break;
                    case ContextualPictureQuery.ObjectsType.Old:
                        result = result.Concat(_oldLines);
                        break;
                    case ContextualPictureQuery.ObjectsType.All:
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
                    case ContextualPictureQuery.ObjectsType.New:
                        result = result.Concat(_newCircles);
                        break;
                    case ContextualPictureQuery.ObjectsType.Old:
                        result = result.Concat(_oldCircles);
                        break;
                    case ContextualPictureQuery.ObjectsType.All:
                        result = result.Concat(_oldCircles).Concat(_newCircles);
                        break;
                }
            }

            // If we should take into account some points...
            if (query.ContainingPoints != null)
            {
                // Then we take those line and circles...
                result = result.Where(o => o is DefinableByPoints lineOrCircle
                    // Whose points are superset of the points that should be contained in this object
                    && lineOrCircle.Points.Select(p => p.ConfigurationObject).ToSet().IsSupersetOf(query.ContainingPoints));
            }

            // Return the result casted to the wanted type
            return result.Cast<T>();
        }

        /// <summary>
        /// Gets the geometric objects of the requested type that corresponds to a given configuration object.
        /// </summary>
        /// <typeparam name="T">The type of the geometric object.</typeparam>
        /// <param name="configurationObject">The configuration object.</param>
        /// <returns>The corresponding geometric object.</returns>
        public T GetGeometricObject<T>(ConfigurationObject configurationObject) where T : GeometricObject => (T)_configurationObjectsMap[configurationObject];

        /// <summary>
        /// Gets the analytic representation of a given geometric object in a given picture.
        /// </summary>
        /// /// <typeparam name="T">The wanted type of the analytic object.</typeparam>
        /// <param name="geometricObject">The geometric object.</param>
        /// <param name="picture">The picture.</param>
        /// <returns>The analytic object represented by the given geometric object in the given picture.</returns>
        public T GetAnalyticObject<T>(GeometricObject geometricObject, Picture picture) where T : IAnalyticObject => (T)_objects[picture].GetRightValue(geometricObject);

        #endregion

        #region Private methods

        /// <summary>
        /// Adds all the objects to the contextual picture. Only the last object is added a new one.
        /// Throws an <see cref="InconstructibleContextualPicture"/> if it cannot be done.
        /// </summary>
        /// <param name="objects">The objects to be added.</param>
        private void AddObjects(IReadOnlyList<ConfigurationObject> objects)
        {
            // Prepare a variable holding the currently added object so we can access
            // it the inconsistency exception callback
            var currentObject = default(ConfigurationObject);

            try
            {
                // Add all objects, safely through the pictures handling inconsistencies
                _pictures.ExecuteAndReconstructAtIncosistencies(
                    // Add all the objects
                    () => objects.ForEach((configurationObject, index) =>
                    {
                        // Set that we're processing this object
                        currentObject = configurationObject;

                        // Add the current object. Only the last one is new
                        Add(configurationObject, isNew: index == objects.Count - 1);
                    }),
                    // Inconsistency handler
                    e =>
                    {
                        // Trace possible inconsistency exceptions
                        _tracer?.TraceInconsistencyWhileConstructingPicture(_pictures.Configuration, currentObject, e.Message);

                        // Reset fields
                        _objects.Values.ForEach(map => map.Clear());
                        _oldPoints.Clear();
                        _oldLines.Clear();
                        _oldCircles.Clear();
                        _newPoints.Clear();
                        _newLines.Clear();
                        _newCircles.Clear();
                        _configurationObjectsMap.Clear();
                    });
            }
            catch (UnresolvedInconsistencyException e)
            {
                // If we are unable to resolve inconsistencies, we trace it
                _tracer?.TraceConstructionFailure(_pictures.Configuration, e.Message);

                // And say the picture is not constructible
                throw new InconstructibleContextualPicture($"The construction of the contextual picture failed. The inner reason: {e.Message}.");
            }
        }

        /// <summary>
        /// Adds a given object and all the objects it implicitly creates (lines / circles).
        /// </summary>
        /// <param name="configurationObject">The object that should be added.</param>
        /// <param name="isNew">Indicates if the object should be considered as a new one (which affects queries).</param>
        private void Add(ConfigurationObject configurationObject, bool isNew)
        {
            // Let the helper method get the geometric object that represents this object
            var geometricObject = GetGeometricObject(configurationObject);

            // If this object is not null (i.e. there already is it's representation)...
            if (geometricObject != null)
            {
                // Pull the internal configuration object
                var internalConfigurationObject = geometricObject.ConfigurationObject;

                // If this object is not null, it means this object has already been added
                if (internalConfigurationObject != null)
                    throw new ConstructorException("An attempt to add an existing configuration object.");

                // Otherwise we can set the internal object to this one
                geometricObject.ConfigurationObject = configurationObject;

                // Map them
                _configurationObjectsMap.Add(configurationObject, geometricObject);

                // And terminate, since the implicit objects this might create have already been added
                return;
            }

            // If this object isn't null, it means that this object doesn't exist 
            // Let's create it based on the configuration object's type
            geometricObject = configurationObject.ObjectType switch
            {
                // Point case
                ConfigurationObjectType.Point => new PointObject(configurationObject) as GeometricObject,

                // Line case
                ConfigurationObjectType.Line => new LineObject(configurationObject),

                // Circle case
                ConfigurationObjectType.Circle => new CircleObject(configurationObject),

                // Default case
                _ => throw new ConstructorException($"Unhandled type of configuration object: {configurationObject.ObjectType}")
            };

            // We add the geometric object to all the dictionaries
            _objects.Keys.ForEach(picture => _objects[picture].Add(geometricObject, picture.Get(configurationObject)));

            // Map them
            _configurationObjectsMap.Add(configurationObject, geometricObject);

            // Switch based on the object type
            switch (configurationObject.ObjectType)
            {
                // Point
                case ConfigurationObjectType.Point:

                    // Use the helper method to do the job
                    AddPoint((PointObject)geometricObject, isNew);

                    break;

                // Line or circle
                case ConfigurationObjectType.Line:
                case ConfigurationObjectType.Circle:

                    // Use the helper method to do the job
                    AddLineOrCircle(geometricObject, isNew);

                    break;

                // Default case
                default:
                    throw new ConstructorException($"Unhandled type of configuration object: {configurationObject.ObjectType}");
            }
        }

        /// <summary>
        /// Gets the geometric object that corresponds to a given configuration object. 
        /// </summary>
        /// <param name="configurationObject">The configuration object.</param>
        /// <returns>The corresponding geometric object, if there is any; otherwise null.</returns>
        private GeometricObject GetGeometricObject(ConfigurationObject configurationObject)
        {
            // NOTE: We could have just taken one picture and look for the object
            // in it, but we didn't. There is a chance of inconsistency, that is 
            // extremely rare. Assume we have 2 pictures, in each we have 2 points,
            // A,B. So we have the line AB in the picture implicitly. Now assume
            // we should add its physical version, which was constructed in a different
            // way than the line AB. It might happen that this slight imprecision will
            // cause that the line will be found in one picture, but won't be found 
            // in the other. I genuinely hope it won't happen, because that would mean 
            // our numerical system is very imprecise for our calculations. In any case
            // if it does happen, we can find it. What we wouldn't be able to find it 
            // is it happening incorrectly in all the pictures. But even once it is
            // very improbable :)

            // Declare the result
            GeometricObject result = null;

            // We're gonna check all the pictures
            foreach (var picture in _pictures)
            {
                // Pull the analytic representation of this object. 
                var analyticObject = picture.Get(configurationObject);

                // If the analytic version of this object is not present...
                if (!_objects[picture].ContainsRightKey(analyticObject))
                {
                    // If this is not the first picture and our results
                    // don't match, then we have an inconsistency
                    if (picture != _pictures.First() && result != null)
                        throw new InconsistentPicturesException("The geometric object corresponding to a configuration object could not be determined consistently.");

                    // Otherwise we'll try another picture
                    continue;
                }

                // But if it exists in the picture, we pull its geometric version.
                var geometricObject = _objects[picture].GetLeftValue(analyticObject);

                // If this is not the first picture and our results
                // don't match, then we have an inconsistency
                if (picture != _pictures.First() && result != geometricObject)
                    throw new InconsistentPicturesException("The geometric object corresponding to a configuration object could not be determined consistently.");

                // If we're fine, we can set the result and test the next picture
                result = geometricObject;
            }

            // If we got here, then there are no inconsistencies and we can return the result
            return result;
        }

        /// <summary>
        /// Adds a given point object to the picture. It handles creating all the lines
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
                    lineObject.AddPoint(pointObject);

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
                    circleObject.AddPoint(pointObject);

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
        /// Tries to construct a new geometric line using given two points. 
        /// If it doesn't exist yet, it is properly initialized.
        /// </summary>
        /// <param name="point1">The first point.</param>
        /// <param name="point2">The second point.</param>
        /// <param name="isNew">Indicates if a new line should be added to the new lines set.</param>
        private void ResolveLine(PointObject point1, PointObject point2, bool isNew)
        {
            // Initialize a map that caches created analytic representations of this line
            var picturesMap = new Dictionary<Picture, Line>();

            // Initialize a resulting line object 
            LineObject result = null;

            // Try to find or construct the line in every picture
            foreach (var picture in _pictures)
            {
                // Pull the map between geometric and analytic objects for this picture
                var map = _objects[picture];

                // We should be able to construct a line from the analytic representations of the points in the map
                var analyticLine = default(Line);

                try
                {
                    // Let's try doing so
                    analyticLine = new Line((Point)map.GetRightValue(point1), (Point)map.GetRightValue(point2));
                }
                catch (AnalyticException)
                {
                    // If we can't do it, then we have a very rare case where two points are almost
                    // the same, which was not found out by their equals method, but we still cannot
                    // construct a line from them. We will consider this an inconsistency
                    throw new InconsistentPicturesException("Two points were evaluated distinct, but yet we weren't able to construct a line through them (this should be very rare).");
                }

                // Cache the result
                picturesMap.Add(picture, analyticLine);

                // If the line is present in the map...
                if (map.ContainsRightKey(analyticLine))
                {
                    // Then pull the corresponding geometric line
                    var newResult = map.GetLeftValue(analyticLine);

                    // If this is not the first picture and our results 
                    // don't match, then we have an inconsistency
                    if (picture != _pictures.First() && result != newResult)
                        throw new InconsistentPicturesException("A line object couldn't be set consistently within more pictures (this should be very rare).");

                    // Otherwise we can update the result
                    result = (LineObject)newResult;
                }
                // If this is not the first picture and the result is
                // already set, then we also have an inconsistency
                else if (picture != _pictures.First() && result != null)
                    throw new InconsistentPicturesException("A line object couldn't be set consistently within more pictures (this should be very rare).");
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
            picturesMap.ForEach(pair => _objects[pair.Key].Add(result, pair.Value));
        }

        /// <summary>
        /// Tries to construct a new geometric circle using given three points. The order
        /// of the points is not important.
        /// </summary>
        /// <param name="point1">The first point.</param>
        /// <param name="point2">The second point.</param>
        /// <param name="point3">The third point.</param>
        /// <param name="isNew">Indicates if a new circle should be added to the new circles set.</param>
        private void ResolveCircle(PointObject point1, PointObject point2, PointObject point3, bool isNew)
        {
            // Initialize a map that caches created analytic representations of this circle
            var picturesMap = new Dictionary<Picture, Circle>();

            // Initialize a resulting circle
            CircleObject result = null;

            // Initialize variable indicating if the given points are collinear
            bool? collinear = null;

            // Iterate over pictures
            foreach (var picture in _pictures)
            {
                // Pull the map between geometric and analytic objects for this picture
                var map = _objects[picture];

                // Get the analytic versions of the points from the map
                var analyticPoint1 = (Point)map.GetRightValue(point1);
                var analyticPoint2 = (Point)map.GetRightValue(point2);
                var analyticPoint3 = (Point)map.GetRightValue(point3);

                // Check if they are collinear...
                var areCollinear = AnalyticHelpers.AreCollinear(analyticPoint1, analyticPoint2, analyticPoint3);

                // If we got a different result than the one set, then we have an inconsistency 
                if (collinear != null && collinear.Value != areCollinear)
                    throw new InconsistentPicturesException("Three points are not collinear in every picture.");

                // We're fine, we can set the collinearity result
                collinear = areCollinear;

                // If they are collinear, we can't do more
                if (areCollinear)
                    continue;

                // Otherwise we should be able to construct the analytic circle from the points
                var analyticCircle = default(Circle);

                try
                {
                    // Let's try so
                    analyticCircle = new Circle(analyticPoint1, analyticPoint2, analyticPoint3);
                }
                catch (AnalyticException)
                {
                    // If it fails, which is a very rare case, it will be considered an inconsistency
                    throw new InconsistentPicturesException("Three points were evaluated collinear, but yet we weren't able to construct a circle through them (this is a rare case)");
                }

                // Cache the result
                picturesMap.Add(picture, analyticCircle);

                // If the circle is present in the map...
                if (map.ContainsRightKey(analyticCircle))
                {
                    // Then pull the corresponding geometric circle
                    var newResult = map.GetLeftValue(analyticCircle);

                    // If this is not the first picture and our results 
                    // don't match, then we have an inconsistency
                    if (picture != _pictures.First() && result != newResult)
                        throw new InconsistentPicturesException("A circle object couldn't be set consistently with more pictures (this should be very rare).");

                    // Otherwise we can update the result
                    result = (CircleObject)newResult;
                }
                // If this is not the first picture and the result is
                // already set, then we also have an inconsistency
                else if (picture != _pictures.First() && result != null)
                    throw new InconsistentPicturesException("A circle object couldn't be set consistently with more pictures (this should be very rare).");
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
            picturesMap.ForEach(pair => _objects[pair.Key].Add(result, pair.Value));
        }

        /// <summary>
        /// Adds a given geometric object, that is either a line or a circle, to the picture.
        /// </summary>
        /// <param name="geometricObject">The geometric object to be added.</param>
        /// <param name="isNew">Indicates if the object should be added to the particular new set.</param>
        private void AddLineOrCircle(GeometricObject geometricObject, bool isNew)
        {
            // Safely cast the object to a line and a circle
            var line = geometricObject as LineObject;
            var circle = geometricObject as CircleObject;

            // Iterate over all the points 
            foreach (var pointObject in _oldPoints.Concat(_newPoints))
            {
                // If the current point lies on the line / circle, we want to mark it
                if (IsPointOnLineOrCircle(pointObject, geometricObject))
                {
                    // If it's a line
                    if (line != null)
                    {
                        // Make sure the line knows about the point
                        line.AddPoint(pointObject);

                        // Make sure the point knows about the line
                        pointObject.Lines.Add(line);
                    }
                    // Or if it's a circle
                    else if (circle != null)
                    {
                        // Make sure the circle knows about the point
                        circle.AddPoint(pointObject);

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
        /// Finds out if a given point lies on a given geometric object, that is either a line or a circle.
        /// </summary>
        /// <param name="pointObject">The point object to be examined.</param>
        /// <param name="geometricObject">The geometric line or circle.</param>
        /// <returns>true, if the point lies on the line/circle; false otherwise.</returns>
        private bool IsPointOnLineOrCircle(PointObject pointObject, GeometricObject geometricObject)
        {
            // Initialize a variable holding the result
            bool? result = null;

            // Iterate over pictures
            foreach (var picture in _pictures)
            {
                // Pull the analytic representations of the point and the line/circle
                var point = (Point)_objects[picture].GetRightValue(pointObject);
                var lineOrCircle = _objects[picture].GetRightValue(geometricObject);

                // Let the helper decide if the point lies on the object
                var liesOn = AnalyticHelpers.LiesOn(lineOrCircle, point);

                // If the result has been set and it differs from the currently 
                // found value, then we have an inconsistency
                if (result != null && result.Value != liesOn)
                    throw new InconsistentPicturesException($"The fact whether a point lies on a {(geometricObject is LineObject ? "line" : "circle")} is not the same in every picture.");

                // Otherwise we update the result
                result = liesOn;
            }

            // Finally we can return the result
            return result.Value;
        }

        #endregion
    }
}