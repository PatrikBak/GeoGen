using System.Collections.Generic;

namespace GeoGen.Utilities
{
    /// <summary>
    /// Represents a Cartesian product generator from a given dictionary mapping TKey to the enumerable
    /// of elements of the type TValue. Each generated element is then a dictionary mapping TKeys to TValues.
    /// </summary>
    public interface ICombinator
    {
        /// <summary>
        /// Generates all possible combinations of elements provided in the possibilities map. This method should
        /// work in a lazy way. For example: For { a, [1, 2] }, { b, [2, 3] } it will generate 4 dictionaries:
        /// { {a, 1}, {b, 2} }, { {a, 1}, {b, 3} }, { {a, 2}, {b, 2} }, { {a, 2}, {b, 3} }.  If there is a key with 
        /// no possibilities, the result would be an empty enumerable.
        /// </summary>
        /// <param name="possibilities">The possibilities dictionary.</param>
        /// <returns>The lazy enumerable of resulting combinations.</returns>
        IEnumerable<Dictionary<TKey, TValue>> Combine<TKey, TValue>(IReadOnlyDictionary<TKey, IEnumerable<TValue>> possibilities);
    }
}