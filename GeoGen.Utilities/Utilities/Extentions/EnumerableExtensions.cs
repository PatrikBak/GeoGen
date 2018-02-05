using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.Utilities
{
    /// <summary>
    /// Extension methods for <see cref="IEnumerable{T}"/>
    /// </summary>
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Checks if the enumerable has no element.
        /// </summary>
        /// <typeparam name="T">The type of elements.</typeparam>
        /// <param name="enumerable">The enumerable.</param>
        /// <returns>true, if the enumerable is empty; false otherwise</returns>
        public static bool Empty<T>(this IEnumerable<T> enumerable)
        {
            return !enumerable.Any();
        }

        /// <summary>
        /// Creates a single-element enumerable containing a given item.
        /// </summary>
        /// <typeparam name="T">The type of the element.</typeparam>
        /// <param name="item">The item</param>
        /// <returns>The enumerable containing the item.</returns>
        public static IEnumerable<T> AsEnumerable<T>(this T item)
        {
            yield return item;
        }

        /// <summary>
        /// Converts an enumerable to a <see cref="HashSet{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of elements.</typeparam>
        /// <param name="enumerable">The enumerable.</param>
        /// <returns>The hash set of the enumerable's items.</returns>
        public static HashSet<T> ToSet<T>(this IEnumerable<T> enumerable)
        {
            return new HashSet<T>(enumerable);
        }

        /// <summary>
        /// Converts an enumerable to a <see cref="HashSet{T}"/> using
        /// a custom equality comparer.
        /// </summary>
        /// <typeparam name="T">The type of elements.</typeparam>
        /// <param name="enumerable">The enumerable.</param>
        /// <param name="equalityComparer">The equality comparer.</param>
        /// <returns>The hash set of the enumerable's items.</returns>
        public static HashSet<T> ToSet<T>(this IEnumerable<T> enumerable, IEqualityComparer<T> equalityComparer)
        {
            return new HashSet<T>(enumerable, equalityComparer);
        }

        /// <summary>
        /// Finds out if at least a given number of elements matches a given predicate.
        /// </summary>
        /// <typeparam name="T">The type of elements.</typeparam>
        /// <param name="enumerable">The enumerable.</param>
        /// <param name="count">The number of needed elements.</param>
        /// <param name="predicate">The predicate.</param>
        /// <returns>true, if there are at least 'count' number of elements that match the predicate; false otherwise</returns>
        public static bool AtLeast<T>(this IEnumerable<T> enumerable, int count, Func<T, bool> predicate)
        {
            // Prepare variable for keeping track of already found good elements
            var matches = 0;

            // Enumerate
            foreach (var element in enumerable)
            {
                // If element is not fine, we skip it
                if (!predicate(element))
                    continue;

                // If it's good, we update the count
                matches++;

                // If we have enough elements, we might return
                if (matches == count)
                    return true;
            }

            // Otherwise we didn't find enough good elements
            return false;
        }
    }
}