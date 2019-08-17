using GeoGen.Constructor;
using GeoGen.Core;
using GeoGen.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.TheoremsFinder
{
    /// <summary>
    /// A <see cref="ITheoremsFinder"/> for <see cref="TheoremType.CollinearPoints"/>.
    /// </summary>
    public class CollinearPointsTheoremsFinder : TheoremsFinderBase
    {
        /// <summary>
        /// Finds all theorems that hold true in the configuration represented by a given
        /// contextual picture.
        /// </summary>
        /// <param name="contextualPicture">The contextual picture that represents the configuration.</param>
        /// <returns>The enumerable of true theorems in the configuration.</returns>
        public override IEnumerable<Theorem> FindAllTheorems(ContextualPicture contextualPicture)
        {
            // Take all lines 
            return contextualPicture.AllLines
                // That have at least three points
                .Where(line => line.Points.Count >= 3)
                // For each take all triples of its points
                .SelectMany(line => line.Points.Subsets(3))
                // Each of these triples makes a theorem
                .Select(points => ToTheorem(contextualPicture.Pictures.Configuration, points));
        }

        /// <summary>
        /// Finds all theorems that hold true in the configuration represented by a given
        /// contextual picture.
        /// </summary>
        /// <param name="contextualPicture">The contextual picture that represents the configuration.</param>
        /// <returns>The enumerable of true theorems in the configuration.</returns>
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
                // Each of these triples makes a theorem
                .Select(points => ToTheorem(contextualPicture.Pictures.Configuration, points));
        }
    }
}
