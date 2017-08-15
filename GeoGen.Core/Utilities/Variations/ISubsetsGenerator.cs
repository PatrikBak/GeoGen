using System.Collections.Generic;

namespace GeoGen.Core.Utilities.Variations
{
    public interface ISubsetsGenerator<T> 
    {
        IEnumerable<List<T>> Generate(List<T> superSet, int subsetsSize);
    }
}