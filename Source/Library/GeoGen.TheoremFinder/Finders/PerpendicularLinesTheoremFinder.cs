using GeoGen.AnalyticGeometry;
using GeoGen.Constructor;
using GeoGen.Core;
using GeoGen.Utilities;

namespace GeoGen.TheoremFinder
{
    /// <summary>
    /// A <see cref="ITypedTheoremFinder"/> for <see cref="TheoremType.PerpendicularLines"/>.
    /// </summary>
    public class PerpendicularLinesTheoremFinder : TrueInAllPicturesTheoremFinder
    {
        /// <inheritdoc/>
        protected override IEnumerable<GeometricObject[]> GetAllOptions(ContextualPicture contextualPicture)
            // Get all pairs of lines
            => contextualPicture.AllLines.Subsets(2);

        /// <inheritdoc/>
        protected override IEnumerable<GeometricObject[]> GetNewOptions(ContextualPicture contextualPicture)
        {
            // Find new lines
            var newLines = contextualPicture.NewLines.ToList();

            // Find old lines
            var oldLines = contextualPicture.OldLines.ToList();

            // Combine the new lines with themselves
            foreach (var (newLine1, newLine2) in newLines.UnorderedPairs())
                yield return new[] { newLine1, newLine2 };

            // Combine the new lines with the old ones
            foreach (var newLine in newLines)
                foreach (var oldLine in oldLines)
                    yield return new[] { newLine, oldLine };
        }

        /// <inheritdoc/>
        protected override bool IsTrue(IAnalyticObject[] objects)
            // Return if they are perpendicular to each other
            => ((Line)objects[0]).IsPerpendicularTo((Line)objects[1]);

        /// <inheritdoc/>
        public override bool ValidateOldTheorem(ContextualPicture contextualPicture, Theorem oldTheorem)
            // No restrictions
            => true;
    }
}
