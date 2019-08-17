using GeoGen.Constructor;
using GeoGen.Core;
using GeoGen.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.TheoremsFinder
{
    /// <summary>
    /// A <see cref="ITheoremsFinder"/> for <see cref="TheoremType.ConcyclicPoints"/>.
    /// </summary>
    public class ConcyclicPointsTheoremsFinder : TheoremsFinderBase
    {
        /// <summary>
        /// Finds all theorems that hold true in the configuration represented by a given
        /// contextual picture.
        /// </summary>
        /// <param name="contextualPicture">The contextual picture that represents the configuration.</param>
        /// <returns>The enumerable of true theorems in the configuration.</returns>
        public override IEnumerable<Theorem> FindAllTheorems(ContextualPicture contextualPicture)
        {
            // Take all circles 
            return contextualPicture.AllCircles
                // That have at least four points
                .Where(circle => circle.Points.Count >= 4)
                // For each take all quadruples of its points
                .SelectMany(circle => circle.Points.Subsets(4))
                // Each of them makes a theorem
                .Select(points => ToTheorem(contextualPicture.Pictures.Configuration, points));
        }

        /// <summary>
        /// Finds all theorems that hold true in the configuration represented by a given 
        /// contextual picture and in their statement use the last object of the configuration.
        /// </summary>
        /// <param name="contextualPicture">The contextual picture that represents the configuration.</param>
        /// <returns>The enumerable of true theorems in the configuration that use the last object.</returns>
        public override IEnumerable<Theorem> FindNewTheorems(ContextualPicture contextualPicture)
        {
            // Take the new point
            var point = contextualPicture.NewPoints.FirstOrDefault();

            // If there's none, we can't do more
            if (point == null)
                return Enumerable.Empty<Theorem>();

            // Otherwise take its circles 
            return point.Circles
                // That have at least 4 points
                .Where(circle => circle.Points.Count >= 4)
                // For each take all triples of its points distinct from our point + append our point
                .SelectMany(circle => circle.Points.Where(_point => _point != point).Subsets(3).Select(subset => subset.Concat(point)))
                // Each of these quadruples makes a theorem
                .Select(points => ToTheorem(contextualPicture.Pictures.Configuration, points));
        }
    }
}
