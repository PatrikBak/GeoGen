using GeoGen.Algorithm;
using System.Collections.Generic;

namespace GeoGen.ConsoleLauncher
{
    /// <summary>
    /// Extension methods for <see cref="ITheoremWithRankingJsonLazyWriter"/>.
    /// </summary>
    public static class ITheoremWithRankingJsonLazyWriterExtensions
    {
        /// <summary>
        /// Writes all theorems with ranking. The difference between this and 
        /// <see cref="ITheoremWithRankingJsonLazyWriter.Write(IEnumerable{TheoremWithRanking})"/> is that
        /// it doesn't require calling <see cref="ITheoremWithRankingJsonLazyWriter.BeginWriting"/> or
        /// <see cref="ITheoremWithRankingJsonLazyWriter.EndWriting"/>. 
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="theoremsWithRanking">The theorems with ranking to be written.</param>
        public static void WriteEagerly(this ITheoremWithRankingJsonLazyWriter writer, IEnumerable<TheoremWithRanking> theoremsWithRanking)
        {
            // Begin writing
            writer.BeginWriting();

            // Write theorems
            writer.Write(theoremsWithRanking);

            // End writing
            writer.EndWriting();
        }
    }
}
