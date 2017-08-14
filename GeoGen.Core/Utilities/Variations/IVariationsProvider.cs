using System.Collections.Generic;

namespace GeoGen.Core.Utilities.Variations
{
    public interface IVariationsProvider<T>
    {
        IEnumerable<IEnumerable<T>> GetVariations(List<T> list, int numberOfElement);
    }
}