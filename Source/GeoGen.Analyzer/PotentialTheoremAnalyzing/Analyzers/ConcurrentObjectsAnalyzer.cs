using GeoGen.AnalyticGeometry;
using GeoGen.Core;
using GeoGen.GeometryRegistrar;
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
            // Find new circles. Either a new line or a new circle must be included in every new theorem
            var newCircles = container.GetGeometricalObjects<CircleObject>(new ContexualContainerQuery
            {
                Type = ContexualContainerQuery.ObjectsType.New,
                IncludeCirces = true
            }).ToList();

            // Find new lines. Either a new line or a new circle must be included in every new theorem
            var newLines = container.GetGeometricalObjects<LineObject>(new ContexualContainerQuery
            {
                Type = ContexualContainerQuery.ObjectsType.New,
                IncludeLines = true,
            }).ToList();

            // Find old circles.
            var oldCircles = container.GetGeometricalObjects<CircleObject>(new ContexualContainerQuery
            {
                Type = ContexualContainerQuery.ObjectsType.Old,
                IncludeCirces = true
            }).ToList();

            // Find old lines.
            var oldLines = container.GetGeometricalObjects<LineObject>(new ContexualContainerQuery
            {
                Type = ContexualContainerQuery.ObjectsType.Old,
                IncludeLines = true,
            }).ToList();

            // A local helper function for combining triples consisting of
            // lines and circles, where at least one is new
            IEnumerable<(GeometricalObject, GeometricalObject, GeometricalObject)> NewTriplesOfObjects()
            {
                #region 3 lines

                // 3 new lines
                foreach (var newLinesTriple in newLines.UnorderedTriples())
                    yield return newLinesTriple;

                // 2 new lines, 1 old
                foreach (var (newLine1, newLine2) in newLines.UnorderedPairs())
                    foreach (var oldLine in oldLines)
                        yield return (newLine1, newLine2, oldLine);

                // 1 new line, 2 old
                foreach (var newLine in newLines)
                    foreach (var (oldLine1, oldLine2) in oldLines.UnorderedPairs())
                        yield return (newLine, oldLine1, oldLine2);

                #endregion

                #region 3 circles

                // 3 new circles
                foreach (var newCirclesTriple in newCircles.UnorderedTriples())
                    yield return newCirclesTriple;

                // 2 new circles, 1 old
                foreach (var (newCircle1, newCircle2) in newCircles.UnorderedPairs())
                    foreach (var oldCircle in oldCircles)
                        yield return (newCircle1, newCircle2, oldCircle);

                // 1 new circle, 2 old
                foreach (var newCircle in newCircles)
                    foreach (var (oldCircle1, oldCircle2) in oldLines.UnorderedPairs())
                        yield return (newCircle, oldCircle1, oldCircle2);

                #endregion

                #region 2 lines and 1 circle

                // 2 new lines, 1 any circle
                foreach (var (newLine1, newLine2) in newLines.UnorderedPairs())
                    foreach (var anyCircle in oldCircles.Concat(newCircles))
                        yield return (newLine1, newLine2, anyCircle);

                // 1 new line, 1 old line, 1 any circle
                foreach (var newLine in newLines)
                    foreach (var oldLine in oldLines)
                        foreach (var anyCircle in oldCircles.Concat(newCircles))
                            yield return (newLine, oldLine, anyCircle);

                // 2 old lines, 1 new circle
                foreach (var (oldLine1, oldLine2) in oldLines.UnorderedPairs())
                    foreach (var newCircle in newCircles)
                        yield return (newCircle, oldLine1, oldLine2);

                #endregion

                #region 1 line and 2 circles

                // 2 new circles, 1 any line
                foreach (var (newCircle1, newCircle2) in newCircles.UnorderedPairs())
                    foreach (var anyLine in oldLines.Concat(newLines))
                        yield return (newCircle1, newCircle2, anyLine);

                // 1 new circle, 1 old circle, 1 any line
                foreach (var newCircle in newCircles)
                    foreach (var oldCircle in oldCircles)
                        foreach (var anyLine in oldLines.Concat(newLines))
                            yield return (newCircle, oldCircle, anyLine);

                // 2 old circles, 1 new line
                foreach (var (oldCircle1, oldCircle2) in oldCircles.UnorderedPairs())
                    foreach (var newLine in newLines)
                        yield return (newLine, oldCircle1, oldCircle2);

                #endregion
            }

            // Go through all the possible combinations
            foreach (var (object1, object2, object3) in NewTriplesOfObjects())
            {
                // Construct the verifier function
                bool Verify(IObjectsContainer objectsContainer)
                {
                    // Cast the objects to their analytic versions
                    var analyticObject1 = container.GetAnalyticObject<IAnalyticObject>(object1, objectsContainer);
                    var analyticObject2 = container.GetAnalyticObject<IAnalyticObject>(object2, objectsContainer);
                    var analyticObject3 = container.GetAnalyticObject<IAnalyticObject>(object3, objectsContainer);

                    // Let the helper function intersection them
                    var intersections = AnalyticHelpers.Intersect(analyticObject1, analyticObject2, analyticObject3);

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

                    // Set the involved objects to our 3 objects
                    InvolvedObjects = new[] { object1, object2, object3 }
                };
            }
        }
    }
}