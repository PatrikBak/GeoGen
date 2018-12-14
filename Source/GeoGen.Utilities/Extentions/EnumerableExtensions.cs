using System;
using System.Collections;
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
        /// Invokes a given action for each element in the enumerable.
        /// </summary>
        /// <typeparam name="T">The type of the elements.</typeparam>
        /// <param name="enumerable">The enumerable.</param>
        /// <param name="action">The action to invoke.</param>
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            // Invoke the method that uses even index and ignores it
            enumerable.ForEach((element, index) => action(element));
        }

        /// <summary>
        /// Invokes a given action for each element in the enumerable.
        /// </summary>
        /// <typeparam name="T">The type of the elements.</typeparam>
        /// <param name="enumerable">The enumerable.</param>
        /// <param name="action">The action to invoke, with two paremeters: The element and its index.</param>
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T, int> action)
        {
            if (enumerable == null)
                throw new ArgumentNullException(nameof(enumerable));

            if (action == null)
                throw new ArgumentNullException(nameof(action));

            // Prepare a counter
            var counter = 0;

            // For each element...
            foreach (var element in enumerable)
            {
                // Invoke the action
                action(element, counter);

                // Mark that we've seen the element
                counter++;
            }
        }

        /// <summary>
        /// Invokes a given action for each element in the enumerable.
        /// </summary>
        /// <typeparam name="T">The type of the elements.</typeparam>
        /// <param name="enumerable">The enumerable.</param>
        /// <param name="action">The action to invoke.</param>
        public static void ForEach(this IEnumerable enumerable, Action<object> action)
        {
            if (enumerable == null)
                throw new ArgumentNullException(nameof(enumerable));

            if (action == null)
                throw new ArgumentNullException(nameof(action));

            // For each element...
            foreach (var element in enumerable)
            {
                // Invoke the action
                action(element);
            }
        }

        /// <summary>
        /// Checks if the enumerable has no elements.
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
        /// <param name="item">The item.</param>
        /// <returns>The enumerable containing the single given item.</returns>
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
        /// Converts an enumerable to a <see cref="HashSet{T}"/> using a custom equality comparer.
        /// </summary>
        /// <typeparam name="T">The type of elements.</typeparam>
        /// <param name="enumerable">The enumerable.</param>
        /// <param name="equalityComparer">The equality comparer.</param>
        /// <returns>The hash set of the enumerable's items.</returns>
        public static HashSet<T> ToSet<T>(this IEnumerable<T> enumerable, IEqualityComparer<T> equalityComparer)
        {
            return new HashSet<T>(enumerable, equalityComparer);
        }
    }
}