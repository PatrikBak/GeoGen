using System.Collections.Generic;

namespace GeoGen.Core.Utilities.Variations
{
    public interface IVariationsProvider
    {
        IEnumerable<int[]> GetVariations(int n, int k);
    }
}