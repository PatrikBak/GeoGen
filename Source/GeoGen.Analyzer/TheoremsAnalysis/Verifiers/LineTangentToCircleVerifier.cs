using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.AnalyticalGeometry;
using GeoGen.Core;
using GeoGen.Utilities;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// An <see cref="ITheoremVerifier"/> for <see cref="TheoremType.LineTangentToCircle"/>.
    /// </summary>
    internal class LineTangentToCircleVerifier : TheoremVerifierBase
    {
        /// <summary>
        /// Finds all potencial unverified theorems wrapped in <see cref="PotentialTheorem"/> objects.
        /// </summary>
        /// <param name="container">The container from which we get the geometrical objects.</param>
        /// <returns>The outputs.</returns>
        public override IEnumerable<PotentialTheorem> FindPotencialTheorems(IContextualContainer container)
        {
            // Find all new circles. 
            var newCircles = container.GetGeometricalObjects<CircleObject>(new ContexualContainerQuery
            {
                Type = ContexualContainerQuery.ObjectsType.New,
                IncludePoints = false,
                IncludeLines = false,
                IncludeCirces = true
            }).ToList();

            // Find all circles.
            var allCircles = container.GetGeometricalObjects<CircleObject>(new ContexualContainerQuery
            {
                Type = ContexualContainerQuery.ObjectsType.All,
                IncludePoints = false,
                IncludeLines = false,
                IncludeCirces = true
            }).ToList();

            // Find all new lines. 
            var newLines = container.GetGeometricalObjects<LineObject>(new ContexualContainerQuery
            {
                Type = ContexualContainerQuery.ObjectsType.New,
                IncludePoints = false,
                IncludeLines = true,
                IncludeCirces = false
            }).ToList();

            // Find all lines.
            var allLines = container.GetGeometricalObjects<LineObject>(new ContexualContainerQuery
            {
                Type = ContexualContainerQuery.ObjectsType.All,
                IncludePoints = false,
                IncludeLines = true,
                IncludeCirces = false
            }).ToList();

            // A local function for combinining pairs consisting of one line and one circle
            // where at least one of them is new
            IEnumerable<(LineObject line, CircleObject circle)> CombineLineAndCircle()
            {
                // First combine the new lines with all circles
                foreach (var newLine in newLines)
                {
                    foreach (var circle in allCircles)
                    {
                        yield return (newLine, circle);
                    }
                }

                // First combine the new circles with all lines
                foreach (var newCircle in newCircles)
                {
                    foreach (var line in allLines)
                    {
                        yield return (line, newCircle);
                    }
                }
            }

            // Now execute the local function to find all potential pairs of lines and circles
            foreach (var (line, circle) in CombineLineAndCircle())
            {
                // Construct the verifier function
                bool Verify(IObjectsContainer objectsContainer)
                {
                    // Pull analytical circles representing each one
                    var analyticalLine = container.GetAnalyticalObject<Line>(line, objectsContainer);
                    var analyticalCircle = container.GetAnalyticalObject<Circle>(circle, objectsContainer);

                    // Return if there are tangent to each other
                    return analyticalCircle.IsTangentTo(analyticalLine);
                }

                // Construct the output
                yield return new PotentialTheorem
                {
                    TheoremType = Type,
                    InvolvedObjects = new GeometricalObject[] { line, circle },
                    VerifierFunction = Verify
                };
            }
        }
    }
}