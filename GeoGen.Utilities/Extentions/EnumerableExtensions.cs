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
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="enumerable">The enumerable.</param>
        /// <returns>true, if the enumerable is empty; false otherwise</returns>
        public static bool Empty<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable == null)
                throw new ArgumentNullException(nameof(enumerable));

            return !enumerable.Any();
        }

        /// <summary>
        /// Creates a single-element enumerable containing a given item.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="item">The item</param>
        /// <returns>The enumerable with count 1.</returns>
        public static IEnumerable<T> SingleItemAsEnumerable<T>(this T item)
        {
            yield return item;
        }

        /// <summary>
        /// Converts an enumerable to <see cref="HashSet{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="enumerable">The enumerable.</param>
        /// <returns>The hash set of the enumerable's items.</returns>
        public static HashSet<T> ToSet<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable == null)
                throw new ArgumentNullException(nameof(enumerable));

            return new HashSet<T>(enumerable);
        }

        /// <summary>
        /// Converts an enumerable to <see cref="HashSet{T}"/> using
        /// a custom equality comparer.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="enumerable">The enumerable.</param>
        /// <param name="equalityComparer">The equality comparer.</param>
        /// <returns>The hash set of the enumerable's items.</returns>
        public static HashSet<T> ToSet<T>(this IEnumerable<T> enumerable, IEqualityComparer<T> equalityComparer)
        {
            if (enumerable == null)
                throw new ArgumentNullException(nameof(enumerable));

            if (equalityComparer == null)
                throw new ArgumentNullException(nameof(equalityComparer));

            return new HashSet<T>(enumerable, equalityComparer);
        }

        /// <summary>
        /// Adds the specified element at the end of the IEnummerable.
        /// </summary>
        /// <typeparam name="T">The type of elements the IEnumerable contains.</typeparam>
        /// <param name="enumerable">The enumerable.</param>
        /// <param name="item">The item to be concatenated.</param>
        /// <returns>An IEnumerable, enumerating first the items in the existing enumerable</returns>
        public static IEnumerable<T> ConcatItem<T>(this IEnumerable<T> enumerable, T item)
        {
            if (enumerable == null)
                throw new ArgumentException(nameof(enumerable));

            foreach (var t in enumerable)
                yield return t;

            yield return item;
        }
    }
}