using System.Collections.Generic;

namespace GeoGen.Core.Utilities.Combinator
{
    public interface ICombinator<TKey, TValue>
    {
        IEnumerable<IReadOnlyDictionary<TKey, TValue>> Combine(IReadOnlyDictionary<TKey, IEnumerable<TValue>> possibilities);
    }
}