using GeoGen.Core;
using GeoGen.Utilities;
using System.Collections.Generic;
using System.Linq;

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
            // Take the new points. At least one of the involved points must be new
            return container.GetGeometricalObjects<PointObject>(new ContexualContainerQuery
            {
                Type = ContexualContainerQuery.ObjectsType.New,
                IncludePoints = true,
            })
            // And find all the circles that pass through it
            .SelectMany(point => point.Circles)
            // Take only those that contain at least 4 points
            .Where(circle => circle.Points.Count >= 4)
            // And are distinct 
            .Distinct()
            // For each of them their quadruples of their points
            .SelectMany(circle => circle.Points.Subsets(4))
            // Each represents a theorem (not even potential, the contextual container made sure it's true)
            .Select(quadruple => new PotentialTheorem
            {
                // Set the type using the base property
                TheoremType = Type,

                // Set the verifier function to a constant function returning always true
                VerificationFunction = _ => true,

                // Set the involved objects to the these triple of points
                InvolvedObjects = quadruple
            });
        }
    }
}