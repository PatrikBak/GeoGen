using System;
using System.Collections.Generic;

namespace GeoGen.Core.Utilities.Variations
{
    /// <summary>
    /// A fast recursive implementation of the <see cref="IVariationsProvider{T}"/> interface.
    /// The class is thread-safe.
    /// </summary>
    /// <typeparam name="T">The type of elements</typeparam>
    public class VariationsProvider<T> : IVariationsProvider<T>
    {
        /// <summary>
        /// Generates all possible variations of a given list. For example: For the list {1, 2, 3} all 
        /// the variations with 2 elements are: {1, 2}, {1, 3}, {2, 1}, {2, 3}, {3, 1}, {3, 2}.
        /// The generation is lazy. The count of generated enumerables will be numberOfElements.
        /// </summary>
        /// <param name="list">The list whose elements are used in variations.</param>
        /// <param name="numberOfElement">The number of elements in each variation.</param>
        /// <returns>Lazy enumerable of all possible variations.</returns>
        public IEnumerable<IEnumerable<T>> GetVariations(List<T> list, int numberOfElement)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (list.Empty())
                throw new ArgumentException("The list of elements can't be empty");

            if (numberOfElement < 1 || numberOfElement > list.Count)
                throw new ArgumentOutOfRangeException(nameof(numberOfElement), "The number of elements should be in the interval [1, list.Count].");

            return GetVariations(0, list.ToArray(), new T[numberOfElement], numberOfElement);
        }

        /// <summary>
        /// A recursive method to generate the variations.
        /// </summary>
        /// <param name="index">Current index of the generation process. On start it should be 0.</param>
        /// <param name="listCopy">The copy of the list that we're using. It will to be modified.</param>
        /// <param name="result">The resulting list that will be yield when it's ready. The count should be numberOfElements.</param>
        /// <param name="numberOfElements">The number of elements in each variation.</param>
        /// <returns>Lazy enumerable of all possible variations.</returns>
        private static IEnumerable<IEnumerable<T>> GetVariations(int index, T[] listCopy, IList<T> result, int numberOfElements)
        {
            // TODO: Comment out
            for (var i = index; i < listCopy.Length; i++)
            {
                result[index] = listCopy[i];
                Swap(ref listCopy[i], ref listCopy[index]);

                if (index == numberOfElements - 1)
                {
                    yield return result;
                }
                else
                {
                    foreach (var variation in GetVariations(index + 1, listCopy, result, numberOfElements))
                    {
                        yield return variation;
                    }
                }

                Swap(ref listCopy[i], ref listCopy[index]);
            }
        }

        /// <summary>
        /// Swaps the value of two elements.
        /// </summary>
        /// <param name="v1">The reference of the first element.</param>
        /// <param name="v2">The reference of the second element.</param>
        private static void Swap(ref T v1, ref T v2)
        {
            var old = v1;
            v1 = v2;
            v2 = old;
        }
    }
}