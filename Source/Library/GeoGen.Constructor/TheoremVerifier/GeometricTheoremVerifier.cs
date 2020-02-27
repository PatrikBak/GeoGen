using GeoGen.AnalyticGeometry;
using GeoGen.Core;
using GeoGen.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using static GeoGen.AnalyticGeometry.AnalyticHelpers;
using static GeoGen.Core.TheoremType;

namespace GeoGen.Constructor
{
    /// <summary>
    /// The default implementation of <see cref="IGeometricTheoremVerifier"/> that uses <see cref="IGeometryConstructor"/>
    /// to construct inner theorem objects that haven't been constructed yet.
    /// </summary>
    public class GeometricTheoremVerifier : IGeometricTheoremVerifier
    {
        #region Private fields

        /// <summary>
        /// The constructor used for construction of unknown objects of theorems.
        /// </summary>
        private readonly IGeometryConstructor _constructor;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="GeometricTheoremVerifier"/> class.
        /// </summary>
        /// <param name="constructor">The constructor used for construction of unknown objects of theorems.</param>
        public GeometricTheoremVerifier(IGeometryConstructor constructor)
        {
            _constructor = constructor ?? throw new ArgumentNullException(nameof(constructor));
        }

        #endregion

        #region IGeometricTheoremVerifier implementation

