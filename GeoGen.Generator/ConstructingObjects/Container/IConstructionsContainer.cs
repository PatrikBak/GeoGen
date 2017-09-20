using System.Collections.Generic;
using GeoGen.Core.Constructions;

namespace GeoGen.Generator.ConstructingObjects.Container
{
    /// <summary>
    /// Represents a container for <see cref="ConstructionWrapper"/> that can be initiliazed
    /// from a list of <see cref="Construction"/>s. It implements the <see cref="IEnumerable{T}"/> 
    /// interface whose generic type is <see cref="ConstructionWrapper"/>.
    /// </summary>
    internal interface IConstructionsContainer : IEnumerable<ConstructionWrapper>
    {
        /// <summary>
        /// Initializes the container with a given enumerable of constructions.
        /// It performs the check whether the constructions have distinct ids (which is needed
        /// during the generation process).
        /// </summary>
        /// <param name="constructions"></param>
        void Initialize(IEnumerable<Construction> constructions);
    }
}