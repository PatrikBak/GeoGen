namespace GeoGen.TheoremSorter
{
    /// <summary>
    /// Represents a factory for creating new instances of <see cref="ITheoremSorter"/>.
    /// <para>
    /// The implementation is supposed to be provided by the dependency injection management system.
    /// </para>
    /// </summary>
    public interface ITheoremSorterFactory
    {
        /// <summary>
        /// Creates a theorem sorter that gives track of at most a given number of theorems.
        /// </summary>
        /// <param name="numberOfTheorems">The maximal number of theorems a new theorem sorter should keep track of.</param>
        /// <returns>A new theorem sorter that keep track of at most a given number of theorems.</returns>
        ITheoremSorter Create(int numberOfTheorems);
    }
}