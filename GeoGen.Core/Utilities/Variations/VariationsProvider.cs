using System.Collections.Generic;

namespace GeoGen.Core.Utilities.Variations
{
    public class VariationsProvider<T> : IVariationsProvider<T>
    {
        public IEnumerable<IEnumerable<T>> GetVariations(List<T> list, int numberOfElement)
        {
            return GetVariations(0, list.ToArray(), new T[numberOfElement], numberOfElement);
        }

        private static IEnumerable<IEnumerable<T>> GetVariations(int index, T[] listCopy, T[] result, int numberOfElements)
        {
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

        private static void Swap(ref T v1, ref T v2)
        {
            var old = v1;
            v1 = v2;
            v2 = old;
        }
    }
}