using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.AnalyticGeometry;
using GeoGen.Core;
using GeoGen.Utilities;

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
            // Find all new lines. At least one of them must be included in a new theorem
            var newLines = container.GetGeometricalObjects<LineObject>(new ContexualContainerQuery
            {
                Type = ContexualContainerQuery.ObjectsType.New,
                IncludePoints = false,
                IncludeLines = true,
                IncludeCirces = false
            }).ToList();

            // Find all old lines.
            var oldLines = container.GetGeometricalObjects<LineObject>(new ContexualContainerQuery
            {
                Type = ContexualContainerQuery.ObjectsType.Old,
                IncludePoints = false,
                IncludeLines = true,
                IncludeCirces = false
            }).ToList();

            // A local function for combinining all the lines so that at least one of them is new
            IEnumerable<(LineObject line1, LineObject line2)> CombineLines()
            {
                // First combine the new lines with themselves
                for (var i = 0; i < newLines.Count; i++)
                {
                    for (var j = i + 1; j < newLines.Count; j++)
                    {
                        yield return (newLines[i], newLines[j]);
                    }
                }

                // Now combine new lines with old ones
                foreach (var newLine in newLines)
                {
                    foreach (var oldLine in oldLines)
                    {
                        yield return (newLine, oldLine);
                    }
                }
            }

            // Now execute the local function to find all potential pairs of lines
            foreach (var (line1, line2) in CombineLines())
            {
                // Construct the verifier function
                bool Verify(IObjectsContainer objectsContainer)
                {
                    // Pull analytic lines representing each one
                    var analyticLine1 = container.GetAnalyticObject<Line>(line1, objectsContainer);
                    var analyticLine2 = container.GetAnalyticObject<Line>(line2, objectsContainer);

                    // Return if there are parallel
                    return analyticLine1.IsPerpendicularTo(analyticLine2);
                }

                // Construct the output
                yield return new PotentialTheorem
                {
                    TheoremType = Type,
                    InvolvedObjects = new[] { line1, line2 },
                    VerificationFunction = Verify
                };
            }
        }
    }
}