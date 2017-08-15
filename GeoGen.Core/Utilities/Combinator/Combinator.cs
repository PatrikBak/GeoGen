using System.Collections.Generic;
using System.Linq;

namespace GeoGen.Core.Utilities.Combinator
{
    public class Combinator<TKey, TValue> : ICombinator<TKey, TValue>
    {
        private IReadOnlyDictionary<TKey, IEnumerable<TValue>> _possibilities;

        private List<TKey> _keys;

        public IEnumerable<IReadOnlyDictionary<TKey, TValue>> Combine(IReadOnlyDictionary<TKey, IEnumerable<TValue>> possibilities)
        {
            // TODO: Handle exceptions and empty enumerables
            _possibilities = possibilities;
            _keys = possibilities.Keys.ToList();

            return GeneratePossibleArrays(0, new TValue[_keys.Count]).Select
            (
                array => array.Select((value, i) => new {value, i}).
                                ToDictionary(arg => _keys[arg.i], arg => arg.value)
            );
        }

        private IEnumerable<TValue[]> GeneratePossibleArrays(int currentindex, TValue[] currentArray)
        {
            foreach (var element in _possibilities[_keys[currentindex]])
            {
                currentArray[currentindex] = element;

                if (currentindex == _keys.Count - 1)
                {
                    yield return currentArray;
                }
                else
                {
                    foreach (var result in GeneratePossibleArrays(currentindex + 1, currentArray))
                    {
                        yield return result;
                    }
                }
            }
        }
    }
}