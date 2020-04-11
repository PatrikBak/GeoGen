using GeoGen.Core;
using System.Linq;

namespace GeoGen.TheoremRanker
{
    /// <summary>
    /// The <see cref="IAspectTheoremRanker"/> of <see cref="RankedAspect.Symmetry"/>.
    /// </summary>
    public class SymmetryRanker : AspectTheoremRankerBase
    {
        /// <inheritdoc/>
        public override double Rank(Theorem theorem, Configuration configuration, TheoremMap allTheorems)
        {
            // Find the number of possible symmetry mappings of loose objects
            var allSymmetryMappingsCount = configuration.LooseObjectsHolder.GetSymmetricMappings().Count();

            // Find the number of actual symmetry mappings that preserve both configuration and theorem
            var validSymmetryMappingsCount = theorem.GetSymmetryMappings(configuration).Count();

            // Return the final result based on how many of possible mappings are valid
            return validSymmetryMappingsCount switch
            {
                // No symmetry case
                0 => 0,

                // Full symmetry case
                _ when validSymmetryMappingsCount == allSymmetryMappingsCount => 1,

                // Any other case means some sort of partial symmetry
                _ => 0.5
            };
        }
    }
}
