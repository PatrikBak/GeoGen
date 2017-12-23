using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.Utilities
{
    /// <summary>
    /// A default recursive implementation of <see cref="ICombinator"/>.
    /// </summary>
    public sealed class Combinator : ICombinator
    {
        #region ICombinator methods

        /// <summary>
        /// Generates all possible combinations of elements provided in the possibilities map. This method should
        /// work in a lazy way. For example: For { a, [1, 2] }, { b, [2, 3] } it will generate 4 dictionaries:
        /// { {a, 1}, {b, 2} }, { {a, 1}, {b, 3} }, { {a, 2}, {b, 2} }, { {a, 2}, {b, 3} }.  If there is key with 
        /// no possibilities, the result would be an empty enumerable.
        /// </summary>
        /// <param name="possibilities">The possibilities dictionary.</param>
        /// <returns>The lazy enumerable of resulting combinations.</returns>
        public IEnumerable<Dictionary<TKey, TValue>> Combine<TKey, TValue>(IReadOnlyDictionary<TKey, IEnumerable<TValue>> possibilities)
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

        #endregion

        #region Private methods

        /// <summary>
        /// Recursively generates all possible combinations stored in the array ordered such that together with
        /// the keys list it represents a resulting dictionary.
        /// </summary>
        /// <typeparam name="TKey">The key type.</typeparam>
        /// <typeparam name="TValue">The value type..</typeparam>
        /// <param name="index">The current index of the generation process. On start it should be 0.</param>
        /// <param name="result">The resulting array that will be yielded when it's ready. It size should be keys.Count</param>
        /// <param name="possibilities">The possibilities dictionary input.</param>
        /// <param name="keys">Extracted list of keys from the possibilities dictionary (so we don't have to enumerate them more than once).</param>
        /// <returns>Laze enumerable of all possible combinations (ordered according to the order of the keys).</returns>
        private static IEnumerable<TValue[]> GeneratePossibleArrays<TKey, TValue>
        (
            int index,
            TValue[] result,
            IReadOnlyDictionary<TKey, IEnumerable<TValue>> possibilities,
            IReadOnlyList<TKey> keys
        )
        {
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

        #endregion
    }
}