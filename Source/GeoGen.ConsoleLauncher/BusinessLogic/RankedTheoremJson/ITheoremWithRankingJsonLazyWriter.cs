using System.Collections.Generic;

namespace GeoGen.ConsoleLauncher
{
    /// <summary>
    /// Represents a writer of <see cref="TheoremWithRanking"/> objects that writes them lazily.
    /// </summary>
    public interface ITheoremWithRankingJsonLazyWriter
    {
        /// <summary>
        /// Begins lazy writing.
        /// </summary>
        void BeginWriting();

        /// <summary>
        /// Writes a given theorem with ranking.
        /// </summary>
        /// <param name="theoremsWithRanking">The theorems ranking to be written.</param>
        void Write(IEnumerable<TheoremWithRanking> theoremsWithRanking);

        /// <summary>
        /// Finishes lazy writing.
        /// </summary>
        void EndWriting();
    }
}
