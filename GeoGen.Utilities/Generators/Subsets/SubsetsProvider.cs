using System;
using System.Collections.Generic;
using System.Linq;

namespace GeoGen.Utilities
{
    public sealed class SubsetsProvider : ISubsetsProvider
    {
        public IEnumerable<IEnumerable<T>> GetSubsets<T>(IReadOnlyList<T> list, int numberOfElements)
        {
            if (list == null)
                throw new ArgumentNullException(nameof(list));

            if (numberOfElements < 0 || numberOfElements > list.Count)
                throw new ArgumentOutOfRangeException(nameof(numberOfElements), "The number of elements should be in the interval [0, list.Count].");

            if (numberOfElements == 0)
                return new List<IEnumerable<T>> {Enumerable.Empty<T>()};

            return Combinations(numberOfElements, list.Count).Select(indices => indices.Select(i => list[i - 1]));
        }

        private static IEnumerable<int[]> Combinations(int k, int n)
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