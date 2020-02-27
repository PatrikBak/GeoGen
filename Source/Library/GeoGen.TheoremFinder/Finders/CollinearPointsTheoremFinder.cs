using GeoGen.Constructor;
using GeoGen.Core;
using GeoGen.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.TheoremFinder
{
    /// <summary>
    /// A <see cref="ITypedTheoremFinder"/> for <see cref="TheoremType.CollinearPoints"/>.
    /// </summary>
    public class CollinearPointsTheoremFinder : TheoremFinderBase
    {
        /// <summary>
        /// Finds all theorems of the sought type that hold true in the configuration 
        /// represented by a given contextual picture.
        /// </summary>
        /// <param name="contextualPicture">The contextual picture that represents the configuration.</param>
        /// <returns>The enumerable of true theorems of the sought type in the configuration.</returns>       
        public override IEnumerable<Theorem> FindAllTheorems(ContextualPicture contextualPicture)
        {
            // Take all lines 
            return contextualPicture.AllLines
                // That have at least three points
                .Where(line => line.Points.Count >= 3)
                // For each take all triples of its points
                .SelectMany(line => line.Points.Subsets(3))
                // Get the configuration objects
                .Select(points => points.Select(point => point.ConfigurationObject).ToArray())
                // Each of these triples makes a theorem
                .Select(points => new Theorem(Type, points));
        }

        /// <summary>
        /// Finds all theorems of the sought type that hold true in the configuration 
        /// represented by a given contextual picture and in their statement use the
        /// last object of the configuration, while there is no geometrically distinct
        /// way to state them without this last object.
        /// </summary>
        /// <param name="contextualPicture">The contextual picture that represents the configuration.</param>
        /// <returns>The enumerable of true theorems of the sought type in the configuration that need the last object.</returns>   
        public override IEnumerable<Theorem> FindNewTheorems(ContextualPicture contextualPicture)
        {
            // Take the new point
            var point = contextualPicture.NewPoints.FirstOrDefault();

            // If there's none, we can't do more
            if (point == null)
                return Enumerable.Empty<Theorem>();

            // Otherwise take its lines 
            return point.Lines
                // That have at least three points
                .Where(line => line.Points.Count >= 3)
                // For each take all pairs of its points distinct from our point + append our point
                .SelectMany(line => line.Points.Where(_point => _point != point).Subsets(2).Select(subset => subset.Concat(point)))
                // Get the configuration objects
                .Select(points => points.Select(point => point.ConfigurationObject).ToArray())
                // Each of these triples makes a theorem
                .Select(points => new Theorem(Type, points));
        }
    }
}
