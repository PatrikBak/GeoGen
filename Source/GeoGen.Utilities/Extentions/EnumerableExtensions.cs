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
        /// Projects each element of a sequence into a new form, excluding null elements.
        /// </summary>
        /// <typeparam name="TSource">The type of the elements of source.</typeparam>
        /// <typeparam name="TResult">The type of the value returned by selector.</typeparam>
        /// <param name="enumerable">The enumerable</param>
        /// <param name="selector">The transform function to apply to each element.</param>
        /// <returns>The projected enumerable contaning non-null elements only.</returns>
        public static IEnumerable<TResult> SelectNotNull<TSource, TResult>(this IEnumerable<TSource> enumerable, Func<TSource, TResult> selector)
        {
            return enumerable.Select(selector).Where(item => item != null);
        }

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
        public static bool IsEmpty<T>(this IEnumerable<T> enumerable) => !enumerable.Any();

        /// <summary>
        /// Checks if the enumerable is null or has no elements.
        /// </summary>
        /// <typeparam name="T">The type of elements.</typeparam>
        /// <param name="enumerable">The enumerable.</param>
        /// <returns>true, if the enumerable is null or empty; false otherwise</returns>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable) => enumerable == null || enumerable.IsEmpty();

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

        /// <summary>
        /// Converts an enumerable to a <see cref="Dictionary{TKey, TValue}"/> using a custom key selector and a custom value selector.
        /// </summary>
        /// <typeparam name="TKey">The type of the keys for the dictionary.</typeparam>
        /// <typeparam name="TValue">The type of the values for the dictionary.</typeparam>
        /// <typeparam name="TSource">The type of the enumerable source items.</typeparam>
        /// <param name="enumerable">The enumerable.</param>
        /// <param name="keySelector">The key selector accepting the source item and its enumeration index as parameters.</param>
        /// <param name="valueSelector">The value selector accepting the source item and its enumeration index as parameters.</param>
        /// <returns>The dictionary.</returns>
        public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue, TSource>(this IEnumerable<TSource> enumerable, Func<TSource, int, TKey> keySelector, Func<TSource, int, TValue> valueSelector)
        {
            // Cast each item to a (item, index) tuple and then use the .NET ToDictionary method
            return enumerable.Select((item, index) => (item, index)).ToDictionary(pair => keySelector(pair.item, pair.index), pair => valueSelector(pair.item, pair.index));
        }
    }
}