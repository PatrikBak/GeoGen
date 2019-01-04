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
    }
}