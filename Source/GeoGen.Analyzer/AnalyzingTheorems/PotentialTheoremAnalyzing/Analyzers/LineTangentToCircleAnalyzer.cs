using GeoGen.AnalyticGeometry;
using GeoGen.Core;
using System.Collections.Generic;
using System.Linq;

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

            // Find all circles.
            var allCircles = container.GetGeometricalObjects<CircleObject>(new ContexualContainerQuery
            {
                Type = ContexualContainerQuery.ObjectsType.All,
                IncludeCirces = true
            }).ToList();

            // Find old lines.
            var oldLines = container.GetGeometricalObjects<LineObject>(new ContexualContainerQuery
            {
                Type = ContexualContainerQuery.ObjectsType.Old,
                IncludeLines = true,
            }).ToList();

            // A local helper function for combining pairs consisting of
            // one line and one circle where at least one of them is new
            IEnumerable<(LineObject, CircleObject)> CombineLinesWithCircles()
            {
                // First combine the new lines with all the circles
                foreach (var newLine in newLines)
                    foreach (var anyCircle in allCircles)
                        yield return (newLine, anyCircle);

                // Then combine the new circles with just the old lines
                foreach (var newCircle in newCircles)
                    foreach (var oldLine in oldLines)
                        yield return (oldLine, newCircle);
            }

            // Go through all the possible combinations
            foreach (var (line, circle) in CombineLinesWithCircles())
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

                // Lazily return the output
                yield return new PotentialTheorem
                {
                    // Set the type using the base property
                    TheoremType = Type,

                    // Set the function
                    VerificationFunction = Verify,

                    // Set the involved objects to our line and circle
                    InvolvedObjects = new GeometricalObject[] { line, circle },
                };
            }
        }
    }
}