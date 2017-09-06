using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.Core.Utilities.Combinator
{
    /// <summary>
    /// A recursive implementation of the interface <see cref="ICombinator{TKey,TValue}" />
    /// The class is thread-safe.
    /// </summary>
    /// <typeparam name="TKey">The dictionary key type.</typeparam>
    /// <typeparam name="TValue">The dictionary value type.</typeparam>
    public class Combinator<TKey, TValue> : ICombinator<TKey, TValue>
    {
        /// <summary>
        /// Generates all possible combinations of elements provided in the possibilities map. This method should
        /// work in a lazy way. For example: For { a, [1, 2] }, { b, [2, 3] } it will generate 4 dictionaries:
        /// { {a, 1}, {b, 2} }, { {a, 1}, {b, 3} }, { {a, 2}, {b, 2} }, { {a, 2}, {b, 3} }.  If there is key with 
        /// no possibilities, the result would be an empty enumerable.
        /// </summary>
        /// <param name="possibilities">The possibilities dictionary.</param>
        /// <returns>The lazy enumerable of resulting combinations.</returns>
        public IEnumerable<IReadOnlyDictionary<TKey, TValue>> Combine(IReadOnlyDictionary<TKey, IEnumerable<TValue>> possibilities)
        {
            if (possibilities == null)
                throw new ArgumentNullException(nameof(possibilities));

            var keysList = possibilities.Keys.ToList();

            if (keysList.Empty())
                throw new ArgumentException("The possibilities dictionary can't be empty");

            return GeneratePossibleArrays(0, new TValue[keysList.Count], possibilities, keysList).Select
            (
                // Cast generated array to dictionary
                array => array.Select((value, i) => new {value, i}).ToDictionary(arg => keysList[arg.i], arg => arg.value)
            );
        }

        /// <summary>
        /// Recursively generates all possible combinations stored in the array ordered such that together with
        /// the keys list it represents a resultig dictionary.
        /// </summary>
        /// <param name="index">The current index of the generation process. On start it should be 0.</param>
        /// <param name="result">The resulting array that will be yielded when it's ready. It size should be keys.Count</param>
        /// <param name="possibilities">The possibilities dictionary input.</param>
        /// <param name="keys">Extracted list of keysfrom the possibities dictionary (so we don't have to enumerate them more than once).</param>
        /// <returns>Laze enumerable of all possible combinations (ordered according to the order of the keys).</returns>
        private static IEnumerable<TValue[]> GeneratePossibleArrays(int index, TValue[] result,
            IReadOnlyDictionary<TKey, IEnumerable<TValue>> possibilities, IReadOnlyList<TKey> keys)
        {
            // TODO: Comment out
            foreach (var element in possibilities[keys[index]])
            {
                result[index] = element;

                if (index == keys.Count - 1)
                {
                    yield return result;
                }
                else
                {
                    foreach (var combination in GeneratePossibleArrays(index + 1, result, possibilities, keys))
                    {
                        yield return combination;
                    }
                }
            }
        }
    }
}