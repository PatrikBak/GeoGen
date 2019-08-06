using System.Collections.Generic;

namespace GeoGen.Utilities
{
    /// <summary>
    /// Extension methods for <see cref="IList{T}"/>.
    /// </summary>
    public static class ListExtentions
    {
        /// <summary>
        /// Enumerates all the unordered pairs of distinct items of the list.
        /// </summary>
        /// <typeparam name="T">The type of items.</typeparam>
        /// <param name="list">The list.</param>
        /// <returns>An enumerable of all the unordered pairs.</returns>
        public static IEnumerable<(T, T)> UnorderedPairs<T>(this IList<T> list)
        {
            // Go through all the items twice
            for (var i = 0; i < list.Count; i++)
                for (var j = i + 1; j < list.Count; j++)
                    yield return (list[i], list[j]);
        }

        /// <summary>
        /// Enumerates all the unordered triples of distinct items of the list.
        /// </summary>
        /// <typeparam name="T">The type of items.</typeparam>
        /// <param name="list">The list.</param>
        /// <returns>An enumerable of all the unordered triples.</returns>
        public static IEnumerable<(T, T, T)> UnorderedTriples<T>(this IList<T> list)
        {
            // Go through all the items twice
            for (var i = 0; i < list.Count; i++)
                for (var j = i + 1; j < list.Count; j++)
                    for (var k = j + 1; k < list.Count; k++)
                        yield return (list[i], list[j], list[k]);
        }

        /// <summary>
        /// Finds the index of an item in the list using a custom comparer.
        /// </summary>
        /// <typeparam name="T">The type of items.</typeparam>
        /// <param name="list">The list.</param>
        /// <param name="item">The item that we are looking for.</param>
        /// <param name="comparer">The equality comparer used to compare items.</param>
        /// <returns>The index of the found item, if it exists; -1 otherwise.</returns>
        public static int IndexOf<T>(this IList<T> list, T item, IEqualityComparer<T> comparer)
        {
            // Go through the list
            for (var index = 0; index < list.Count; index++)
            {
                // If the current item is equal to this one, return the index
                if (comparer.Equals(item, list[index]))
                    return index;
            }

            // If we got here, we didn't find it
            return -1;
        }
    }
}