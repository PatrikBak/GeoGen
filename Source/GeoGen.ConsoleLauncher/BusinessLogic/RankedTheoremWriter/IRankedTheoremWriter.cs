using GeoGen.Algorithm;
using System.Collections.Generic;

namespace GeoGen.ConsoleLauncher
{
    /// <summary>
    /// Represents a writer of <see cref="TheoremWithRanking"/> to a file.
    /// </summary>
    public interface IRankedTheoremWriter
    {
        /// <summary>
        /// Writes given ranked theorems to a file.
        /// </summary>
        /// <param name="theorems">The enumerable of ranked theorems with their ids to be written to the file.</param>
        /// <param name="theoremFilePath">The path to the file where the theorems should be written. </param>
        void WriteTheorems(IEnumerable<(TheoremWithRanking rankedTheorem, string id)> theorems, string theoremFilePath);
    }
}