using GeoGen.Core;
using GeoGen.TheoremSorter;

namespace GeoGen.MainLauncher
{
    /// <summary>
    /// Represents a service that maps <see cref="TheoremType"/>s and <see cref="ITheoremSorter"/>s. The idea is
    /// to have separate sorters for individual types.
    /// </summary>
    public interface ITheoremSorterTypeResolver
    {
        /// <summary>
        /// Gets all created sorters with the type of theorems associated with them.
        /// </summary>
        IEnumerable<(TheoremType type, ITheoremSorter sorter)> AllSorters { get; }

        /// <summary>
        /// Gets a created or creates a new theorem sorted for theorems of a given type.
        /// </summary>
        /// <param name="theoremType">The type of theorems.</param>
        /// <returns>The sorter of theorems intended to be used for the given type.</returns>
        ITheoremSorter GetSorterForType(TheoremType theoremType);
    }
}