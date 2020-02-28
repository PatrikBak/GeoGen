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
            // Find the number of possible mappings of loose objects minus the identity mapping
            var numberOfAllMappings = configuration.LooseObjectsHolder.GetSymmetryMappings().Count() - 1;

            // Find the number of actual symmetry mappings 
            var numberOfSymmetryMappings = configuration.GetSymmetryMappings()
                // That preserve not just the configuration, but the theorem as well
                .Where(mapping => theorem.Equals(theorem.Remap(mapping)))
                // We want their count
                .Count();

            // Return the final result
            return numberOfSymmetryMappings switch
            {
                // No symmetry case
                _ when numberOfSymmetryMappings == 0 => 0,

                // Full symmetry case
                _ when numberOfSymmetryMappings == numberOfAllMappings => 1,

                // Any other case means some sort of partial symmetry
                _ => 0.5
            };
        }
    }
}
