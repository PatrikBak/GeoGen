using System.Collections.Generic;

namespace GeoGen.Utilities
{
    /// <summary>
    /// Extension methods for <see cref="IReadOnlyList{T}"/>.
    /// </summary>
    public static class ListExtensions
    {
        /// <summary>
        /// Enumerates all the unordered pairs of distinct items of the list.
        /// </summary>
        /// <typeparam name="T">The type of items.</typeparam>
        /// <param name="list">The list.</param>
        /// <returns>An enumerable of all the unordered pairs.</returns>
        public static IEnumerable<(T, T)> UnorderedPairs<T>(this IReadOnlyList<T> list)
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
        public static IEnumerable<(T, T, T)> UnorderedTriples<T>(this IReadOnlyList<T> list)
        {
            // Go through all the items twice
            for (var i = 0; i < list.Count; i++)
                for (var j = i + 1; j < list.Count; j++)
                    for (var k = j + 1; k < list.Count; k++)
                        yield return (list[i], list[j], list[k]);
        }

        /// <summary>
        /// Finds the index of an item in the list using a function to retrieve items.
        /// </summary>        
        /// <typeparam name="T">The type of items of the list.</typeparam>
        /// <param name="list">The list.</param>
        /// <param name="item">The item that we're looking for.</param>
        /// <returns>The index of the found item, if it exists; -1 otherwise.</returns>
        public static int IndexOf<T>(this IReadOnlyList<T> list, T item)
        {
            // Go through the list
            for (var index = 0; index < list.Count; index++)
                // If the current item is equal to this one, 
                if (item.Equals(list[index]))
                    // Return the index
                    return index;

            // If we got here, we didn't find it
            return -1;
        }

        /// <summary>
        /// Calculates the hash-code of the list based on the hash-codes of its items.
        /// </summary>
        /// <typeparam name="T">The type of items of the list.</typeparam>
        /// <param name="list">The list.</param>
        /// <returns>The hash code.</returns>
        public static int GetHashCodeOfList<T>(this IReadOnlyList<T> list)
        {
            // We don't care about overflow
            unchecked
            {
                // A prime seed
                var hash = 19;

                // Perform a well-known algorithm for ordered items
                foreach (var item in list)
                    hash = hash * 31 + item.GetHashCode();

                // Return the result
                return hash;
            }
        }
    }
}