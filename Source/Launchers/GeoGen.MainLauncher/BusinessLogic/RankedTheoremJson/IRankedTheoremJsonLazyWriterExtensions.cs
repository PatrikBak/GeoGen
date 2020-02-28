using System.Collections.Generic;

namespace GeoGen.MainLauncher
{
    /// <summary>
    /// Extension methods for <see cref="IRankedTheoremJsonLazyWriter"/>.
    /// </summary>
    public static class IRankedTheoremJsonLazyWriterExtensions
    {
        /// <summary>
        /// Writes all ranked theorems. The difference between this and <see cref="IRankedTheoremJsonLazyWriter.Write(IEnumerable{RankedTheorem})"/>
        /// is that it doesn't require calling <see cref="IRankedTheoremJsonLazyWriter.BeginWriting"/> and <see cref="IRankedTheoremJsonLazyWriter.EndWriting"/>. 
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="rankedTheorems">The ranked theorems to be written.</param>
        public static void WriteEagerly(this IRankedTheoremJsonLazyWriter writer, IEnumerable<RankedTheorem> rankedTheorems)
        {
            // Begin writing
            writer.BeginWriting();

            // Write theorems
            writer.Write(rankedTheorems);

            // End writing
            writer.EndWriting();
        }
    }
}
