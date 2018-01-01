using System;
using System.Collections.Generic;

namespace GeoGen.Analyzer
{
    /// <summary>
    /// Represents a manager of all available <see cref="IObjectsContainer"/>s.
    /// </summary>
    internal interface IObjectsContainersManager : IEnumerable<IObjectsContainer>
    {
        T ExecuteAndResolvePossibleIncosistencies<T>(Func<T> function);
    }
}