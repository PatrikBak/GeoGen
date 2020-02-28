using System.Collections.Generic;

namespace GeoGen.MainLauncher
{
    /// <summary>
    /// Represents a writer of <see cref="RankedTheorem"/> objects that writes them lazily.
    /// </summary>
    public interface IRankedTheoremJsonLazyWriter
    {
        /// <summary>
        /// Begins lazy writing.
        /// </summary>
        void BeginWriting();

        /// <summary>
        /// Writes a given ranked theorem.
        /// </summary>
        /// <param name="rankedTheorems">The ranked theorems to be written.</param>
        void Write(IEnumerable<RankedTheorem> rankedTheorems);

        /// <summary>
        /// Finishes lazy writing.
        /// </summary>
        void EndWriting();
    }
}
