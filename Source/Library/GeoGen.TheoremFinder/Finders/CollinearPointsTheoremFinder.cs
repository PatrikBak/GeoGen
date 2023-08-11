using GeoGen.Constructor;
using GeoGen.Core;
using GeoGen.Utilities;

namespace GeoGen.TheoremFinder
{
    /// <summary>
    /// A <see cref="ITypedTheoremFinder"/> for <see cref="TheoremType.CollinearPoints"/>.
    /// </summary>
    public class CollinearPointsTheoremFinder : TheoremFinderBase
    {
        /// <inheritdoc/>     
        public override IEnumerable<Theorem> FindAllTheorems(ContextualPicture contextualPicture)
            // Take all lines 
            => contextualPicture.AllLines
                // That have at least three points
                .Where(line => line.Points.Count >= 3)
                // For each take all triples of its points
                .SelectMany(line => line.Points.Subsets(3))
                // Get the configuration objects
                .Select(points => points.Select(point => point.ConfigurationObject).ToArray())
                // Each of these triples makes a theorem
                .Select(points => new Theorem(Type, points));

        /// <inheritdoc/>  
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

        /// <inheritdoc/>
        public override bool ValidateOldTheorem(ContextualPicture contextualPicture, Theorem oldTheorem)
            // No restrictions
            => true;
    }
}
