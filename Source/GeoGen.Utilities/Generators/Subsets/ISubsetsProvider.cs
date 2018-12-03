using System.Collections.Generic;

namespace GeoGen.Utilities
{
    /// <summary>
    /// Represents a generator of all possible subsets from a given list.
    /// </summary>
    public interface ISubsetsProvider
    {
        /// <summary>
        /// Generates all possible subsets of elements from a given list, with
        /// a given number of elements.
        /// </summary>
        /// <typeparam name="T">The type of elements within the list.</typeparam>
        /// <param name="list">The list of elements.</param>
        /// <param name="numberOfElements">The number of elements of each generated subset.</param>
        /// <returns>The enumerable of all possible subsets.</returns>
        IEnumerable<IEnumerable<T>> GetSubsets<T>(IReadOnlyList<T> list, int numberOfElements);
    }
}