namespace GeoGen.TheoremRanker.RankedTheoremIO
{
    /// <summary>
    /// Represents a reader of <see cref="RankedTheorem"/> objects that reads them lazily.
    /// </summary>
    public interface IRankedTheoremJsonLazyReader
    {
        /// <summary>
        /// Lazily reads a given file where <see cref="RankedTheorem"/> objects have been written.
        /// </summary>
        /// <param name="filePath">The path to the file with ranked theorems.</param>
        /// <returns>An enumerable of read ranked theorems.</returns>
        IEnumerable<RankedTheorem> Read(string filePath);
    }
}
