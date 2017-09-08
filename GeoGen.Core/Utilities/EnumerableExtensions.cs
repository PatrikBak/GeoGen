using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

namespace GeoGen.Core.Utilities
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
        /// Creates a single-element enumerable containing the given item.
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
        /// <returns></returns>
        public static HashSet<T> ToSet<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable == null)
                throw new ArgumentNullException(nameof(enumerable));

            return new HashSet<T>(enumerable);
        }
    }
}