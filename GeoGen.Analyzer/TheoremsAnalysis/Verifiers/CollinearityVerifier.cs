using System;
using System.Collections.Generic;
using System.Linq;
using GeoGen.Core;
using GeoGen.Utilities;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// An <see cref="ITheoremVerifier"/> for <see cref="TheoremType.CollinearPoints"/>.
    /// </summary>
    internal class CollinearityVerifier : ITheoremVerifier
    {
        /// <summary>
        /// Gets the enumerable of verifier outputs that pulls objects from
        /// a given contextual container (that represents the configuration)
        /// </summary>
        /// <param name="container">The container.</param>
        /// <returns>The outputs.</returns>
        public IEnumerable<VerifierOutput> GetOutput(IContextualContainer container)
        {
            // Get all non-physical lines
            var lineObjects = container.GetGeometricalObjects<LineObject>().ToList();

            // Iterate over lines
            foreach (var lineObject in lineObjects)
            {
                // Take points that lie on this line (there should be at least one)
                var containingPoints = lineObject.Points;

                // If there are less than 3 points, skip
                if (containingPoints.Count < 3)
                    continue;

                // Otherwise we have an always-true type of a collinearity
                // (that is true in all containers)
                yield return new VerifierOutput
                {
                    Type = TheoremType.CollinearPoints,
                    VerifierFunction = null,
                    AlwaysTrue = true,
                    InvoldedObjects = containingPoints.Cast<GeometricalObject>().ToList()
                };
            }
        }
    }
}