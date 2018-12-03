using System.Collections.Generic;
using System.Linq;
using GeoGen.Core;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// An <see cref="ITheoremVerifier"/> for the type <see cref="TheoremType.ConcyclicPoints"/>.
    /// </summary>
    internal class ConcyclicPointsVerifier : TheoremVerifierBase
    {
        /// <summary>
        /// Finds all potencial unverified theorems wrapped in <see cref="PotentialTheorem"/> objects.
        /// </summary>
        /// <param name="container">The container from which we get the geometrical objects.</param>
        /// <returns>The outputs.</returns>
        public override IEnumerable<PotentialTheorem> FindPotencialTheorems(IContextualContainer container)
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
                        VerifierFunction = null
                    });
        }
    }
}