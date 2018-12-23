using System;
using System.Collections.Generic;

namespace GeoGen.Utilities
{
    /// <summary>
    /// A static helper class for calculating hash codes.
    /// </summary>
    public static class HashCodeUtilities
    {
        /// <summary>
        /// Gets an order independent hash code of a given collection, using a given function 
        /// to determine hash codes of single elements. The code is taken from: 
        /// https://stackoverflow.com/questions/670063/getting-hash-of-a-list-of-strings-regardless-of-order
        /// </summary>
        /// <typeparam name="T">The type of elements.</typeparam>
        /// <param name="enumerable">The enumerable</param>
        /// <param name="hashCoder">The function that takes an element and returns it's hash code.</param>
        /// <returns>The hash code.</returns>
        public static int GetOrderIndependentHashCode<T>(this IEnumerable<T> enumerable, Func<T, int> hashCoder)
        {
            var hash = 0;
            var valueCounts = new Dictionary<T, int>();

            foreach (var element in enumerable)
            {
                var curHash = hashCoder(element);

                if (valueCounts.TryGetValue(element, out var bitOffset))
                    valueCounts[element] = bitOffset + 1;
                else
                    valueCounts.Add(element, bitOffset);

                // The current hash code is shifted (with wrapping) one bit
                // further left on each successive recurrence of a certain
                // value to widen the distribution.
                // 37 is an arbitrary low prime number that helps the
                // algorithm to smooth out the distribution.
                hash = unchecked(hash + ((curHash << bitOffset) | (curHash >> (32 - bitOffset))) * 37);
            }

            return hash;
        }

        /// <summary>
        /// Gets an order dependent hash code from given objects.
        /// </summary>
        /// <param name="objects">The objects.</param>
        /// <returns>The hash code.</returns>
        public static int GetOrderDependentHashCode(params object[] objects)
        {
            var hash = 0;

            foreach (var item in objects)
            {
                unchecked
                {
                    hash *= 37;
                    hash += item.GetHashCode();
                }
            }

            return hash;
        }
    }
}