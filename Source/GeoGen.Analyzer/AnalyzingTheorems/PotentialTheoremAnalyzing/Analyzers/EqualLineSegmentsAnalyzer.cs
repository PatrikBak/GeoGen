using GeoGen.AnalyticGeometry;
using GeoGen.Core;
using GeoGen.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// An <see cref="IPotentialTheoremsAnalyzer"/> for <see cref="TheoremType.EqualLineSegments"/>.
    /// </summary>
    public class EqualLineSegmentsAnalyzer : PotentialTheoremsAnalyzerBase
    {
        /// <summary>
        /// Finds all potential (unverified) theorems in a given contextual container.
        /// </summary>
        /// <param name="container">The container from which we get the actual geometric objects.</param>
        /// <returns>An enumerable of found potential theorems.</returns>
        public override IEnumerable<PotentialTheorem> FindPotentialTheorems(IContextualContainer container)
        {
            // Find new points.  At least one of them must be included in every new theorem
            var newPoints = container.GetGeometricalObjects<PointObject>(new ContexualContainerQuery
            {
                Type = ContexualContainerQuery.ObjectsType.New,
                IncludePoints = true
            }).ToList();

            // Find old points. 
            var oldPoints = container.GetGeometricalObjects<PointObject>(new ContexualContainerQuery
            {
                Type = ContexualContainerQuery.ObjectsType.Old,
                IncludePoints = true
            }).ToList();

            // Find all points
            var allPoints = newPoints.Concat(oldPoints).ToList();

            // A local helper function for getting all the pairs of points
            // representing a line segment where at leasts one point is new
            IEnumerable<(PointObject point1, PointObject point2)> NewLineSegments()
            {
                // First combine the new points with themselves
                foreach (var pairOfPoints in newPoints.UnorderedPairs())
                    yield return pairOfPoints;

                // Now combine the new points with the old ones
                foreach (var newPoint in newPoints)
                    foreach (var oldPoint in oldPoints)
                        yield return (newPoint, oldPoint);
            }

            // Go through all the new line segments
            foreach (var (point1, point2) in NewLineSegments())
            {
                // Go through all the possible line segment
                foreach (var (point3, point4) in allPoints.UnorderedPairs())
                {
                    // Skip them if they are equal
                    if ((point1, point2) == (point3, point4) || (point1, point2) == (point4, point3))
                        continue;

                    // Construct the verifier function
                    bool Verify(IObjectsContainer objectsContainer)
                    {
                        // Cast the points to their analytic versions
                        var analyticPoint1 = container.GetAnalyticObject<Point>(point1, objectsContainer);
                        var analyticPoint2 = container.GetAnalyticObject<Point>(point2, objectsContainer);
                        var analyticPoint3 = container.GetAnalyticObject<Point>(point3, objectsContainer);
                        var analyticPoint4 = container.GetAnalyticObject<Point>(point4, objectsContainer);

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
}