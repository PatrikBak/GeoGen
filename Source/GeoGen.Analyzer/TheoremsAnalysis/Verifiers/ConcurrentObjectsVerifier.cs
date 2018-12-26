using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.AnalyticGeometry;
using GeoGen.Core;
using GeoGen.Utilities;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// An <see cref="ITheoremVerifier"/> for <see cref="TheoremType.ConcurrentObjects"/>.
    /// </summary>
    public class ConcurrentObjectsVerifier : TheoremVerifierBase
    {
        /// <summary>
        /// Finds all potencial unverified theorems wrapped in <see cref="PotentialTheorem"/> objects.
        /// </summary>
        /// <param name="container">The container from which we get the geometrical objects.</param>
        /// <returns>The outputs.</returns>
        public override IEnumerable<PotentialTheorem> FindPotencialTheorems(IContextualContainer container)
        {
            // Find new lines / circles. At least one of them must be included in a new theorem
            var newLinesCircles = container.GetGeometricalObjects<GeometricalObject>(new ContexualContainerQuery
            {
                Type = ContexualContainerQuery.ObjectsType.New,
                IncludePoints = false,
                IncludeLines = true,
                IncludeCirces = true
            }).ToList();

            // Find old lines / circles. 
            var oldLinesCircles = container.GetGeometricalObjects<GeometricalObject>(new ContexualContainerQuery
            {
                Type = ContexualContainerQuery.ObjectsType.Old,
                IncludePoints = false,
                IncludeLines = true,
                IncludeCirces = true
            }).ToList();

            // Local function to enumerate all pairs of objects that are going to be intersected
            IEnumerable<(GeometricalObject object1, GeometricalObject object2)> PairsOfObjects()
            {
                // First combine the new objects with themselves
                for (var i = 0; i < newLinesCircles.Count; i++)
                {
                    for (var j = i + 1; j < newLinesCircles.Count; j++)
                    {
                        yield return (newLinesCircles[0], newLinesCircles[1]);
                    }
                }

                // Now combine the new objects with old ones
                foreach (var newObject in newLinesCircles)
                {
                    foreach (var oldObject in oldLinesCircles)
                    {
                        yield return (newObject, oldObject);
                    }
                }
            }

            // Local function that finds an intersection of two geometrical objects 
            // that are not present in a given container
            HashSet<Point> FindNewIntersections((GeometricalObject object1, GeometricalObject object2) objects, IObjectsContainer objectsContainer)
            {
                // Pull analytial versions of these objects
                var analyticObject1 = container.GetAnalyticObject<AnalyticObject>(objects.object1, objectsContainer);
                var analyticObject2 = container.GetAnalyticObject<AnalyticObject>(objects.object2, objectsContainer);

                // Let the helper method intersect them
                return AnalyticHelpers.Intersect(analyticObject1, analyticObject2)
                    // And pick the ones that are not in the container
                    .Where(point => !objectsContainer.Contains(point))
                    // And cast the result to the set
                    .ToSet();
            }

            // Prepare a dictionary mapping points to sets of lines and circles passing through them
            // This is enumerator with respect to some of the containers. 
            var intersectionsMap = new Dictionary<Point, HashSet<GeometricalObject>>();

            // Pull the first container acorrding to which we're going to fill the map
            var firstContainer = container.Manager.First();

            // Enumerate all valid pairs of lines using our local function
            foreach (var objects in PairsOfObjects())
            {
                // Intersect them
                var intersections = FindNewIntersections(objects, firstContainer);

                // Map each intersection
                foreach (var intersection in intersections)
                {
                    // Get or create the set of passing passing through this point
                    var passingObjects = intersectionsMap.GetOrAdd(intersection, () => new HashSet<GeometricalObject>());

                    // Add the involved objects
                    passingObjects.Add(objects.object1, objects.object2);
                }
            }

            // Now we're interested only in those sets that have at least 3 elements (i.e. 
            // there are at least three lines/circles passing through some points) We will take 
            // into account onle those ones with respect to the other containers
            var objectsLists = intersectionsMap.Values.Where(set => set.Count >= 3).Select(set => set.ToList());

            // For each of them
            foreach (var objectList in objectsLists)
            {
                // We'll take all triples of these objects
                foreach (var triple in objectList.Subsets(3))
                {
                    // Enumerate involved objects
                    var involvedObjects = triple.ToArray();

                    // Construct the verifier function 
                    bool Verify(IObjectsContainer objectsContainer)
                    {
                        // If the container is the first one, we're sure the objects are fine
                        if (ReferenceEquals(firstContainer, objectsContainer))
                            return true;

                        // Cast the objects to their analytic versions
                        var analyticObjects = involvedObjects.Select(lineOrCircle => container.GetAnalyticObject<AnalyticObject>(lineOrCircle, objectsContainer));

                        // Let the helper function intersection them
                        var intersections = AnalyticHelpers.Intersect(analyticObjects);

                        // Return true if and only if there is an intersection
                        return intersections.Any();                            
                    }

                    // Construct the output
                    yield return new PotentialTheorem
                    {
                        TheoremType = Type,
                        InvolvedObjects = involvedObjects,
                        VerifierFunction = Verify
                    };
                }
            }
        }
    }
}