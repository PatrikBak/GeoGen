using GeoGen.AnalyticGeometry;
using GeoGen.Core;
using GeoGen.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// An <see cref="IPotentialTheoremsAnalyzer"/> for <see cref="TheoremType.EqualAngles"/>.
    /// </summary>
    public class EqualAnglesAnalyzer : PotentialTheoremsAnalyzerBase
    {
        /// <summary>
        /// Finds all potential (unverified) theorems in a given contextual container.
        /// </summary>
        /// <param name="container">The container from which we get the actual geometric objects.</param>
        /// <returns>An enumerable of found potential theorems.</returns>
        public override IEnumerable<PotentialTheorem> FindPotentialTheorems(IContextualContainer container)
        {
            // Find new lines.  At least one of them must be included in every new theorem
            var newLines = container.GetGeometricalObjects<LineObject>(new ContexualContainerQuery
            {
                Type = ContexualContainerQuery.ObjectsType.New,
                IncludeLines = true
            }).ToList();

            // Find old lines. 
            var oldLines = container.GetGeometricalObjects<LineObject>(new ContexualContainerQuery
            {
                Type = ContexualContainerQuery.ObjectsType.Old,
                IncludeLines = true
            }).ToList();

            // A local helper function for getting all the pairs of line
            // representing an angle where at leasts one line is new
            IEnumerable<(LineObject line1, LineObject line2)> NewAngles()
            {
                // First combine the new lines with themselves
                foreach (var pairOfLines in newLines.UnorderedPairs())
                    yield return pairOfLines;

                // Now combine the new lines with the old ones
                foreach (var newLine in newLines)
                    foreach (var oldLine in oldLines)
                        yield return (newLine, oldLine);
            }

            // A local helper function for getting all the pairs of 
            // angles where at least one contains a new line
            IEnumerable<((LineObject, LineObject), (LineObject, LineObject))> NewPairsOfAngles()
            {
                // First enumerate the new angles
                var newAngles = NewAngles().ToList();

                // Now enumerate the old line angles
                var oldAngles = oldLines.UnorderedPairs().ToList();

                // Now we can combine the new angles with themselves
                foreach (var pairOfNewAngles in newAngles.UnorderedPairs())
                    yield return pairOfNewAngles;

                // And the new angles with the old ones
                foreach (var newAngle in newAngles)
                    foreach (var oldAngle in oldAngles)
                        yield return (newAngle, oldAngle);
            }

            // Go through all the possible combinations
            foreach (var ((line1, line2), (line3, line4)) in NewPairsOfAngles())
            {
                // Construct the verifier function
                bool Verify(IObjectsContainer objectsContainer)
                {
                    // Cast the lines to their analytic versions
                    var analyticLine1 = container.GetAnalyticObject<Line>(line1, objectsContainer);
                    var analyticLine2 = container.GetAnalyticObject<Line>(line3, objectsContainer);
                    var analyticLine3 = container.GetAnalyticObject<Line>(line3, objectsContainer);
                    var analyticLine4 = container.GetAnalyticObject<Line>(line4, objectsContainer);

                    // Return if the angles between them match
                    return analyticLine1.AngleBetween(analyticLine2).Rounded() == analyticLine3.AngleBetween(analyticLine4).Rounded();
                }

                // Lazily return the output
                yield return new PotentialTheorem
                {
                    // Set the type using the base property
                    TheoremType = Type,

                    // Set the function
                    VerificationFunction = Verify,

                    // Set the involved objects to our four angle lines
                    InvolvedObjects = new[] { line1, line2, line3, line4 }
                };
            }
        }
    }
}