using System.Collections.Generic;

namespace GeoGen.MainLauncher
{
    /// <summary>
    /// Represents a tracker of best <see cref="RankedTheorem"/>s that sorts them by their ranking.
    /// </summary>
    public interface IBestTheoremFinder
    {
        /// <summary>
        /// The best theorems that currently have been found.
        /// </summary>
        IEnumerable<RankedTheorem> BestTheorems { get; }

        /// <summary>
        /// Gives given theorems for the finder to judge them.
        /// </summary>
        /// <param name="theorems">The theorems to be examined.</param>
        /// <param name="bestTheoremsChanged">Indicates whether <see cref="BestTheorems"/> has changed after adding all theorems.</param>
        void AddTheorems(IEnumerable<RankedTheorem> theorems, out bool bestTheoremsChanged);
    }
}