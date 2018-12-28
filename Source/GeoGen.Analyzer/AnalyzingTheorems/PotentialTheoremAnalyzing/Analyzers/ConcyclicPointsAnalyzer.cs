using System.Collections.Generic;
using System.Linq;
using GeoGen.Core;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// An <see cref="IPotentialTheoremsAnalyzer"/> for the type <see cref="TheoremType.ConcyclicPoints"/>.
    /// </summary>
    public class ConcyclicPointsAnalyzer : PotentialTheoremsAnalyzerBase
    {
        /// <summary>
        /// Finds all potential (unverified) theorems in a given contextual container.
        /// </summary>
        /// <param name="container">The container from which we get the actual geometric objects.</param>
        /// <returns>An enumerable of found potential theorems.</returns>
        public override IEnumerable<PotentialTheorem> FindPotentialTheorems(IContextualContainer container)
        {
            // Now we first pull new points
            return container.GetGeometricalObjects<PointObject>(new ContexualContainerQuery
                    {
                        Type = ContexualContainerQuery.ObjectsType.New,
                        IncludePoints = true,
                        IncludeLines = false,
                        IncludeCirces = false
                    })
                    // And find circles that they lie on
                    .SelectMany(point => point.Circles)
                    // Take only distinct ones
                    .Distinct()
                    // And only those that contain at least 4 points
                    .Where(circle => circle.Points.Count >= 4)
                    // Each of these circles represents a new theorem correct in all containers
                    // (thus we don't set the verifier function)
                    .Select(circle => new PotentialTheorem
                    {
                        TheoremType = Type,
                        InvolvedObjects = circle.Points,
                        VerificationFunction = _ => true
                    });
        }
    }
}