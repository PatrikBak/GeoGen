using GeoGen.AnalyticGeometry;
using GeoGen.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core;

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
            // Find new points. 
            var newPoints = container.GetGeometricalObjects<PointObject>(new ContexualContainerQuery
            {
                Type = ContexualContainerQuery.ObjectsType.New,
                IncludePoints = true,
                IncludeLines = false,
                IncludeCirces = false
            }).ToList();

            // Find old points. 
            var oldPoints = container.GetGeometricalObjects<PointObject>(new ContexualContainerQuery
            {
                Type = ContexualContainerQuery.ObjectsType.Old,
                IncludePoints = true,
                IncludeLines = false,
                IncludeCirces = false
            }).ToList();

            // Local function to enumerate all line segments containing at least one new point
            IEnumerable<(PointObject point1, PointObject point2)> LineSegments()
            {
                // First combine the new points with themselves
                for (var i = 0; i < newPoints.Count; i++)
                {
                    for (var j = i + 1; j < newPoints.Count; j++)
                    {
                        yield return (newPoints[i], newPoints[j]);
                    }
                }

                // Now combine new points with old ones
                foreach (var newPoint in newPoints)
                {
                    foreach (var oldPoint in oldPoints)
                    {
                        yield return (newPoint, oldPoint);
                    }
                }
            }

            // Local function that calculates the distance between two points
            // with respect to a passed container
            RoundedDouble DistanceBetween((PointObject point1, PointObject point2) lineSegment, IObjectsContainer objectsContainer)
            {
                // Pull analytial versions of this points
                var analyticPoint1 = container.GetAnalyticObject<Point>(lineSegment.point1, objectsContainer);
                var analyticPoint2 = container.GetAnalyticObject<Point>(lineSegment.point2, objectsContainer);

                // Return their distance
                return (RoundedDouble)analyticPoint1.DistanceTo(analyticPoint2);
            }

            // Prepare a dictionary mapping line segments lengths to lists of line segments with this length
            // This is enumerator with respect to some of the containers. 
            var distancesMap = new Dictionary<RoundedDouble, List<(PointObject point1, PointObject point2)>>();

            // Pull the first container acorrding to which we're going to fill the map
            var firstContainer = container.Manager.First();

            // Enumerate all valid line segments using our local function
            foreach (var lineSegment in LineSegments())
            {
                // Find their distance in the first container
                var distance = DistanceBetween(lineSegment, firstContainer);

                // Find the right list of line segments with this distance
                var listOfSegments = distancesMap.GetOrAdd(distance, () => new List<(PointObject point1, PointObject point2)>());

                // Add the line segment to the list
                listOfSegments.Add(lineSegment);
            }

            // Now we're interested only in those lists that have at least 2 elements (i.e. 
            // there are at least two line segments with the same length). We will take into
            // account onle those ones with respect to the other containers
            var lineSegmentsLists = distancesMap.Values.Where(list => list.Count >= 2);
            
            // For each of them
            foreach (var lineSegments in lineSegmentsLists)
            {
                // We'll take all pairs of these lines segments...
                foreach (var pairOfLineSegments in lineSegments.Subsets(2))
                {
                    // Enumerate the pair
                    var pair = pairOfLineSegments.ToArray();

                    // Construct the verifier function 
                    bool Verify(IObjectsContainer objectsContainer)
                    {
                        // If the container is the first one, we're sure the objects are fine
                        if (ReferenceEquals(firstContainer, objectsContainer))
                            return true;

                        // Otherwise we check if their sizes are equal in this container
                        return DistanceBetween(pair[0], objectsContainer) == DistanceBetween(pair[1], objectsContainer);
                    }

                    // Construct the output
                    yield return new PotentialTheorem
                    {
                        TheoremType = Type,
                        InvolvedObjects = new [] { pair[0].point1, pair[0].point2, pair[1].point1, pair[1].point2 },
                        VerificationFunction = Verify
                    };
                }
            }
        }
    }
}