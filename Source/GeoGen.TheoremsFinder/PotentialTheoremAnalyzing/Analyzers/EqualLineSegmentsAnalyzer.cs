using GeoGen.AnalyticGeometry;
using GeoGen.Constructor;
using GeoGen.Core;
using GeoGen.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.TheoremsFinder
{
    /// <summary>
    /// An <see cref="IPotentialTheoremsAnalyzer"/> for <see cref="TheoremType.EqualLineSegments"/>.
    /// </summary>
    public class EqualLineSegmentsAnalyzer : PotentialTheoremsAnalyzerBase
    {
        /// <summary>
        /// Finds all potential (unverified) theorems in a given contextual picture.
        /// </summary>
        /// <param name="contextualPicture">The picture from which we get the actual geometric objects.</param>
        /// <returns>An enumerable of found potential theorems.</returns>
        public override IEnumerable<PotentialTheorem> FindPotentialTheorems(ContextualPicture contextualPicture)
        {
            // Find new points.  At least one of them must be included in every new theorem
            var newPoints = contextualPicture.GetGeometricObjects<PointObject>(new ContextualPictureQuery
            {
                Type = ContextualPictureQuery.ObjectsType.New,
                IncludePoints = true
            }).ToList();

            // Find old points. 
            var oldPoints = contextualPicture.GetGeometricObjects<PointObject>(new ContextualPictureQuery
            {
                Type = ContextualPictureQuery.ObjectsType.Old,
                IncludePoints = true
            }).ToList();

            // Find all points
            var allPoints = newPoints.Concat(oldPoints).ToList();

            // A local helper function for getting all the pairs of points
            // representing a line segment where at leasts one point is new
            IEnumerable<(PointObject, PointObject)> NewLineSegments()
            {
                // First combine the new points with themselves
                foreach (var pairOfPoints in newPoints.UnorderedPairs())
                    yield return pairOfPoints;

                // Now combine the new points with the old ones
                foreach (var newPoint in newPoints)
                    foreach (var oldPoint in oldPoints)
                        yield return (newPoint, oldPoint);
            }

            // A local helper function for getting all the pairs of 
            // line segments where at least one contains a where point
            IEnumerable<((PointObject, PointObject), (PointObject, PointObject))> NewPairsOfLineSegments()
            {
                // First enumerate the new line segments
                var newLineSegments = NewLineSegments().ToList();

                // Now enumerate the old line segments
                var oldLineSegments = oldPoints.UnorderedPairs().ToList();

                // Now we can combine the new line segments with themselves
                foreach (var pairOfLineSegments in newLineSegments.UnorderedPairs())
                    yield return pairOfLineSegments;

                // And the new line segments with the old ones
                foreach (var newLineSegment in newLineSegments)
                    foreach (var oldLineSegment in oldLineSegments)
                        yield return (newLineSegment, oldLineSegment);
            }

            // Go through all the possible combinations
            foreach (var ((point1, point2), (point3, point4)) in NewPairsOfLineSegments())
            {
                // Construct the verifier function
                bool Verify(Picture picture)
                {
                    // Cast the points to their analytic versions
                    var analyticPoint1 = contextualPicture.GetAnalyticObject<Point>(point1, picture);
                    var analyticPoint2 = contextualPicture.GetAnalyticObject<Point>(point2, picture);
                    var analyticPoint3 = contextualPicture.GetAnalyticObject<Point>(point3, picture);
                    var analyticPoint4 = contextualPicture.GetAnalyticObject<Point>(point4, picture);

                    // Return if their lengths match
                    return analyticPoint1.DistanceTo(analyticPoint2).Rounded() == analyticPoint3.DistanceTo(analyticPoint4).Rounded();
                }

                // Lazily return the output
                yield return new PotentialTheorem
                {
                    // Set the type using the base property
                    TheoremType = Type,

                    // Set the function
                    VerificationFunction = Verify,

                    // Set the involved objects to our four line segment points 
                    InvolvedObjects = new[] { point1, point2, point3, point4 }
                };
            }
        }
    }
}