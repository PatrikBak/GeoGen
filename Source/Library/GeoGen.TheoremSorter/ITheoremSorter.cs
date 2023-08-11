using GeoGen.TheoremRanker;

namespace GeoGen.TheoremSorter
{
    /// <summary>
    /// Represents a tracker of best <see cref="RankedTheorem"/>s that sorts them by their ranking
    /// and ensures no theorem is tracked in any form more than once.
    /// </summary>
    public interface ITheoremSorter
    {
        /// <summary>
        /// The best theorems that currently have been found.
        /// </summary>
        IEnumerable<RankedTheorem> BestTheorems { get; }

        /// <summary>
        /// Adds given best theorems to the sorter so it can judge whether they are better than the ones
        /// which it currently keeps track of.
        /// </summary>
        /// <param name="rankedTheorems">The theorems to be added.</param>
        /// <param name="bestTheoremsChanged">Indicates whether <see cref="BestTheorems"/> has changed after adding all theorems.</param>
        void AddTheorems(IEnumerable<RankedTheorem> rankedTheorems, out bool bestTheoremsChanged);
    }
}