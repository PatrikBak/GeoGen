using System.Collections.Generic;
using System.Linq;

namespace GeoGen.Core.Utilities.Variations
{
    public class VariationsProvider<T> : IVariationsProvider<T>
    {
        private readonly ISubsetsGenerator<T> _subsetsGenerator;

        public VariationsProvider(ISubsetsGenerator<T> subsetsGenerator)
        {
            _subsetsGenerator = subsetsGenerator;
        }

        public IEnumerable<IEnumerable<T>> GetVariations(List<T> list, int numberOfElement)
        {
            foreach (var subset in _subsetsGenerator.Generate(list, numberOfElement))
            {
                var permutationArray = Enumerable.Range(0, numberOfElement).ToArray();

                do
                {
                    yield return permutationArray.Select(index => subset[index]);
                }
                while (NextPermutation(permutationArray));
            }
        }

        private static bool NextPermutation(int[] array)
        {
            var i = array.Length - 1;

            while (i > 0 && array[i - 1].CompareTo(array[i]) >= 0)
                i--;

            if (i <= 0)
                return false;

            var j = array.Length - 1;

            while (array[j].CompareTo(array[i - 1]) <= 0)
                j--;

            var temp = array[i - 1];
            array[i - 1] = array[j];
            array[j] = temp;

            j = array.Length - 1;

            while (i < j)
            {
                temp = array[i];
                array[i] = array[j];
                array[j] = temp;
                i++;
                j--;
            }

            return true;
        }
    }
}