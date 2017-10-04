using System.Collections.Generic;

namespace GeoGen.Core.Utilities.SubsetsProviding
{
    public interface ISubsetsProvider<T>
    {
        IEnumerable<IEnumerable<T>> GetSubsets(IReadOnlyList<T> list, int numberOfElements);
    }
}