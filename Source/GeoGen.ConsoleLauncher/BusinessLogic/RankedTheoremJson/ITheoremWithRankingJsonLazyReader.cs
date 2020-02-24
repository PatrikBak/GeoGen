using System.Collections.Generic;

namespace GeoGen.ConsoleLauncher
{
    /// <summary>
    /// Represents a reader of <see cref="TheoremWithRanking"/> objects that reads them lazily.
    /// </summary>
    public interface ITheoremWithRankingJsonLazyReader
    {
        /// <summary>
        /// Lazily reads a given file where <see cref="TheoremWithRanking"/> objects have been written.
        /// </summary>
        /// <param name="filePath">The path to the file with theorems with ranking.</param>
        /// <returns>An enumerable of read theorems with ranking.</returns>
        IEnumerable<TheoremWithRanking> Read(string filePath);
    }
}
