using System.Collections.Generic;
using GeoGen.Core.Constructions;

namespace GeoGen.Generator.ConstructingObjects
{
    /// <summary>
    /// Represents a container for <see cref="ConstructionWrapper"/>.
    /// </summary>
    internal interface IConstructionsContainer : IEnumerable<ConstructionWrapper>
    {
    }
}