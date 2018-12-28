using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.AnalyticGeometry;
using GeoGen.Core;
using GeoGen.Utilities;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// An <see cref="IPotentialTheoremsAnalyzer"/> for <see cref="TheoremType.LineTangentToCircle"/>.
    /// </summary>
    public class LineTangentToCircleAnalyzer : PotentialTheoremsAnalyzerBase
    {
        /// <summary>
        /// Finds all potential (unverified) theorems in a given contextual container.
        /// </summary>
        /// <param name="container">The container from which we get the actual geometric objects.</param>
        /// <returns>An enumerable of found potential theorems.</returns>
        public override IEnumerable<PotentialTheorem> FindPotentialTheorems(IContextualContainer container)
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
                    // Pull analytic circles representing each one
                    var analyticLine = container.GetAnalyticObject<Line>(line, objectsContainer);
                    var analyticCircle = container.GetAnalyticObject<Circle>(circle, objectsContainer);

                    // Return if there are tangent to each other
                    return analyticCircle.IsTangentTo(analyticLine);
                }

                // Construct the output
                yield return new PotentialTheorem
                {
                    TheoremType = Type,
                    InvolvedObjects = new GeometricalObject[] { line, circle },
                    VerificationFunction = Verify
                };
            }
        }
    }
}