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

            // If there is no symmetry mapping for the layout, then 0 seems 
            // like a good ranking
            if (allSymmetryMappingsCount == 0)
                return 0;

            // Otherwise find the number of actual symmetry mappings that 
            // preserve both configuration and theorem
            var validSymmetryMappingsCount = theorem.GetSymmetryMappings(configuration).Count();

            // The symmetry ranking is the percentage of valid mappings
            return (double)validSymmetryMappingsCount / allSymmetryMappingsCount;
        }
    }
}
