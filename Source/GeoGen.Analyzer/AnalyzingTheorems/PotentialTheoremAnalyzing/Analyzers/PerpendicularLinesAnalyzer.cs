using GeoGen.AnalyticGeometry;
using GeoGen.Core;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// An <see cref="IPotentialTheoremsAnalyzer"/> for <see cref="TheoremType.PerpendicularLines"/>.
    /// </summary>
    public class PerpendicularLinesAnalyzer : PotentialTheoremsAnalyzerBase
    {
        /// <summary>
        /// Finds all potential (unverified) theorems in a given contextual container.
        /// </summary>
        /// <param name="container">The container from which we get the actual geometric objects.</param>
        /// <returns>An enumerable of found potential theorems.</returns>
        public override IEnumerable<PotentialTheorem> FindPotentialTheorems(IContextualContainer container)
        {
            // Find new lines. At least one of them must be included in every new theorem
            var newLines = container.GetGeometricalObjects<LineObject>(new ContexualContainerQuery
            {
                Type = ContexualContainerQuery.ObjectsType.New,
                IncludeLines = true
            }).ToList();

            // Find all lines.
            var allLines = container.GetGeometricalObjects<LineObject>(new ContexualContainerQuery
            {
                Type = ContexualContainerQuery.ObjectsType.All,
                IncludeLines = true
            }).ToList();

            // Go through all the new lines
            foreach (var newLine in newLines)
            {
                // Go through all the lines
                foreach (var anyLine in allLines)
                {
                    // If we happen to have equal ones, skip them
                    if (newLine == anyLine)
                        continue;

                    // Construct the verifier function
                    bool Verify(IObjectsContainer objectsContainer)
                    {
                        // Cast the lines to their analytic versions
                        var analyticLine1 = container.GetAnalyticObject<Line>(newLine, objectsContainer);
                        var analyticLine2 = container.GetAnalyticObject<Line>(anyLine, objectsContainer);

                        // Return if there are perpendicular
                        return analyticLine1.IsPerpendicularTo(analyticLine2);
                    }

                    // Lazily return the output
                    yield return new PotentialTheorem
                    {
                        // Set the type using the base property
                        TheoremType = Type,

                        // Set the function
                        VerificationFunction = Verify,

                        // Set the involved objects to our two lines
                        InvolvedObjects = new[] { newLine, anyLine }
                    };
                }
            }
        }
    }
}