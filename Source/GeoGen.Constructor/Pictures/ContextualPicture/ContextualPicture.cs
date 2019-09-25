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
    /// 
    /// TODO: Whenever pictures are reconstructed, this becomes invalid. One solution might be
    ///       to have an event of reconstruction in <see cref="Pictures"/>, and whenever it 
    ///       happens, reconstruct this, without ruining the old objects.
    ///       
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
        private readonly Dictionary<ConfigurationObject, GeometricObject> _configurationObjectMap = new Dictionary<ConfigurationObject, GeometricObject>();

        /// <summary>
        /// The tracer of unsuccessful attempts to reconstruct the contextual picture.
        /// </summary>
        private readonly IContexualPictureConstructionFailureTracer _tracer;

        #endregion

        #region Public properties

        /// <summary>
        /// Gets the pictures that hold all the representations of the configuration.
        /// </summary>
        public Pictures Pictures { get; }

        /// <summary>
        /// Gets all lines and circles of the picture.
        /// </summary>
        public IEnumerable<DefinableByPoints> AllLinesAndCircles => AllLines.Cast<DefinableByPoints>().Concat(AllCircles);

        /// <summary>
        /// Gets all old lines and circles of the picture.
        /// </summary>
        public IEnumerable<DefinableByPoints> OldLinesAndCircles => OldLines.Cast<DefinableByPoints>().Concat(OldCircles);

        /// <summary>
        /// Gets all new lines and circles of the picture.
        /// </summary>
        public IEnumerable<DefinableByPoints> NewLinesAndCircles => NewLines.Cast<DefinableByPoints>().Concat(NewCircles);

        /// <summary>
        /// Gets all lines of the pictures.
        /// </summary>
        public IEnumerable<LineObject> AllLines => _oldLines.Concat(_newLines);

        /// <summary>
        /// Gets all circles of the pictures.
        /// </summary>
        public IEnumerable<CircleObject> AllCircles => _oldCircles.Concat(_newCircles);

        /// <summary>
        /// Gets all points of the pictures.
        /// </summary>
        public IEnumerable<PointObject> AllPoints => _oldPoints.Concat(_newPoints);

        /// <summary>
        /// Gets old points of the pictures.
        /// </summary>
        public IEnumerable<PointObject> OldPoints => _oldPoints;

        /// <summary>
        /// Gets old lines of the pictures.
        /// </summary>
        public IEnumerable<LineObject> OldLines => _oldLines;

        /// <summary>
        /// Gets old circles of the pictures.
        /// </summary>
        public IEnumerable<CircleObject> OldCircles => _oldCircles;

        /// <summary>
        /// Gets new points of the pictures.
        /// </summary>
        public IEnumerable<PointObject> NewPoints => _newPoints;

        /// <summary>
        /// Gets new lines of the pictures.
        /// </summary>
        public IEnumerable<LineObject> NewLines => _newLines;

        /// <summary>
        /// Gets new circles of the pictures.
        /// </summary>
        public IEnumerable<CircleObject> NewCircles => _newCircles;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ContextualPicture"/> class. Throws
        /// an <see cref="InconstructibleContextualPicture"/> if it cannot be done.
        /// </summary>
        /// <param name="pictures">The pictures that hold all the representations of the configuration.</param>
        /// <param name="tracer">The tracer of unsuccessful attempts to reconstruct the contextual picture.</param>
        public ContextualPicture(Pictures pictures, IContexualPictureConstructionFailureTracer tracer = null)
            : this(pictures, createEmpty: false, tracer)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContextualPicture"/> class. Throws
        /// an <see cref="InconstructibleContextualPicture"/> if it cannot be done.
        /// </summary>
        /// <param name="pictures">The pictures that hold all the representations of the configuration.</param>
        /// <param name="createEmpty">Indicates if we should leave the picture empty without adding any objects or pictures.</param>
        /// <param name="tracer">The tracer of unsuccessful attempts to reconstruct the contextual picture.</param>
        private ContextualPicture(Pictures pictures, bool createEmpty, IContexualPictureConstructionFailureTracer tracer = null)
        {
            Pictures = pictures ?? throw new ArgumentNullException(nameof(pictures));
            _tracer = tracer;

            // If we should add objects, do it
            if (!createEmpty)
            {
                // Initialize the dictionary mapping pictures to the object maps
                Pictures.ForEach(picture => _objects.Add(picture, new Map<GeometricObject, IAnalyticObject>()));

                // Add them
                AddObjects(pictures.Configuration.AllObjects);
            }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Clones the contextual picture by cloning the current one and adding only 
        /// the last object of the configuration represented in the pictures. Throws
        /// an <see cref="InconstructibleContextualPicture"/> if it cannot be done.
        /// </summary>
        /// <param name="newPictures">The pictures that should be used to construct the contextual picture.</param>
        /// <returns>The contextual picture containing this cloned picture.</returns>
        public ContextualPicture ConstructByCloning(Pictures newPictures)
        {
            // Create an empty picture
            var newPicture = new ContextualPicture(newPictures, createEmpty: true, _tracer);

            // We need to have geometric objects cloned
            // Thus we prepare a dictionary mapping old to newly created ones
            // To get all the geometric objects we take some map from the _objects
            var geometricObjectMap = _objects.First().Value.Select(pair => pair.Item1).Select(geometricObject => geometricObject switch
            {
                // Point
                PointObject point => (oldObject: geometricObject, newObject: new PointObject(point.ConfigurationObject) as GeometricObject),

                // Point
                LineObject line => (oldObject: geometricObject, newObject: new LineObject(line.ConfigurationObject)),

                // Circle
                CircleObject circle => (oldObject: geometricObject, newObject: new CircleObject(circle.ConfigurationObject)),

                // Default case
                _ => throw new ConstructorException($"Unhandled type of geometric object: {geometricObject.GetType()}")
            })
            // Wrap them in a dictionary
            .ToDictionary(pair => pair.oldObject, pair => pair.newObject);

            // Make sure the lines and circles know about its points
            // Take the pairs where we have a line/circle
            geometricObjectMap.Where(pair => pair.Key is DefinableByPoints)
                // For every such pair take the points 
                .ForEach(pair => ((DefinableByPoints)pair.Key).Points
                    // And add the corresponding point to the corresponding object
                    .ForEach(point => ((DefinableByPoints)pair.Value).AddPoint((PointObject)geometricObjectMap[point])));

            // Make sure the points know about its lines and circles
            // Take the pairs where we have a point
            geometricObjectMap.Where(pair => pair.Key is PointObject)
                // For every such pair
                .ForEach(pair =>
                {
                    // Cast the particular points for comfort
                    var oldPoint = (PointObject)pair.Key;
                    var newPoint = (PointObject)pair.Value;

                    // Add the lines / circles corresponding to the old ones to the new point
                    oldPoint.Lines.ForEach(line => newPoint.AddLine((LineObject)geometricObjectMap[line]));
                    oldPoint.Circles.ForEach(circle => newPoint.AddCircle((CircleObject)geometricObjectMap[circle]));
                });

            // Clone the configuration objects map
            _configurationObjectMap.ForEach(pair => newPicture._configurationObjectMap.Add(pair.Key, geometricObjectMap[pair.Value]));

            // Prepare a map that maps old picture instances to corresponding new ones
            var picturesMap = Pictures.ZipToDictionary(newPictures);

            // Clone the objects
            _objects.ForEach(pair =>
            {
                // Deconstruct
                var (picture, objectMap) = pair;

                // Create a new map
                var newMap = new Map<GeometricObject, IAnalyticObject>();

                // Add the new map corresponding to the picture
                newPicture._objects.Add(picturesMap[picture], newMap);

                // Fill the map
                objectMap.ForEach(tuple => newMap.Add(geometricObjectMap[tuple.Item1], tuple.Item2));
            });

            // Clone all objects while making them all old
            _oldPoints.Concat(_newPoints).ForEach(point => newPicture._oldPoints.Add((PointObject)geometricObjectMap[point]));
            _oldLines.Concat(_newLines).ForEach(line => newPicture._oldLines.Add((LineObject)geometricObjectMap[line]));
            _oldCircles.Concat(_newCircles).ForEach(circle => newPicture._oldCircles.Add((CircleObject)geometricObjectMap[circle]));

            // Add the last object of the configuration to the new picture
            newPicture.AddObjects(new[] { newPictures.Configuration.LastConstructedObject });

            // Return the picture
            return newPicture;
        }

        /// <summary>
        /// Gets the geometric object corresponding to the object that can be constructed
        /// via a given constructor.
        /// </summary>
        /// <param name="objectConstructor">The constructor for the objects in all pictures.</param>
        /// <returns>The found object; or null, if such object is not present.</returns>
        public GeometricObject GetGeometricObject(Func<IReadOnlyDictionary<Picture, IAnalyticObject>> objectConstructor)
        {
            try
            {
                // Prepare the result
                var result = default(GeometricObject);

                // Safely execute
                GeneralUtilities.TryExecute(
                    // Finding of the equivalent object
                    () => result = FindEquivalentObject(objectConstructor),
                    // While handling inconsistencies
                    // TODO: Trace
                    (InconsistentPicturesException e) => { });

                // Return the result
                return result;
            }
            catch (UnresolvedInconsistencyException e)
            {
                // If we are unable to resolve inconsistencies, we trace it
                _tracer?.TraceConstructionFailure(Pictures.Configuration, e.Message);

                // And say the picture is not constructible
                throw new InconstructibleContextualPicture($"The reconstruction of the contextual picture failed. The inner reason: {e.Message}.");
            }
        }

        /// <summary>
        /// Gets the geometric objects that corresponds to a given configuration object.
        /// </summary>
        /// <param name="configurationObject">The configuration object.</param>
        /// <returns>The corresponding geometric object.</returns>
        public GeometricObject GetGeometricObject(ConfigurationObject configurationObject) => _configurationObjectMap[configurationObject];

        /// <summary>
        /// Gets the analytic representation of a given geometric object in a given picture.
        /// </summary>
        /// <param name="geometricObject">The geometric object.</param>
        /// <param name="picture">The picture.</param>
        /// <returns>The analytic object.</returns>
        public IAnalyticObject GetAnalyticObject(GeometricObject geometricObject, Picture picture) => _objects[picture].GetRightValue(geometricObject);

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
                Pictures.ExecuteAndReconstructAtIncosistencies(
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
                        _tracer?.TraceInconsistencyWhileConstructingPicture(Pictures.Configuration, currentObject, e.Message);

                        // Reset fields
                        _objects.Values.ForEach(map => map.Clear());
                        _oldPoints.Clear();
                        _oldLines.Clear();
                        _oldCircles.Clear();
                        _newPoints.Clear();
                        _newLines.Clear();
                        _newCircles.Clear();
                        _configurationObjectMap.Clear();
                    });
            }
            catch (UnresolvedInconsistencyException e)
            {
                // If we are unable to resolve inconsistencies, we trace it
                _tracer?.TraceConstructionFailure(Pictures.Configuration, e.Message);

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
            var geometricObject = FindGeometricObject(configurationObject);

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
                _configurationObjectMap.Add(configurationObject, geometricObject);

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
            _configurationObjectMap.Add(configurationObject, geometricObject);

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
        /// Finds the geometric object that corresponds to a given configuration object. 
        /// </summary>
        /// <param name="configurationObject">The configuration object.</param>
        /// <returns>The corresponding geometric object, if there is any; otherwise null.</returns>
        private GeometricObject FindGeometricObject(ConfigurationObject configurationObject)
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
            foreach (var picture in Pictures)
            {
                // Pull the analytic representation of this object. 
                var analyticObject = picture.Get(configurationObject);

                // If the analytic version of this object is not present...
                if (!_objects[picture].ContainsRightKey(analyticObject))
                {
                    // If this is not the first picture and our results
                    // don't match, then we have an inconsistency
                    if (picture != Pictures.First() && result != null)
                        throw new InconsistentPicturesException("The geometric object corresponding to a configuration object could not be determined consistently.");

                    // Otherwise we'll try another picture
                    continue;
                }

                // But if it exists in the picture, we pull its geometric version.
                var geometricObject = _objects[picture].GetLeftValue(analyticObject);

                // If this is not the first picture and our results
                // don't match, then we have an inconsistency
                if (picture != Pictures.First() && result != geometricObject)
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
                    pointObject.AddLine(lineObject);
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
                    pointObject.AddCircle(circleObject);
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
            foreach (var picture in Pictures)
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
                    if (picture != Pictures.First() && result != newResult)
                        throw new InconsistentPicturesException("A line object couldn't be set consistently within more pictures (this should be very rare).");

                    // Otherwise we can update the result
                    result = (LineObject)newResult;
                }
                // If this is not the first picture and the result is
                // already set, then we also have an inconsistency
                else if (picture != Pictures.First() && result != null)
                    throw new InconsistentPicturesException("A line object couldn't be set consistently within more pictures (this should be very rare).");
            }

            // If the result is not null, i.e. the line already exists, we won't do anything else
            if (result != null)
                return;

            // Otherwise we need to create a new line object that contains these two points
            // The constructor will make sure the line knows about the points
            result = new LineObject(point1, point2);

            // Make sure the points know about the line as well
            point1.AddLine(result);
            point2.AddLine(result);

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
            foreach (var picture in Pictures)
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
                    if (picture != Pictures.First() && result != newResult)
                        throw new InconsistentPicturesException("A circle object couldn't be set consistently with more pictures (this should be very rare).");

                    // Otherwise we can update the result
                    result = (CircleObject)newResult;
                }
                // If this is not the first picture and the result is
                // already set, then we also have an inconsistency
                else if (picture != Pictures.First() && result != null)
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
            point1.AddCircle(result);
            point2.AddCircle(result);
            point3.AddCircle(result);

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
            foreach (var point in _oldPoints.Concat(_newPoints))
            {
                // If the current point lies on the line / circle, we want to mark it
                if (IsPointOnLineOrCircle(point, geometricObject))
                {
                    // If it's a line
                    if (line != null)
                    {
                        // Make sure the line knows about the point
                        line.AddPoint(point);

                        // Make sure the point knows about the line
                        point.AddLine(line);
                    }
                    // Or if it's a circle
                    else if (circle != null)
                    {
                        // Make sure the circle knows about the point
                        circle.AddPoint(point);

                        // Make sure the point knows about the circle
                        point.AddCircle(circle);
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
            foreach (var picture in Pictures)
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

        /// <summary>
        /// Finds the geometric object corresponding to the object that can be constructed
        /// via a given constructor. The result might be an object of the picture, but it
        /// doesn't have to be -- in that case it will, however, contain information about
        /// the objects of the picture (for example a line will know its points).
        /// </summary>
        /// <param name="objectConstructor">The constructor for the objects in all pictures.</param>
        /// <returns>The found object; or null, if such object is not present.</returns>
        private GeometricObject FindEquivalentObject(Func<IReadOnlyDictionary<Picture, IAnalyticObject>> objectConstructor)
        {
            // Perform the construction 
            var pictureToObject = objectConstructor();

            // If it cannot be done, then we can't do more
            if (pictureToObject == null)
                return null;

            // Prepare the result
            var foundObject = default(GeometricObject);

            #region Searching already created objects

            // Go through all the pairs of picture-object
            foreach (var picture in Pictures)
            {
                // Find out if this object is present
                var equalObject = _objects[picture].GetLeftValueOrDefault(pictureToObject[picture]);

                // If this is not the first picture and we've marked the found object differently
                // then we have an inconsistency
                if (picture != Pictures.First() && foundObject != equalObject)
                    throw new InconsistentPicturesException("Corresponding equal geometric object couldn't be determined consistently.");

                // Otherwise set the found object
                foundObject = equalObject;
            }

            // If we found the object, we can return it 
            if (foundObject != null)
                return foundObject;

            #endregion

            // If we couldn't find it, we need to construct it
            // We switch based on the type of object that we're dealing with
            switch (pictureToObject.First().Value)
            {
                // Line or circle
                case Line _:
                case Circle _:

                    // We need to find points that lie on this object
                    // Prepare them
                    var points = new HashSet<PointObject>();

                    // Search all the point of the picture individually
                    foreach (var point in AllPoints)
                    {
                        // Prepare the variable indicating whether the point
                        // lies on the object
                        bool? liesOn = null;

                        #region Handling pictures

                        // Iterate over pictures
                        foreach (var picture in Pictures)
                        {
                            // Pull the analytic representations of the point and the line/circle
                            var analyticPoint = (Point)_objects[picture].GetRightValue(point);
                            var lineOrCircle = pictureToObject[picture];

                            // Let the helper decide if the point lies on the object
                            var liesOnInThisPicture = AnalyticHelpers.LiesOn(lineOrCircle, analyticPoint);

                            // If the result has been set and it differs from the currently 
                            // found value, then we have an inconsistency
                            if (liesOn != null && liesOn.Value != liesOnInThisPicture)
                                throw new InconsistentPicturesException($"The fact whether a point lies on a {(lineOrCircle is Line ? "line" : "circle")} is not the same in every picture.");

                            // Otherwise we update the result
                            liesOn = liesOnInThisPicture;
                        }

                        #endregion

                        // If the point lies on our object, add it to the points set
                        if (liesOn.Value)
                            points.Add(point);
                    }

                    // Return the object based on the type
                    return pictureToObject.First().Value switch
                    {
                        // Line
                        Line _ => new LineObject(configurationObject: null, points) as GeometricObject,

                        // Circle
                        Circle _ => new CircleObject(configurationObject: null, points),

                        // Default
                        _ => throw new ConstructorException("Impossible situation")
                    };

                // Point
                case Point _:
                    throw new ConstructorException("Finding equivalent point is currently not supported.");

                default:
                    throw new ConstructorException($"Unhandled type of analytic object: {pictureToObject.First().Value.GetType()}");
            }
        }

        #endregion
    }
}