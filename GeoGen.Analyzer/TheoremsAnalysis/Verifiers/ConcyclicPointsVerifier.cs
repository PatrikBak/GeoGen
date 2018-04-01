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
        /// Gets the enumerable of verifier outputs that pulls objects from
        /// a given contextual container (that represents the configuration)
        /// </summary>
        /// <param name="container">The container.</param>
        /// <returns>The outputs.</returns>
        public override IEnumerable<VerifierOutput> GetOutput(IContextualContainer container)
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
                    // And only those that contain at least 3 points
                    .Where(line => line.Points.Count >= 4)
                    // Each of these lines represents a new theorem correct in all containers
                    .Select(line => new VerifierOutput
                    {
                        Type = Type,
                        VerifierFunction = null,
                        AlwaysTrue = true,
                        InvoldedObjects = line.Points
                    });
        }
    }
}