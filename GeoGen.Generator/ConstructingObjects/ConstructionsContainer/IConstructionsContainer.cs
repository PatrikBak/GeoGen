using System.Collections.Generic;

namespace GeoGen.Generator
{
    /// <summary>
    /// Represents a container for <see cref="ConstructionWrapper"/>.
    /// </summary>
    internal interface IConstructionsContainer : IEnumerable<ConstructionWrapper>
    {
    }
}