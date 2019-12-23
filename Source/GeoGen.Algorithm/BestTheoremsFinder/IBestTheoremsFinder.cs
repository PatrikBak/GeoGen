using System.Collections.Generic;

namespace GeoGen.Algorithm
{
    /// <summary>
    /// Represents a tracker of best <see cref="TheoremWithRanking"/>s. This service is supposed to 
    /// perform the final merging algorithm of all interesting theorems to make sure the final output
    /// doesn't contain duplicates.
    /// </summary>
    public interface IBestTheoremsFinder
    {
        /// <summary>
        /// The best theorems that currently have been found.
        /// </summary>
        IEnumerable<TheoremWithRanking> BestTheorems { get; }

        /// <summary>
        /// Gives given theorems for the finder to judge them.
        /// </summary>
        /// <param name="theorems">The theorems to be examined.</param>
        /// <param name="bestTheoremsChanged">Indicates whether <see cref="BestTheorems"/> has changed after adding all theorems.</param>
        void AddTheorems(IEnumerable<TheoremWithRanking> theorems, out bool bestTheoremsChanged);
    }
}