using System.Collections.Generic;
using GeoGen.Core.Constructions;

namespace GeoGen.Generator.ConstructingObjects
{
    /// <summary>
    /// Represents a container for <see cref="ConstructionWrapper"/> that can be initialized
    /// from a list of <see cref="Construction"/>s. It implements the <see cref="IEnumerable{T}"/> 
    /// interface whose generic type is <see cref="ConstructionWrapper"/>.
    /// </summary>
    internal interface IConstructionsContainer : IEnumerable<ConstructionWrapper>
    {
        /// <summary>
        /// Initializes the container with a given enumerable of constructions.
        /// The ids of the constructions will be ignored.
        /// </summary>
        /// <param name="constructions">The constructions enumerable.</param>
        void Initialize(IEnumerable<Construction> constructions);
    }
}