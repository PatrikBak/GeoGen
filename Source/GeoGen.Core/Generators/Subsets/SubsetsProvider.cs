using System.Collections.Generic;
using System.Linq;

namespace GeoGen.Core
{
    /// <summary>
    /// A default implementation of <see cref="ISubsetsProvider"/>.
    /// </summary>
    public class SubsetsProvider : ISubsetsProvider
    {
        /// <summary>
        /// Generates all possible subsets of elements from a given list, with
        /// a given number of elements.
        /// </summary>
        /// <typeparam name="T">The type of elements within the list.</typeparam>
        /// <param name="list">The list of elements.</param>
        /// <param name="numberOfElements">The number of elements of each generated subset.</param>
        /// <returns>The enumerable of all possible subsets.</returns>
        public IEnumerable<IEnumerable<T>> GetSubsets<T>(IReadOnlyList<T> list, int numberOfElements)
        {
            // No element means exactly one (an empty) subset
            if (numberOfElements == 0)
                return new List<IEnumerable<T>> {Enumerable.Empty<T>()};

            // Call the internal method to generate the combinations of indices.
            return Combinations(list.Count, numberOfElements)
                    // Select each one to the combination of elements.
                    .Select(indices => indices.Select(i => list[i - 1]));
        }

        /// <summary>
        /// Generates all possible combinations of numbers [1,2,...,n] that have
        /// exactly 'k' elements. Its count should be (n choose k).
        /// </summary>
        /// <param name="n">The number of elements within a single combination.</param>
        /// <param name="k">The total number of elements.</param>
        /// <returns>The combinations.</returns>
        private IEnumerable<int[]> Combinations(int n, int k)
        {
            var result = new int[k];
            var stack = new Stack<int>();
            stack.Push(1);

            while (stack.Count > 0)
            {
                var index = stack.Count - 1;
                var value = stack.Pop();

                while (value <= n)
                {
                    result[index++] = value++;
                    stack.Push(value);

                    if (index != k)
                        continue;

                    yield return result;
                    break;
                }
            }
        }
    }
}