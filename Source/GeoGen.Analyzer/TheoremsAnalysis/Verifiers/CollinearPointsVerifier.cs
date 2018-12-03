using System.Collections.Generic;
using System.Linq;
using GeoGen.Core;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// An <see cref="ITheoremVerifier"/> for the type <see cref="TheoremType.CollinearPoints"/>.
    /// </summary>
    internal class CollinearPointsVerifier : TheoremVerifierBase
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
                    // And find lines that they lie on
                    .SelectMany(point => point.Lines)
                    // Take only distinct ones
                    .Distinct()
                    // And only those that contain at least 3 points
                    .Where(line => line.Points.Count >= 3)
                    // Each of these lines represents a new theorem correct in all the containers 
                    // (thus we don't set the verifier function)
                    .Select(line => new PotentialTheorem
                    {
                        TheoremType = Type,
                        InvolvedObjects = line.Points,
                        VerifierFunction = null
                    });
        }
    }
}