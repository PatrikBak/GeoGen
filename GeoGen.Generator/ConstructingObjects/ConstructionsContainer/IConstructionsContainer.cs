using System.Collections.Generic;

namespace GeoGen.Generator
{
    /// <summary>
    /// Represents a container that contains all constructions used in the 
    /// generation process. It implements the <see cref="IEnumerable{T}"/>
    /// where T is <see cref="ConstructionWrapper"/>.
    /// </summary>
    internal interface IConstructionsContainer : IEnumerable<ConstructionWrapper>
    {
    }
}