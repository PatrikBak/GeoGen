using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.AnalyticalGeometry;
using GeoGen.Core;
using GeoGen.Utilities;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// An <see cref="ITheoremVerifier"/> for <see cref="TheoremType.PerpendicularLines"/>.
    /// </summary>
    internal class PerpendicularLinesVerifier : TheoremVerifierBase
    {
        /// <summary>
        /// Finds all potencial unverified theorems wrapped in <see cref="PotentialTheorem"/> objects.
        /// </summary>
        /// <param name="container">The container from which we get the geometrical objects.</param>
        /// <returns>The outputs.</returns>
        public override IEnumerable<PotentialTheorem> FindPotencialTheorems(IContextualContainer container)
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
                    // Pull analytical lines representing each one
                    var analyticalLine1 = container.GetAnalyticalObject<Line>(line1, objectsContainer);
                    var analyticalLine2 = container.GetAnalyticalObject<Line>(line2, objectsContainer);

                    // Return if there are parallel
                    return analyticalLine1.IsPerpendicularTo(analyticalLine2);
                }

                // Construct the output
                yield return new PotentialTheorem
                {
                    TheoremType = Type,
                    InvolvedObjects = new[] { line1, line2 },
                    VerifierFunction = Verify
                };
            }
        }
    }
}