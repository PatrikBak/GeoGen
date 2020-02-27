namespace GeoGen.MainLauncher
{
    /// <summary>
    /// The factory for creating instances of <see cref="ITheoremWithRankingJsonLazyWriter"/>.
    /// <para>The implementation is supposed to be provided by the dependency injection management system.</para>
    /// </summary>
    public interface ITheoremWithRankingJsonLazyWriterFactory
    {
        /// <summary>
        /// Creates a lazy writer of theorems with rankings that writes to a given file.
        /// </summary>
        /// <param name="filePath">The path to the file where the writer should write.</param>
        /// <returns>A writer that writes to a given path.</returns>
        ITheoremWithRankingJsonLazyWriter Create(string filePath);
    }
}
