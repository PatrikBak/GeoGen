using GeoGen.Algorithm;
using System.Collections.Generic;

namespace GeoGen.ConsoleLauncher
{
    /// <summary>
    /// Extension methods for <see cref="ITheoremsWithRankingJsonLazyWriter"/>.
    /// </summary>
    public static class ITheoremsWithRankingJsonLazyWriterExtensions
    {
        /// <summary>
        /// Writes all theorems with ranking. The difference between this and 
        /// <see cref="ITheoremsWithRankingJsonLazyWriter.Write(IEnumerable{TheoremWithRanking})"/> is that
        /// it doesn't require calling <see cref="ITheoremsWithRankingJsonLazyWriter.BeginWriting"/> or
        /// <see cref="ITheoremsWithRankingJsonLazyWriter.EndWriting"/>. 
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="theoremsWithRanking">The theorems with ranking to be written.</param>
        public static void WriteEagerly(this ITheoremsWithRankingJsonLazyWriter writer, IEnumerable<TheoremWithRanking> theoremsWithRanking)
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
