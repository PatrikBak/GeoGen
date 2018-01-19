using System.Collections.Generic;
using System.Linq;

namespace GeoGen.Utilities
{
    /// <summary>
    /// A default implementation of <see cref="ICombinator"/>.
    /// </summary>
    public class Combinator : ICombinator
    {
        #region ICombinator implementation

        /// <summary>
        /// Generates all possible combinations of elements provided in the possibilities map. This method should
        /// work in a lazy way. For example: For { a, [1, 2] }, { b, [2, 3] } it will generate 4 dictionaries:
        /// { {a, 1}, {b, 2} }, { {a, 1}, {b, 3} }, { {a, 2}, {b, 2} }, { {a, 2}, {b, 3} }.  If there is a key with 
        /// no possibilities, the result would be an empty enumerable.
        /// </summary>
        /// <param name="possibilities">The possibilities dictionary.</param>
        /// <returns>The lazy enumerable of resulting combinations.</returns>
        public IEnumerable<Dictionary<TKey, TValue>> Combine<TKey, TValue>(IReadOnlyDictionary<TKey, IEnumerable<TValue>> possibilities)
        {
            // Enumerate the keys so we have them in some order
            var keysList = possibilities.Keys.ToList();

            // Local function to cast a given array of values to the options dictionary
            Dictionary<TKey, TValue> CastValuesToOption(IEnumerable<TValue> values)
            {
                // Select a given array to the anonymous type that remembers the index
                return values.Select((value, i) => new {value, i})
                        // For each of these tuples we find the corresponding TKey element 
                        // and pair it with the TValue element
                        .ToDictionary(arg => keysList[arg.i], arg => arg.value);
            }

            // Call the private function to generate possible TValue array and cast them
            // to the dictionary using a local helper function
            return GeneratePossibleArrays(0, new TValue[keysList.Count], possibilities, keysList).Select(CastValuesToOption);
        }

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
            // Go through all possible elements for the given index
            foreach (var element in possibilities[keys[index]])
            {
                // For each one, set the element on the index
                result[index] = element;

                // If this is the last index
                if (index == keys.Count - 1)
                {
                    // Yield the result
                    yield return result;
                }
                // Otherwise
                else
                {
                    // Generate all possible combinations for higher index (recursively)
                    foreach (var combination in GeneratePossibleArrays(index + 1, result, possibilities, keys))
                    {
                        // And yield each of them
                        yield return combination;
                    }
                }
            }
        }

        #endregion
    }
}