        /// <summary>
        /// Finds out if a given theorem is true in given pictures. It can also handle when the inner objects
        /// of the theorem are not yet constructed, in that case they get constructed, i.e. the pictures will be
        /// modified. If it's not wanted, the pictures have to be cloned before.
        /// </summary>
        /// <param name="pictures">The pictures where the theorem should be checked.</param>
        /// <param name="theorem">The theorem to be checked.</param>
        /// <returns>true, if the theorem can be constructed correctly and holds true in all the pictures; false otherwise.</returns>
        public bool IsTrueInAllPictures(Pictures pictures, Theorem theorem)
        {
            #region Ensure all inner objects are constructed

            // Go through the inner objects
            var objectsToConstruct = theorem.GetInnerConfigurationObjects()
                // And the object needed to construct them
                .GetDefiningObjects()
                // That are constructible
                .OfType<ConstructedConfigurationObject>()
                // That are not already constructed
                .Where(innerObject => !pictures.First().Contains(innerObject));

            // Construct all of them
            foreach (var constructedObject in objectsToConstruct)
            {
                // Safely perform
                var data = GeneralUtilities.TryExecute(
                    // The construction of the object
                    () => _constructor.Construct(pictures, constructedObject, addToPictures: true),
                    // Ignore any exception in this step (it will be solved in the next one)
                    (InconsistentPicturesException _) => { });

                // If there has been an inconsistency, we're sad and say this theorem is not true
                if (data == null)
                    return false;

                // If there is an inconstructible object, the same
                if (data.InconstructibleObject != null)
                    return false;
            }

            #endregion

            // If we got here, all needed objects are constructed. We need the theorem objects
            var theoremObjects = new Dictionary<TheoremObject, Dictionary<Picture, IAnalyticObject>>();

            #region Construct theorem objects

            // Get the inner base theorem objects, i.e. Line / Circle / Point
            // Then we will reconstruct the other i.e., LineSegment / Angle later...
            var baseTheoremObjects = theorem.InvolvedObjects
                // Each theorem object can bring more of them
                .SelectMany(theoremObject => theoremObject switch
                {
                    // If the object is already base, we return it
                    BaseTheoremObject _ => theoremObject.ToEnumerable(),

                    // If we have a line segment, then its inner objects are points
                    LineSegmentTheoremObject lineSegment => lineSegment.ObjectsSet,

                    // If we have an angle, then its inner objects are lines
                    AngleTheoremObject angle => angle.ObjectsSet,

                    // Unhandled cases
                    _ => throw new ConstructorException($"Unhandled type of {nameof(TheoremObject)}: {theoremObject.GetType()}")
                })
                // Each is base
                .Cast<BaseTheoremObject>()
                // There might be duplicates, for example when two line segments are equal
                .Distinct();

            // Construct these base theorem objects one by one
            foreach (var theoremObject in baseTheoremObjects)
            {
                // Try to construct the given one
                var constructionResult = ConstructTheoremObject(pictures, theoremObject);

                // If it failed, then the whole theorem is said to be not true
                if (constructionResult == null)
                    return false;

                // Otherwise add it to the dictionary
                theoremObjects.Add(theoremObject, constructionResult);
            }

            #endregion

            // Verify the theorem in all the pictures
            return pictures.All(picture =>
            {
                try
                {
                    // Run the verification
                    return VerifyTheorem(theorem,
                        // Function that finds an analytic representation of a theorem object
                        theoremObject => theoremObjects[theoremObject][picture],
                        // Function that finds out if an analytic object is an original object of the picture
                        analyticObject => picture.Any(pair => pair.analyticObject.Equals(analyticObject)));
                }
                catch (AnalyticException)
                {
                    // If something very weird happens, it is not a big deal, but we can't say the theorem is not true
                    return false;
                }
            });
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Constructs a given base theorem object.
        /// </summary>
        /// <param name="pictures">The pictures in which the theorem object should be constructed.</param>
        /// <param name="theoremObject">The theorem object that should be constructed.</param>
        /// <returns>The dictionary mapping each picture to the analytic representation of the object, or null, if the construction cannot be done.</returns>
        private Dictionary<Picture, IAnalyticObject> ConstructTheoremObject(Pictures pictures, BaseTheoremObject theoremObject)
        {
            // If the base object is defined explicitly
            if (theoremObject.ConfigurationObject != null)
                // Then we know it's inner configuration object is already constructed and we find it in the pictures
                return pictures.ToDictionary(picture => picture, picture => picture.Get(theoremObject.ConfigurationObject));

            // Otherwise switch based on type
            switch (theoremObject)
            {
                // If we have an object with points, i.e. line / circle...
                case TheoremObjectWithPoints objectWithPoints:

                    // Prepare the result
                    var result = new Dictionary<Picture, IAnalyticObject>();

                    // Construct the points in every picture
                    foreach (var picture in pictures)
                    {
                        // Take the points
                        var points = objectWithPoints.Points
                            // Find their analytic versions
                            .Select(picture.Get)
                            // Cast
                            .Cast<Point>()
                            // Make sure they are distinct
                            .Distinct()
                            // Enumerate
                            .ToArray();

                        // If the number of found points doesn't match the number of defining ones,
                        // then the construction couldn't be carried out
                        if (points.Length != objectWithPoints.NumberOfDefiningPoints)
                            return null;

                        // If the counts are fine, we can construct the object
                        result.Add(picture, objectWithPoints switch
                        {
                            // Line requires 2 points
                            LineTheoremObject _ => new Line(points[0], points[1]),

                            // Circle requires 3 points
                            CircleTheoremObject _ => new Circle(points[0], points[1], points[2]),

                            // Unhandled cases
                            _ => throw new ConstructorException($"Unhandled type of {nameof(TheoremObjectWithPoints)}: {objectWithPoints.GetType()}")
                        });
                    }

                    // Return the result
                    return result;

                // Unhandled cases
                default:
                    throw new ConstructorException($"Unhandled type of {nameof(TheoremObject)}: {theoremObject.GetType()}");
            }
        }

        /// <summary>
        /// Checks if a given theorem is true with respect to a given analytic object provider for particular theorem objects.
        /// </summary>
        /// <param name="theorem">The theorem to be verified.</param>
        /// <param name="analyticObjectProvider">The function that finds the analytic representation of a theorem object.</param>
        /// <param name="originalPictureObject">The function that finds out if the analytic object is part of the original picture.</param>
        /// <returns>true, if the theorem is true with respect to the analytic object provider; false otherwise.</returns>
        private bool VerifyTheorem(Theorem theorem, Func<TheoremObject, IAnalyticObject> analyticObjectProvider, Predicate<IAnalyticObject> originalPictureObject)
        {
            // Switch based on theorem type
            switch (theorem.Type)
            {
                // We should have 3 points
                case CollinearPoints:
                {
                    // Get the points by taking the theorem objects
                    var points = theorem.InvolvedObjects
                        // Find the analytic version for each
                        .Select(analyticObjectProvider.Invoke)
                        // Cast
                        .Cast<Point>()
                        // Ensure they are distinct
                        .Distinct()
                        // Enumerate
                        .ToArray();

                    // Check there are 3 of them and the collinearity holds
                    return points.Length == 3 && AreCollinear(points);
                }

                // We should have 4 points
                case ConcyclicPoints:
                {
                    // Get the points by taking the theorem objects
                    var points = theorem.InvolvedObjects
                        // Find the analytic version for each
                        .Select(analyticObjectProvider.Invoke)
                        // Cast
                        .Cast<Point>()
                        // Ensure they are distinct
                        .Distinct()
                        // Enumerate
                        .ToArray();

                    // Check there are 3 of them and the concyclity holds
                    return points.Length == 4 && AreConcyclic(points);
                }

                // We should have 3 lines
                case ConcurrentLines:
                {
                    // Get the lines by taking the theorem objects
                    var lines = theorem.InvolvedObjects
                        // Find the analytic version for each
                        .Select(analyticObjectProvider.Invoke)
                        // Cast
                        .Cast<Line>()
                        // Ensure they are distinct
                        .Distinct()
                        // Enumerate
                        .ToArray();

                    // Make sure there are three of them
                    if (lines.Length != 3)
                        return false;

                    // Interest the first two
                    var intersection = lines[0].IntersectionWith(lines[1]);

                    // Make sure the intersection exists
                    if (intersection == null)
                        return false;

                    // If the intersection exists, make sure the last point lies on the line
                    return lines[2].Contains(intersection.Value)
                        // And the intersection is not an object of the original picture
                        && !originalPictureObject(intersection);
                }

                // We should have 2 lines
                case ParallelLines:
                {
                    // Get the lines by taking the theorem objects
                    var lines = theorem.InvolvedObjects
                        // Find the analytic version for each
                        .Select(analyticObjectProvider.Invoke)
                        // Cast
                        .Cast<Line>()
                        // Ensure they are distinct
                        .Distinct()
                        // Enumerate
                        .ToArray();

                    // Make sure there are two of them and the parallelity holds
                    return lines.Length == 2 && lines[0].IsParallelTo(lines[1]);
                }

                // We should have 2 lines
                case PerpendicularLines:
                {
                    // Get the lines by taking the theorem objects
                    var lines = theorem.InvolvedObjects
                        // Find the analytic version for each
                        .Select(analyticObjectProvider.Invoke)
                        // Cast
                        .Cast<Line>()
                        // Ensure they are distinct
                        .Distinct()
                        // Enumerate
                        .ToArray();

                    // Make sure there are two of them and the perpendicularity holds
                    return lines.Length == 2 && lines[0].IsPerpendicularTo(lines[1]);
                }

                // We should have 2 circles
                case TangentCircles:
                {
                    // Get the circles by taking the theorem objects
                    var circles = theorem.InvolvedObjects
                        // Find the analytic version for each
                        .Select(analyticObjectProvider.Invoke)
                        // Cast
                        .Cast<Circle>()
                        // Ensure they are distinct
                        .Distinct()
                        // Enumerate
                        .ToArray();

                    // Make sure there are two of them and the tangency holds
                    return circles.Length == 2 && circles[0].IsTangentTo(circles[1]);
                }

                // We should have 1 line and 1 circle (in no specific order)
                case LineTangentToCircle:
                {
                    // Find the line by taking the theorem objects
                    var line = theorem.InvolvedObjects
                        // Find the analytic version 
                        .Select(analyticObjectProvider.Invoke)
                        // Take the line
                        .OfType<Line>()
                        // There should be exactly one
                        .First();

                    // Find the circle by taking the theorem objects
                    var circle = theorem.InvolvedObjects
                        // Find the analytic version 
                        .Select(analyticObjectProvider.Invoke)
                        // Take the circle
                        .OfType<Circle>()
                        // There should be exactly one
                        .First();

                    // Make sure the tangency holds
                    return circle.IsTangentTo(line);
                }

                // We should have two line segments 
                case EqualLineSegments:
                {
                    // Get the segments by taking the theorem objects
                    var segments = theorem.InvolvedObjects
                        // Cast
                        .Cast<LineSegmentTheoremObject>()
                        // Enumerate
                        .ToArray();

                    // Find the particular points
                    var segment1Point1 = (Point)analyticObjectProvider(segments[0].Object1);
                    var segment1Point2 = (Point)analyticObjectProvider(segments[0].Object2);
                    var segment2Point1 = (Point)analyticObjectProvider(segments[1].Object1);
                    var segment2Point2 = (Point)analyticObjectProvider(segments[1].Object2);

                    // Find the lengths of the segments
                    var length1 = segment1Point1.DistanceTo(segment1Point2).Rounded();
                    var length2 = segment2Point1.DistanceTo(segment2Point2).Rounded();

                    // They cannot be equal
                    return !new[] { segment1Point1, segment1Point2 }.OrderlessEquals(new[] { segment2Point1, segment2Point2 })
                        // And the lengths must match and be non-zero
                        && length1 == length2 && length1 != 0;
                }

                // We should have two angles
                case EqualAngles:
                {
                    // Get the angles by taking the theorem objects
                    var angles = theorem.InvolvedObjects
                        // Cast
                        .Cast<AngleTheoremObject>()
                        // Enumerate
                        .ToArray();

                    // Find the particular lines
                    var angle1Line1 = (Line)analyticObjectProvider(angles[0].Object1);
                    var angle1Line2 = (Line)analyticObjectProvider(angles[0].Object2);
                    var angle2Line1 = (Line)analyticObjectProvider(angles[1].Object1);
                    var angle2Line2 = (Line)analyticObjectProvider(angles[1].Object2);

                    // Find the angles between the lines
                    var angle1 = AngleBetweenLines(angle1Line1, angle1Line2).Rounded();
                    var angle2 = AngleBetweenLines(angle2Line1, angle2Line2).Rounded();

                    // They cannot be equal
                    return !new[] { angle1Line1, angle1Line2 }.OrderlessEquals(new[] { angle2Line1, angle2Line2 })
                        // And the sized of the angles must match and be non-zero
                        && angle1 == angle2 && angle1 != 0;
                }

                // We should have a point and a line or circle
                case Incidence:
                {
                    // Find the point by taking the theorem objects
                    var point = theorem.InvolvedObjects
                        // Find the analytic version 
                        .Select(analyticObjectProvider.Invoke)
                        // Take the point
                        .OfType<Point>()
                        // There should be exactly one
                        .First();

                    // Find the line / circle by taking the theorem objects
                    var otherObject = theorem.InvolvedObjects
                        // Find the analytic version 
                        .Select(analyticObjectProvider.Invoke)
                        // The one that is not the point
                        .Where(analayticObject => !analayticObject.Equals(point))
                        // There should be exactly one
                        .First();

                    // Ensure the incidence holds
                    return LiesOn(otherObject, point);
                }

                // We should have two constructible theorem objects
                case EqualObjects:
                {
                    // Get them
                    var object1 = theorem.InvolvedObjectsList[0];
                    var object2 = theorem.InvolvedObjectsList[1];

                    // Make sure their analytic version are the same
                    return analyticObjectProvider(object1).Equals(analyticObjectProvider(object2));
                }

                // Unhandled cases
                default:
                    throw new ConstructorException($"Unhandled value of {nameof(TheoremType)}: {theorem.Type}");
            }
        }

        #endregion
    }
}