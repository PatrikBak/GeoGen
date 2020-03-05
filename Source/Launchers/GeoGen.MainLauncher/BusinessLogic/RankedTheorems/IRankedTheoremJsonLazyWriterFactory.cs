namespace GeoGen.MainLauncher
{
    /// <summary>
    /// The factory for creating instances of <see cref="IRankedTheoremJsonLazyWriter"/>.
    /// <para>The implementation is supposed to be provided by the dependency injection management system.</para>
    /// </summary>
    public interface IRankedTheoremJsonLazyWriterFactory
    {
        /// <summary>
        /// Creates a lazy writer of ranked theorems that writes to a given file.
        /// </summary>
        /// <param name="filePath">The path to the file where the writer should write.</param>
        /// <returns>A writer that writes to a given path.</returns>
        IRankedTheoremJsonLazyWriter Create(string filePath);
    }
}
