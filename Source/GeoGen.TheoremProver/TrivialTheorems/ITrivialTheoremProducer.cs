using GeoGen.Core;
using System.Collections.Generic;

namespace GeoGen.TheoremProver
{
    /// <summary>
    /// Represents a service that can find theorems that be directly derived from a <see cref="ConstructedConfigurationObject"/>.
    /// </summary>
    public interface ITrivialTheoremProducer
    {
        /// <summary>
        /// Derive trivial theorems from a given constructed configuration object.
        /// </summary>
        /// <param name="constructedObject">The object from which we should derive theorems.</param>
        /// <returns>The produced theorems.</returns>
        IReadOnlyList<Theorem> DeriveTrivialTheoremsFromObject(ConstructedConfigurationObject constructedObject);
    }
}
