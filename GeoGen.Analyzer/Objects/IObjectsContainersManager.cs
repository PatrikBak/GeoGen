using System.Collections.Generic;
using GeoGen.Core.Configurations;

namespace GeoGen.Analyzer.Objects
{
    /// <summary>
    /// Represents a manager of all available <see cref="IObjectsContainer"/>s.
    /// </summary>
    internal interface IObjectsContainersManager : IEnumerable<IObjectsContainer>
    {
    }
}