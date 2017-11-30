using System.Collections.Generic;

namespace GeoGen.Core.Utilities.SubsetsProviding
{
    public interface ISubsetsProvider
    {
        IEnumerable<IEnumerable<T>> GetSubsets<T>(IReadOnlyList<T> list, int numberOfElements);
    }
}