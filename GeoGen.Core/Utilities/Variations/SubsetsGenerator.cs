using System.Collections.Generic;
using System.Linq;

namespace GeoGen.Core.Utilities.Variations
{
    public class SubsetsGenerator<T> : ISubsetsGenerator<T>
    {
        public IEnumerable<List<T>> Generate(List<T> superSet, int subsetsSize)
        {
            var numberOfInterations = 1 << superSet.Count;

            for (var i = numberOfInterations - 1; i >= 0; i--)
            {
                var combination = Enumerable.Repeat(default(T), superSet.Count).ToList();

                for (var j = 0; j < superSet.Count; j++)
                {
                    if ((i & 1 << superSet.Count - j - 1) != 0)
                    {
                        combination[j] = superSet[j];
                    }
                }

                var maybeResult = combination.Where(NotEqualToDefault).ToList();

                if (maybeResult.Count == subsetsSize)
                    yield return maybeResult;
            }
        }
        
        private static bool NotEqualToDefault(T element)
        {
            return !EqualityComparer<T>.Default.Equals(element, default(T));
        }
    }
}