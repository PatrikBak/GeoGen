using System.Collections.Generic;
using System.Linq;

namespace GeoGen.Utilities
{
    /// <summary>
    /// A default implementation of <see cref="IVariationsProvider"/>.
    /// </summary>
    public class VariationsProvider : IVariationsProvider
    {
        /// <summary>
        /// Generates all possible variations of a given list. For example: For the list {1, 2, 3} all 
        /// the variations with 2 elements are: {1, 2}, {1, 3}, {2, 1}, {2, 3}, {3, 1}, {3, 2}.
        /// The generation is lazy. The size of generated enumerables will be numberOfElements.
        /// </summary>
        /// <typeparam name="T">The type of elements.</typeparam>
        /// <param name="list">The list whose elements are used in variations.</param>
        /// <param name="numberOfElement">The number of elements in each variation.</param>
        /// <returns>Lazy enumerable of all possible variations.</returns>
        public IEnumerable<IEnumerable<T>> GetVariations<T>(IReadOnlyList<T> list, int numberOfElement)
        {
            // Call the private function that stores the variations inside the created array
            return GetVariations(0, list.ToArray(), new T[numberOfElement], numberOfElement);
        }

        /// <summary>
        /// A recursive method to generate the variations.
        /// </summary>
        /// <typeparam name="T">The type of elements.</typeparam>
        /// <param name="index">Current index of the generation process. On start it should be 0.</param>
        /// <param name="listCopy">The copy of the list that we're using. It will be modified.</param>
        /// <param name="result">The resulting list that will be yield when it's ready. Its count should be numberOfElements.</param>
        /// <param name="numberOfElements">The number of elements in each variation.</param>
        /// <returns>Lazy enumerable of all possible variations.</returns>
        private static IEnumerable<IEnumerable<T>> GetVariations<T>(int index, T[] listCopy, IList<T> result, int numberOfElements)
        {
            for (var i = index; i < listCopy.Length; i++)
            {
                result[index] = listCopy[i];

                GeneralUtilities.Swap(ref listCopy[i], ref listCopy[index]);

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

                GeneralUtilities.Swap(ref listCopy[i], ref listCopy[index]);
            }
        }
    }
}