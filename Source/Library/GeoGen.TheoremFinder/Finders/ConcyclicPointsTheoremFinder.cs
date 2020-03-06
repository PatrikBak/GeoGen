using GeoGen.Constructor;
using GeoGen.Core;
using GeoGen.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.TheoremFinder
{
    /// <summary>
    /// A <see cref="ITypedTheoremFinder"/> for <see cref="TheoremType.ConcyclicPoints"/>.
    /// </summary>
    public class ConcyclicPointsTheoremFinder : TheoremFinderBase
    {
        /// <inheritdoc/>   
        public override IEnumerable<Theorem> FindAllTheorems(ContextualPicture contextualPicture)
            // Take all circles 
            => contextualPicture.AllCircles
                // That have at least four points
                .Where(circle => circle.Points.Count >= 4)
                // For each take all quadruples of its points
                .SelectMany(circle => circle.Points.Subsets(4))
                // Get the configuration objects
                .Select(points => points.Select(point => point.ConfigurationObject).ToArray())
                // Each of these quadruples makes a theorem
                .Select(points => new Theorem(Type, points));

        /// <inheritdoc/>
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
                // Get the configuration objects
                .Select(points => points.Select(point => point.ConfigurationObject).ToArray())
                // Each of these quadruples makes a theorem
                .Select(points => new Theorem(Type, points));
        }

        /// <inheritdoc/>
        public override bool ValidateOldTheorem(ContextualPicture contextualPicture, Theorem oldTheorem)
            // No restrictions
            => true;
    }
}
