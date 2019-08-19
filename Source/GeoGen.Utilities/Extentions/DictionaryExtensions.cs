using System;
using System.Collections.Generic;

namespace GeoGen.Utilities
{
    /// <summary>
    /// Extension methods for <see cref="IDictionary{TKey,TValue}"/>.
    /// </summary>
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Gets a given value, if it's present in the dictionary, or creates 
        /// a new one using a given factory, adds it to the dictionary and returns it.
        /// </summary>
        /// <typeparam name="TKey">The key type.</typeparam>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="key">The key.</param>
        /// <param name="valueFactory">The factory for creating a new value.</param>
        /// <returns>The value from the dictionary, or a new created one.</returns>
        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> valueFactory)
        {
            // If the key is present, we can return the value directly
            if (dictionary.ContainsKey(key))
                return dictionary[key];

            // Otherwise we let the factory create a new value
            var value = valueFactory();

            // Add it to the dictionary
            dictionary.Add(key, value);

            // And return it
            return value;
        }

        /// <summary>
        /// Tries to get the value corresponding to a given key, or returns the default of the 
        /// <typeparamref name="TValue"/> type if the key is not present.
        /// </summary>
        /// <typeparam name="TKey">The key type.</typeparam>
        /// <typeparam name="TValue">The value type.</typeparam>
        /// <param name="dictionary">The dictionary.</param>
        /// <param name="key">The key.</param>
        /// <returns>The value from the dictionary, or the default value of the type <typeparamref name="TValue"/>.</returns>
        public static TValue GetOrDefault<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key) => dictionary.ContainsKey(key) ? dictionary[key] : default;
    }
}