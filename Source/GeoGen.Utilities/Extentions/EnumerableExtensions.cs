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
        /// Sorts the elements of a sequence in ascending order
        /// </summary>
        /// <typeparam name="T">The type of the elements of the enumerable.</typeparam>
        /// <param name="enumerable">The numerable.</param>
        /// <returns>The sorter enumerable.</returns>
        public static IOrderedEnumerable<T> Ordered<T>(this IEnumerable<T> enumerable) => enumerable.OrderBy(element => element);

        /// <summary>
        /// A fluent version of the string.Join method.
        /// </summary>
        /// <typeparam name="T">The type of the elements.</typeparam>
        /// <param name="enumerable">The enumerable.</param>
        /// <param name="separator">The separator of elements.</param>
        /// <returns>The elements joined with the operator.</returns>
        public static string ToJoinedString<T>(this IEnumerable<T> enumerable, string separator = ", ") => string.Join(separator, enumerable);

        /// <summary>
        /// Projects each element of the source using a given select selector, but
        /// only if the selector does not return the default value of <typeparamref name="TResult"/>.
        /// In that gets returns null.
        /// </summary>
        /// <typeparam name="TSource">The type of source items.</typeparam>
        /// <typeparam name="TResult">The type of result items.</typeparam>
        /// <param name="source">The source enumerable.</param>
        /// <param name="selector">The selector function.</param>
        /// <returns>The projected enumerable, if the selector doesn't return the default value; otherwise null.</returns>
        public static List<TResult> SelectIfNotDefault<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
        {
            // Prepare a list of results
            var results = new List<TResult>();

            // Go through the source
            foreach (var element in source)
            {
                // Apply the selector
                var result = selector(element);

                // If the selector return the default value, cut it
                if (result == default)
                    return null;

                // Otherwise add the element
                results.Add(result);
            }

            // Return the results
            return results;
        }

        /// <summary>
        /// Maps given keys to the given values as a dictionary.
        /// </summary>
        /// <typeparam name="TKey">The key type.</typeparam>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="keys">The keys.</param>
        /// <param name="values">The values.</param>
        /// <returns>The dictionary sequentially mapping the elements from <paramref name="keys"/> to the elements from <paramref name="values"/></returns>
        public static Dictionary<TKey, TValue> ZipToDictionary<TKey, TValue>(this IEnumerable<TKey> keys, IEnumerable<TValue> values)
        {
            // First use Zip to connect those sequences into a sequence of pairs
            return keys.Zip(values, (key, value) => (key, value))
                // And then simply make a dictionary 
                .ToDictionary(pair => pair.key, pair => pair.value);
        }

        /// <summary>
        /// Flattens the enumerable of enumerables into a single enumerable.
        /// </summary>
        /// <typeparam name="T">The type of the elements.</typeparam>
        /// <param name="enumerable">The enumerable.</param>
        /// <returns>The enumerable containing all the inner enumerable items.</returns>
        public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> enumerable) => enumerable.SelectMany(_ => _);

        /// <summary>
        /// Concatenates given elements at the end of the enumerable.
        /// </summary>
        /// <typeparam name="T">The type of the elements.</typeparam>
        /// <param name="enumerable">The enumerable.</param>
        /// <param name="items">The items to be concatenated.</param>
        /// <returns>The enumerable with the concatenated items.</returns>
        public static IEnumerable<T> Concat<T>(this IEnumerable<T> enumerable, params T[] items) => enumerable.Concat((IEnumerable<T>)items);

        /// <summary>
        /// Invokes a given action for each element in the enumerable.
        /// </summary>
        /// <typeparam name="T">The type of the elements.</typeparam>
        /// <param name="enumerable">The enumerable.</param>
        /// <param name="action">The action to invoke.</param>
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action) => enumerable.ForEach((element, index) => action(element));

        /// <summary>
        /// Invokes a given action for each element in the enumerable.
        /// </summary>
        /// <typeparam name="T">The type of the elements.</typeparam>
        /// <param name="enumerable">The enumerable.</param>
        /// <param name="action">The action to invoke, with two parameters: The element and its index.</param>
        public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T, int> action)
        {
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
        /// <returns>true, if the enumerable is empty; false otherwise.</returns>
        public static bool IsEmpty<T>(this IEnumerable<T> enumerable) => !enumerable.Any();

        /// <summary>
        /// Checks if the enumerable is null or has no elements.
        /// </summary>
        /// <typeparam name="T">The type of elements.</typeparam>
        /// <param name="enumerable">The enumerable.</param>
        /// <returns>true, if the enumerable is null or empty; false otherwise.</returns>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> enumerable) => enumerable == null || enumerable.IsEmpty();

        /// <summary>
        /// Creates a single-element enumerable containing a given item.
        /// </summary>
        /// <typeparam name="T">The type of the element.</typeparam>
        /// <param name="item">The item.</param>
        /// <returns>The enumerable containing the single given item.</returns>
        public static IEnumerable<T> ToEnumerable<T>(this T item) => new[] { item };

        /// <summary>
        /// Converts an enumerable to a <see cref="HashSet{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of elements.</typeparam>
        /// <param name="enumerable">The enumerable.</param>
        /// <returns>The hash set of the enumerable's items.</returns>
        public static HashSet<T> ToSet<T>(this IEnumerable<T> enumerable) => new HashSet<T>(enumerable);

        /// <summary>
        /// Converts an enumerable to a <see cref="SortedSet{T}"/>.
        /// </summary>
        /// <typeparam name="T">The type of elements.</typeparam>
        /// <param name="enumerable">The enumerable.</param>
        /// <returns>The sorted set of the enumerable's items.</returns>
        public static SortedSet<T> ToSortedSet<T>(this IEnumerable<T> enumerable) => new SortedSet<T>(enumerable);

        /// <summary>
        /// Converts an enumerable to a <see cref="HashSet{T}"/> using a custom equality comparer.
        /// </summary>
        /// <typeparam name="T">The type of elements.</typeparam>
        /// <param name="enumerable">The enumerable.</param>
        /// <param name="equalityComparer">The equality comparer.</param>
        /// <returns>The hash set of the enumerable's items.</returns>
        public static HashSet<T> ToSet<T>(this IEnumerable<T> enumerable, IEqualityComparer<T> equalityComparer) => new HashSet<T>(enumerable, equalityComparer);

        /// <summary>
        /// Converts an enumerable to a <see cref="Dictionary{TKey, TValue}"/> using a custom key selector and a custom value selector.
        /// </summary>
        /// <typeparam name="TKey">The type of keys of the dictionary.</typeparam>
        /// <typeparam name="TValue">The type of values of the dictionary.</typeparam>
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

        /// <summary>
        /// Generates all possible variations with a given size of the elements of
        /// a given enumerable. For example: For the list [1, 2, 3] all the variations
        /// with 2 elements are: [1, 2],  [1, 3], [2, 1], [2, 3], [3, 1], [3, 2]. 
        /// The generation process is lazy. No particular order is guaranteed. The 
        /// enumerable will be enumerated once in the process. The resulting array will 
        /// change every iteration (for efficiency), so if the result needs to be stored,
        /// it needs to be cloned first.
        /// </summary>
        /// <typeparam name="T">The type of elements of the enumerable.</typeparam>
        /// <param name="enumerable">The enumerable.</param>
        /// <param name="numberOfElement">The number of elements in each variation.</param>
        /// <returns>A lazy enumerable of all possible variations with a given size.</returns>
        public static IEnumerable<T[]> Variations<T>(this IEnumerable<T> enumerable, int numberOfElements)
        {
            // Enumerate the enumerable to an array so we can swap its elements
            var items = enumerable.ToArray();

            // Prepare the result that will be returned whenever it's ready
            var result = new T[numberOfElements];

            // Local function that performs the generation algorithm.
            // The idea of the algorithm is the following: Assume that 
            // the items of our array from the interval [0, startIndex-1]
            // represent our variation (or no items for startIndex=0).
            // Then we need to pick the next element, which can be any
            // element from the interval [startIndex, items.Length-1]. 
            // After we pick one (at the index 'i'), we then swap 
            // the elements at 'startIndex' and 'i' so that the sub-array
            // [0, startIndex] is our variation. Then we recursively call 
            // the algorithm for 'startIndex+1'. We do this until we have
            // the requested number of elements
            IEnumerable<T[]> Generate(int startIndex)
            {
                // If we have enough elements...
                if (startIndex == numberOfElements)
                {
                    // Lazily return the result
                    yield return result;
                }
                // Otherwise...
                else
                {
                    // We decide which of the elements from the interval [startIndex, items.Length-1] we're going to add
                    // to our variation (which already has its elements at the indices at the interval [0, startIndex-1]
                    for (var i = startIndex; i < items.Length; i++)
                    {
                        // If we've decided for the i-th element. First we set the result so it reflects the interval [0, startIndex]
                        result[startIndex] = items[i];

                        // Then we make sure that the interval [0, startIndex] represents our current variation
                        GeneralUtilities.Swap(ref items[i], ref items[startIndex]);

                        // Perform recursive search for variations for the rest of the sequence
                        foreach (var variation in Generate(startIndex + 1))
                            yield return variation;

                        // And revert the swapping we did
                        GeneralUtilities.Swap(ref items[i], ref items[startIndex]);
                    }
                }
            }

            // Execute the generated process from the beginning
            return Generate(0);
        }

        /// <summary>
        /// Generates all possible permutations from the elements of a given enumerable. 
        /// For example: For the list [1, 2, 3] all the permutations are [1, 2, 3],  
        /// [1, 3, 2], [2, 1, 3], [2, 3, 1], [3, 1, 2], [3, 2, 1]. The generation process 
        /// is lazy. No particular order is guaranteed. The enumerable will be enumerated 
        /// once in the process. The resulting array will change every iteration (for
        /// efficiency), so if the result needs to be stored, it needs to be cloned first.
        /// </summary>
        /// <typeparam name="T">The type of elements of the enumerable.</typeparam>
        /// <param name="enumerable">The enumerable.</param>
        /// <returns>A lazy enumerable of all possible permutations.</returns>
        public static IEnumerable<T[]> Permutations<T>(this IEnumerable<T> enumerable)
        {
            // Enumerate the enumerable
            var array = enumerable.ToArray();

            // Permutations are variations of all the elements
            return array.Variations(array.Length);
        }

        /// <summary>
        /// Generates all possible combinations from the elements from the enumerables, where
        /// each enumerable with its position represents the set of possible options at this position.
        /// For example, for three options lists [2,3], [3], [5,4] all the results will be 
        /// [2,3,5], [2,3,4], [3,3,5], [3,3,4]. The resulting array will change every iteration 
        /// (for efficiency), so if the result needs to be stored, it needs to be cloned first.
        /// </summary>
        /// <typeparam name="T">The type of the elements of enumerable.</typeparam>
        /// <param name="enumerable">The enumerable.</param>
        /// <returns>A lazy enumerable of all possible combinations of elements.</returns>
        public static IEnumerable<T[]> Combine<T>(this IEnumerable<IEnumerable<T>> enumerable)
        {
            // Enumerate the options
            var optionsArray = enumerable.ToArray();

            // Prepare the resulting array that will be returned whenever it's ready
            var result = new T[optionsArray.Length];

            // Local function that performs the generation. It assumes we've already decided
            // for some elements from the options at the indices from the interval [0, optionsIndex-1],
            // then it decides for some element from the option enumerable at 'optionsIndex', 
            // and then recursively decide for the other ones
            IEnumerable<T[]> Generate(int optionsIndex)
            {
                // If we've been through all the option enumerables...
                if (optionsIndex == optionsArray.Length)
                {
                    // Then we lazily return the result
                    yield return result;
                }
                // Otherwise...
                else
                {
                    // We decide which of the elements of the current option's enumerable we're going to pick
                    foreach (var item in optionsArray[optionsIndex])
                    {
                        // If we've decided for some item, we first mark it in the result
                        result[optionsIndex] = item;

                        // And recursively examine the other option enumerables
                        foreach (var option in Generate(optionsIndex + 1))
                            yield return option;
                    }
                }
            }

            // Execute the algorithm from the beginning
            return Generate(0);
        }

        /// <summary>
        /// Generates all possible subsets from the elements of a given enumerable 
        /// with a given size. For example: For the list [1, 2, 3, 4] all the subset
        /// of size 2 are [1,2], [1,3], [1,4], [2,3], [2,4], [3,4]. The generation process 
        /// is lazy. No particular order is guaranteed. The enumerable will be enumerated 
        /// once in the process. 
        /// </summary>
        /// <typeparam name="T">The type of elements of the enumerable.</typeparam>
        /// <param name="enumerable">The enumerable.</param>
        /// <param name="numberOfElements">The size of each subset.</param>
        /// <returns>A lazy enumerable of all possible subsets with a given size.</returns>
        public static IEnumerable<IEnumerable<T>> Subsets<T>(this IEnumerable<T> enumerable, int numberOfElements)
        {
            // Enumerate the items to an array
            var items = enumerable.ToArray();

            // Local function that performs a simple recursive algorithm that returns all the
            // from the elements with indices from the interval [startIndex, items.Length-1] 
            // with the requested size
            IEnumerable<IEnumerable<T>> Generate(int startIndex, int requestedNumberOfElements)
            {
                // Our available elements have indices from the interval [startIndex, items.Length-1],
                // i.e. there are 'item.Length - startIndex' of them. If we're requesting more...
                if (items.Length - startIndex < requestedNumberOfElements)
                {
                    // We can break
                    yield break;
                }
                // Otherwise if we're requesting 0 elements
                else if (requestedNumberOfElements == 0)
                {
                    // Them we simply return an empty set
                    yield return Enumerable.Empty<T>();
                }
                // Otherwise...
                else
                {
                    // We first assume the element at the 'startIndex' is in the subset. Then we need to 
                    // request one fewer elements from the remaining ones and include this one as well
                    foreach (var subset in Generate(startIndex + 1, requestedNumberOfElements - 1))
                        yield return subset.Concat(items[startIndex]);

                    // Now we assume we're not including the element at the 'startIndex'. Then we ask
                    // the same number of elements 
                    foreach (var subset in Generate(startIndex + 1, requestedNumberOfElements))
                        yield return subset;
                }
            }

            // Execute the algorithm from the beginning
            return Generate(0, numberOfElements);
        }

        /// <summary>
        /// Generates all possible subsets from the elements of a given enumerable.
        /// </summary>
        /// <typeparam name="T">The type of elements of the enumerable.</typeparam>
        /// <param name="enumerable">The enumerable.</param>
        /// <returns>A lazy enumerable of all possible subsets.</returns>
        public static IEnumerable<IEnumerable<T>> Subsets<T>(this IEnumerable<T> enumerable)
        {
            // Enumerate the items to an array
            var items = enumerable.ToArray();

            // Merge the subsets of every possible size
            return Enumerable.Range(0, items.Length + 1).SelectMany(size => items.Subsets(size));
        }
    }
}