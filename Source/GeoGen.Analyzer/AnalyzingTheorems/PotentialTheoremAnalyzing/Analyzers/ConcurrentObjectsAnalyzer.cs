using GeoGen.AnalyticGeometry;
using GeoGen.Core;
using GeoGen.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// An <see cref="IPotentialTheoremsAnalyzer"/> for <see cref="TheoremType.ConcurrentObjects"/>.
    /// </summary>
    public class ConcurrentObjectsAnalyzer : PotentialTheoremsAnalyzerBase
    {
        /// <summary>
        /// Finds all potential (unverified) theorems in a given contextual container.
        /// </summary>
        /// <param name="container">The container from which we get the actual geometric objects.</param>
        /// <returns>An enumerable of found potential theorems.</returns>
        public override IEnumerable<PotentialTheorem> FindPotentialTheorems(IContextualContainer container)
        {
            // Find new lines / circles. At least one of them must be included in every new theorem
            var newLinesCircles = container.GetGeometricalObjects<GeometricalObject>(new ContexualContainerQuery
            {
                Type = ContexualContainerQuery.ObjectsType.New,
                IncludeLines = true,
                IncludeCirces = true
            }).ToList();

            // Find all lines / circles. 
            var allLinesCircles = container.GetGeometricalObjects<GeometricalObject>(new ContexualContainerQuery
            {
                Type = ContexualContainerQuery.ObjectsType.All,
                IncludeLines = true,
                IncludeCirces = true
            }).ToList();

            // Go through all the new objects
            foreach (var newObject in newLinesCircles)
            {
                // Go through all the pairs of all the objects
                foreach (var (anyObject1, anyObject2) in allLinesCircles.UnorderedPairs())
                {
                    // The new object might be in this pair. Skip those
                    if (newObject == anyObject1 || newObject == anyObject2)
                        continue;

                    // Construct the involved objects 
                    var involvedObjects = new[] { newObject, anyObject1, anyObject2 };

                    // Construct the verifier function
                    bool Verify(IObjectsContainer objectsContainer)
                    {
                        // Cast the objects to their analytic versions
                        var analyticObjects = involvedObjects.Select(lineOrCircle => container.GetAnalyticObject<AnalyticObject>(lineOrCircle, objectsContainer));

                        // Let the helper function intersection them
                        var intersections = AnalyticHelpers.Intersect(analyticObjects);

                        // Return true if and only if there is an intersection that doesn't exist in this container
                        // Thanks to that we can be sure that we don't return very trivial theorems stating that
                        // three objects are concurrent at some point that is part of the definition of each of them
                        return intersections.Any(intersection => !objectsContainer.Contains(intersection));
                    };

                    // Lazily return the output
                    yield return new PotentialTheorem
                    {
                        // Set the type using the base property
                        TheoremType = Type,

                        // Set the function
                        VerificationFunction = Verify,

                        // Set the involved objects
                        InvolvedObjects = involvedObjects
                    };
                }
            }
        }
    }
}