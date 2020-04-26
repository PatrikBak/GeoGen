using GeoGen.TheoremRanker;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.TheoremSorter
{
    /// <summary>
    /// The extensions for <see cref="ITheoremSorter"/>.
    /// </summary>
    public static class TheoremSorterExtensions
    {
        /// <summary>
        /// Enumerates the <see cref="ITheoremSorter.BestTheorems"/> in such a way that each inner
        /// configuration has exactly one ranked theorem.
        /// </summary>
        /// <param name="sorter">The theorem sorter.</param>
        /// <returns>The best theorems where every two have different configurations.</returns>
        public static IEnumerable<RankedTheorem> BestTheoremsOnePerConfiguration(this ITheoremSorter sorter)
            // Take the best theorems
            => sorter.BestTheorems
                // Group by the configuration
                .GroupBy(rankedTheorem => rankedTheorem.Configuration)
                // Take one from each group
                .Select(group => group.First());
    }
}