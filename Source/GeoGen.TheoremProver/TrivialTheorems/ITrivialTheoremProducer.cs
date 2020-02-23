using GeoGen.Core;
using System.Collections.Generic;

namespace GeoGen.TheoremProver
{
    /// <summary>
    /// Represents a service that finds <see cref="Theorem"/>s that be directly inferred from a <see cref="ConstructedConfigurationObject"/>.
    /// </summary>
    public interface ITrivialTheoremProducer
    {
        /// <summary>
        /// Infers trivial theorems from a given constructed configuration object.
        /// </summary>
        /// <param name="constructedObject">The constructed object from which we should infer theorems.</param>
        /// <returns>The inferred trivial theorems.</returns>
        IReadOnlyList<Theorem> InferTrivialTheoremsFromObject(ConstructedConfigurationObject constructedObject);
    }
}
